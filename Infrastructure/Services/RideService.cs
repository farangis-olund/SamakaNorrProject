using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class RideService(RideRepository rideRepository, BookingRepository bookingRepository, UserManager<UserEntity> userManager)
{
	private readonly RideRepository _rideRepository = rideRepository;
    private readonly BookingRepository _bookingRepository = bookingRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<ResponseResult> AddRideAsync(RideRegistrationModel ride)
	{
		
		try
		{
			var userEntity = await _userManager.FindByEmailAsync(ride.DriverEmail);
            if (userEntity != null)
			{
                var newRideEntity = new RideEntity
                {
                    DriverId = userEntity.Id,
					Origin = ride.Origin,
                    Destination = ride.Destination,
                    AvailableSeats = ride.AvailableSeats,
                    Free = ride.Free,
                    Price = ride.Price,
                    TripDetails = ride.TripDetails,
                    DepartureTime = ride.DepartureTime
                };
                var result = await _rideRepository.AddAsync(newRideEntity);

                return ResponseFactory.Ok(result);
            }
            return ResponseFactory.NotFound("User not found, first you should registrate your account!");

        }
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult> GetRideAsync(string id)
	{
		try
		{
			var result = await _rideRepository.GetOneAsync(c => c.DriverId == id);
			if (result.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(result.ContentResult!);
			return ResponseFactory.NotFound();

		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> GetRideAsync(int id)
    {
        try
        {
            var result = await _rideRepository.GetOneAsync(c => c.Id == id);
			if (result.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(result.ContentResult!);
			return ResponseFactory.NotFound();

		}
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetAllRidesAsync()
	{
		try
		{
			var courseEntities = await _rideRepository.GetAllAsync();

			if (courseEntities.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(courseEntities.ContentResult!);
			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> GetAllNotApprovedRidesAsync()
    {
        try
        {
            var rideResponse = await _rideRepository.GetAllAsync(r => true); 

            if (rideResponse.StatusCode != StatusCode.Ok || rideResponse.ContentResult == null)
                return ResponseFactory.NotFound("No rides found.");

            var allRides = (List<RideEntity>)rideResponse.ContentResult!;

            var unapprovedRides = allRides
               .Where(r => !r.Bookings.Any(b =>
                b.BookingStatus == BookingStatus.Confirmed ||
                b.BookingStatus == BookingStatus.Completed))
                .ToList();

            return ResponseFactory.Ok(unapprovedRides);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error("Failed to fetch unapproved rides: " + ex.Message);
        }
    }




    public async Task<ResponseResult> GetAllRidesAsync(string userId)
    {
        try
        {
            var courseEntities = await _rideRepository.GetAllAsync(c => c.DriverId == userId);

            if (courseEntities.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(courseEntities.ContentResult!);
            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    //public async Task<ResponseResult> UpdateRideAsync(int id, RideEntity ride)
    //{
    //	try
    //	{
    //		var response = await _courseRepository.GetOneAsync(c => c.Id == id);

    //		if (response.StatusCode == StatusCode.Ok)
    //		{
    //			var existingcourse = (CourseEntity)response.ContentResult!;
    //			existingcourse.Name = course.Name;
    //			existingcourse.Description = course.Description;
    //			existingcourse.CreatedDate = course.CreatedDate;
    //			existingcourse.Duration = course.Duration;
    //			existingcourse.ArticleCount	= course.ArticleCount;
    //			existingcourse.BestSeller = course.BestSeller;
    //			existingcourse.Digital = course.Digital;
    //			existingcourse.ProgramDetails = course.ProgramDetails;
    //			existingcourse.Price = course.Price;
    //			existingcourse.DiscountPrice = course.DiscountPrice;
    //			existingcourse.Ingress = course.Ingress;
    //			existingcourse.DownloadedResourses = course.DownloadedResourses;
    //			existingcourse.ImgUrl = course.ImgUrl;

    //			var updateResponse = await _courseRepository.UpdateAsync(c => c.Id == course.Id, existingcourse);

    //			return ResponseFactory.Ok(updateResponse);
    //		}
    //		else
    //		{
    //			return response;
    //		}
    //	}
    //	catch (Exception ex)
    //	{
    //		return ResponseFactory.Error(ex.Message);
    //	}
    //}

    public async Task<ResponseResult> DeleteRideAsync(int id)
	{
		try
		{
			var existingcourse = await _rideRepository.GetOneAsync(x => x.Id == id);

			if (existingcourse != null)
			{
				await _rideRepository.RemoveAsync(c => c.Id == id);
				return ResponseFactory.Ok("Successfully removed!");
			}

			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> SearchRidesAsync(RideSearchModel model)
    {
        try
        {
            // Retrieve all rides using your existing method
            var allRidesResponse = await GetAllNotApprovedRidesAsync();

            if (allRidesResponse.StatusCode == StatusCode.Ok)
            {
                var allRides = (List<RideEntity>)allRidesResponse.ContentResult!;

                // Filter rides based on the search criteria
                var filteredRides = allRides.Where(ride =>
                    (string.IsNullOrEmpty(model.Origin) || ride.Origin.Contains(model.Origin, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(model.Destination) || ride.Destination.Contains(model.Destination, StringComparison.OrdinalIgnoreCase)) &&
                    (!model.DepartureTime.HasValue || ride.DepartureTime.Date == model.DepartureTime.Value.Date))
                .ToList();

                return ResponseFactory.Ok(filteredRides);
            }

            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    //public async Task<ResponseResult> GetAllRidesWithStatusAsync(string userId)
    //{
    //    try
    //    {

    //        var allRides = await _rideRepository.GetAllAsync(c => c.DriverId == userId);

    //        if (allRides.StatusCode != StatusCode.Ok || allRides.ContentResult == null)
    //            return ResponseFactory.NotFound("No rides found.");

    //        var ridesWithStatus = new List<RideModel>();

    //        foreach (var ride in (List<RideEntity>)allRides.ContentResult)
    //        {

    //            var bookingResponse = await _bookingRepository.GetAllAsync(b => b.RideId == ride.Id);
    //            BookingStatus rideStatus;
    //            int bookingId = 0;
    //            if (bookingResponse.StatusCode == StatusCode.Ok && bookingResponse.ContentResult != null)
    //            {
    //                var booking = (List<BookingEntity>)bookingResponse.ContentResult;

    //                if (!booking.Any())
    //                {
    //                    rideStatus = BookingStatus.NotBooked;
    //                }
    //                else if (booking.Any(b => b.BookingStatus == BookingStatus.Pending))
    //                {
    //                    rideStatus = BookingStatus.Pending;
    //                    bookingId = booking.Id;
    //                }
    //                else if (booking.Any(b => b.BookingStatus == BookingStatus.Confirmed))
    //                {
    //                    rideStatus = BookingStatus.Confirmed;
    //                    bookingId = booking.Id;
    //                }
    //                else if (booking.Any(b => b.BookingStatus == BookingStatus.Completed))
    //                {
    //                    rideStatus = BookingStatus.Completed;
    //                    bookingId = booking.Id;
    //                }
    //                else
    //                {
    //                    rideStatus = BookingStatus.NotBooked;
    //                }
    //            }
    //            else
    //            {
    //                rideStatus = BookingStatus.NotBooked;
    //            }



    //            var rideModel = new RideModel
    //            {
    //                Id = ride.Id,
    //                Origin = ride.Origin,
    //                Destination = ride.Destination,
    //                DepartureTime = ride.DepartureTime,
    //                DriverName = ride.DriverId,
    //                DriverId = ride.DriverId,
    //                Price = ride.Price,
    //                Free = ride.Free,
    //                TripDetails = ride.TripDetails,
    //                AvailableSeats = ride.AvailableSeats,
    //                BookingStatus = rideStatus,
    //                Messages = ride.Messages
    //                    .OrderBy(m => m.Timestamp)
    //                    .Select(m => new MessageModel
    //                    {
    //                        Sender = m.SenderId,
    //                        Text = m.MessageContent,
    //                        Timestamp = m.Timestamp
    //                    }).ToList(),
    //                HasUnreadMessages = ride.Messages.Any(m => m.IsRead==false && m.ReceiverId == userId && m.SenderId != userId)
    //            };

    //            ridesWithStatus.Add(rideModel);
    //        }

    //        return ResponseFactory.Ok(ridesWithStatus.OrderByDescending(r => r.DepartureTime).ToList());
    //    }
    //    catch (Exception ex)
    //    {
    //        return ResponseFactory.Error($"Error retrieving rides with status: {ex.Message}");
    //    }
    //}

    public async Task<ResponseResult> GetAllRidesWithStatusAsync(string userId)
    {
        try
        {
            var allRides = await _rideRepository.GetAllAsync(c => c.DriverId == userId);

            if (allRides.StatusCode != StatusCode.Ok || allRides.ContentResult == null)
                return ResponseFactory.NotFound("No rides found.");

            var ridesWithStatus = new List<RideModel>();

            foreach (var ride in (List<RideEntity>)allRides.ContentResult)
            {
                var bookingResponse = await _bookingRepository.GetAllAsync(b => b.RideId == ride.Id);

                if (bookingResponse.StatusCode == StatusCode.Ok &&
                    bookingResponse.ContentResult is List<BookingEntity> bookings &&
                    bookings.Any())
                {
                    foreach (var booking in bookings)
                    {
                        var rideModel = new RideModel
                        {
                            Id = ride.Id,
                            Origin = ride.Origin,
                            Destination = ride.Destination,
                            DepartureTime = ride.DepartureTime,
                            DriverName = ride.DriverId,
                            DriverId = ride.DriverId,
                            Price = ride.Price,
                            Free = ride.Free,
                            TripDetails = ride.TripDetails,
                            AvailableSeats = ride.AvailableSeats,
                            BookingStatus = booking.BookingStatus,
                            BookingId = booking.Id,
                            Messages = ride.Messages
                                .OrderBy(m => m.Timestamp)
                                .Select(m => new MessageModel
                                {
                                    Sender = m.SenderId,
                                    Text = m.MessageContent,
                                    Timestamp = m.Timestamp
                                }).ToList(),
                            HasUnreadMessages = ride.Messages.Any(m =>
                                !m.IsRead && m.ReceiverId == userId && m.SenderId != userId)
                        };

                        ridesWithStatus.Add(rideModel);
                    }
                }
                else
                {
                    
                    var rideModel = new RideModel
                    {
                        Id = ride.Id,
                        Origin = ride.Origin,
                        Destination = ride.Destination,
                        DepartureTime = ride.DepartureTime,
                        DriverName = ride.DriverId,
                        DriverId = ride.DriverId,
                        Price = ride.Price,
                        Free = ride.Free,
                        TripDetails = ride.TripDetails,
                        AvailableSeats = ride.AvailableSeats,
                        BookingStatus = BookingStatus.NotBooked,
                        BookingId = 0,
                        Messages = ride.Messages
                            .OrderBy(m => m.Timestamp)
                            .Select(m => new MessageModel
                            {
                                Sender = m.SenderId,
                                Text = m.MessageContent,
                                Timestamp = m.Timestamp
                            }).ToList(),
                        HasUnreadMessages = ride.Messages.Any(m =>
                            !m.IsRead && m.ReceiverId == userId && m.SenderId != userId)
                    };

                    ridesWithStatus.Add(rideModel);
                }
            }

            return ResponseFactory.Ok(ridesWithStatus.OrderByDescending(r => r.DepartureTime).ToList());
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error retrieving rides with status: {ex.Message}");
        }
    }


    public async Task<ResponseResult> GetAllPassengerRidesAsync(string userId)
    {
        try
        {
            var bookingResponse = await _bookingRepository.GetAllAsync(b => b.PassengerId == userId);

            if (bookingResponse.StatusCode != StatusCode.Ok || bookingResponse.ContentResult == null)
            {
                return ResponseFactory.NotFound("No bookings found for this user.");
            }

            var bookings = (List<BookingEntity>)bookingResponse.ContentResult;
            var rideIds = bookings.Select(b => b.RideId).Distinct().ToList();

            var rideResponse = await _rideRepository.GetAllAsync(r => rideIds.Contains(r.Id));

            if (rideResponse.StatusCode != StatusCode.Ok || rideResponse.ContentResult == null)
            {
                return ResponseFactory.NotFound("No rides found for these bookings.");
            }

            var rides = (List<RideEntity>)rideResponse.ContentResult;
            var rideModels = new List<BookedRideModel>();

            foreach (var ride in rides)
            {
                var userBooking = bookings.FirstOrDefault(b => b.RideId == ride.Id);

                var rideModel = new BookedRideModel
				{
                    RideId = ride.Id,
                    BookingId = userBooking!.Id,
                    Origin = ride.Origin,
                    Destination = ride.Destination,
                    DepartureTime = ride.DepartureTime,
                    DriverName = ride.DriverId, 
                    DriverId = ride.DriverId,
                    Price = ride.Price,
                    Free = ride.Free,
                    TripDetails = ride.TripDetails,
                    AvailableSeats = ride.AvailableSeats,
                    BookingStatus = userBooking?.BookingStatus ?? BookingStatus.NotBooked,
                    
                    Messages = ride.Messages
                        .OrderBy(m => m.Timestamp)
                        .Select(m => new MessageModel
                        {
                            Sender = m.SenderId,
                            Text = m.MessageContent,
                            Timestamp = m.Timestamp
                        }).ToList(),
                    HasUnreadMessages = ride.Messages.Any(m => m.IsRead == false && m.SenderId != userId)

                };

                rideModels.Add(rideModel);
            }

            return ResponseFactory.Ok(rideModels.OrderByDescending(r => r.DepartureTime).ToList());
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error("An error occurred: " + ex.Message);
        }
    }


}
