using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using WebApp.ViewModels;

namespace WebApp.Services;

public class AccountService(UserManager<UserEntity> userManager,
						  AddressService addressService,
						  RideService rideService )
{
	private readonly UserManager<UserEntity> _userManager = userManager;
	private readonly AddressService _addressService = addressService;
	private readonly RideService _rideService = rideService;
	

	public async Task<AccountDetailViewModel> GetAccountDetailsAsync(UserEntity user)
	{
		// Fetch user details
		var userDetails = new AccountDetailViewModel
		{
			ProfileInfo = new AccountProfileModel
			{
				ProfileImgUrl = user.ProfileImgUrl,
				FirstName = user.FirstName,
				LastName = user.LastName,
				UserName = user.Email!,
				Email = user.Email!,
				IsExternalAccount = user.IsExternalAccount
			},
			BasicInfo = new AccountDetailBasicInfoModel
			{
				UserId = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email!,
				Phone = user.PhoneNumber!,
				Biography = user.Bio
			}
		};

		
		if (user.AddressId.HasValue)
		{
			var addressResponse = await _addressService.GetAddressAsync(user.AddressId.Value);
			if (addressResponse.StatusCode == StatusCode.Ok)
			{
				var address = (AddressEntity)addressResponse.ContentResult!;
				userDetails.AddressInfo = new AccountDetailAddressInfoModel
				{
					Addressline_1 = address.Addressline_1,
					Addressline_2 = address.Addressline_2,
					City = address.City,
					PostalCode = address.PostalCode
				};
			}
		}
        // Saved rides (as driver)
        userDetails.SavedRides ??= new RidesViewModel { Rides = [] };
        var savedRidesResponse = await _rideService.GetAllRidesWithStatusAsync(user.Id);
        if (savedRidesResponse.StatusCode == StatusCode.Ok)
        {
            foreach (var ride in (List<RideModel>)savedRidesResponse.ContentResult!)
            {
                var rideItem = new RideModel
                {
                    Id = ride.Id,
                    Origin = ride.Origin,
                    Destination = ride.Destination,
                    DepartureTime = ride.DepartureTime,
                    DriverName = user.FirstName,
                    UserImgUrl = user.ProfileImgUrl,
                    Price = ride.Price,
					DriverId = ride.DriverId,
                    Free = ride.Free,
                    TripDetails = ride.TripDetails,
                    AvailableSeats = ride.AvailableSeats,
                    BookingStatus = ride.BookingStatus,
                    Messages = ride.Messages
                        .OrderBy(m => m.Timestamp)
                        .Select(m => new MessageModel
                        {
                            Sender = m.Sender,
							Receiver = m.Receiver,
                            Text = m.Text,
                            Timestamp = m.Timestamp
                        }).ToList(),
                    HasUnreadMessages = ride.HasUnreadMessages

                };
				userDetails.SavedRides.Rides.Add(rideItem);
            }
        }

        // Passenger rides
        var passengerRideResponse = await _rideService.GetAllPassengerRidesAsync(user.Id);
        if (passengerRideResponse.StatusCode == StatusCode.Ok)
        {
            var passengerRides = (List<BookedRideModel>)passengerRideResponse.ContentResult!;

            userDetails.PassengerRides = passengerRides.Select(ride => new BookedRideModel
            {
                RideId = ride.RideId,
				BookingId = ride.BookingId,
                Origin = ride.Origin,
                Destination = ride.Destination,
                DepartureTime = ride.DepartureTime,
                DriverName = ride.DriverName,
                Price = ride.Price,
                Free = ride.Free,
                TripDetails = ride.TripDetails,
                AvailableSeats = ride.AvailableSeats,
                RequiredSeats = ride.RequiredSeats,
                BookingStatus = ride.BookingStatus,
				DriverId = ride.DriverId,
                Messages = ride.Messages
                        .OrderBy(m => m.Timestamp)
                        .Select(m => new MessageModel
                        {
                            Sender = m.Sender,
                            Receiver = m.Receiver,
                            Text = m.Text,
                            Timestamp = m.Timestamp
                        }).ToList(),
                HasUnreadMessages = ride.HasUnreadMessages
            }).ToList();
        }
        return userDetails;
	}

	public async Task<bool> UpdateAccountAsync(UserEntity user, AccountDetailViewModel viewModel)
	{
		// Fetch the current user
		if (user == null)
		{
			return false;
		}

		// Update basic info if provided
		if (viewModel.BasicInfo != null)
		{
			if (viewModel.BasicInfo.FirstName != null && viewModel.BasicInfo.LastName != null && viewModel.BasicInfo.Email != null)
			{
				user.FirstName = viewModel.BasicInfo.FirstName;
				user.LastName = viewModel.BasicInfo.LastName;
				user.Email = viewModel.BasicInfo.Email;
				user.UserName = viewModel.BasicInfo.Email;
				user.PhoneNumber = viewModel.BasicInfo.Phone;
				user.Bio = viewModel.BasicInfo.Biography;

				var basicInfoUpdateResult = await _userManager.UpdateAsync(user);
				if (!basicInfoUpdateResult.Succeeded)
				{
					// Failed to update basic info
					return false;
				}
			}
		}

		// Update address info if provided
		if (viewModel.AddressInfo != null)
		{
			if (viewModel.AddressInfo.Addressline_1 != null && viewModel.AddressInfo.City != null && viewModel.AddressInfo.PostalCode != null)
			{
				if (user.AddressId.HasValue)
				{
					// Update existing address
					var addressResponse = await _addressService.GetAddressAsync(user.AddressId.Value);
					if (addressResponse.StatusCode == StatusCode.Ok)
					{
						var addressEntity = (AddressEntity)addressResponse.ContentResult!;
						addressEntity.Addressline_1 = viewModel.AddressInfo.Addressline_1;
						addressEntity.Addressline_2 = viewModel.AddressInfo.Addressline_2;
						addressEntity.City = viewModel.AddressInfo.City;
						addressEntity.PostalCode = viewModel.AddressInfo.PostalCode;

						var addressUpdateResult = await _addressService.UpdateAddressAsync(addressEntity);
						if (addressUpdateResult.StatusCode != StatusCode.Ok)
						{
							// Failed to update address
							return false;
						}
					}
				}
				else
				{
					// Create new address
					var newAddress = new AddressEntity
					{
						Addressline_1 = viewModel.AddressInfo.Addressline_1,
						Addressline_2 = viewModel.AddressInfo.Addressline_2,
						City = viewModel.AddressInfo.City,
						PostalCode = viewModel.AddressInfo.PostalCode
					};

					var addressAddResult = await _addressService.AddAddressAsync(newAddress);
					if (addressAddResult.StatusCode == StatusCode.Ok)
					{
						var address = (AddressEntity)addressAddResult.ContentResult!;
						user.AddressId = address.Id;

						var userUpdateResult = await _userManager.UpdateAsync(user);
						if (!userUpdateResult.Succeeded)
						{
							// Failed to update user with new address ID
							return false;
						}
					}
					else
					{
						// Failed to add new address
						return false;
					}
				}
			}
		}

		return true;
	}

}
