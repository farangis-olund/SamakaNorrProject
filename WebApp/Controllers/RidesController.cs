using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class RidesController(DataContext context, RideService rideService, UserManager<UserEntity> userManager, BookingService bookingService) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RideService _rideService = rideService;
    private readonly BookingService _bookingService = bookingService;
    private readonly DataContext _context = context;


    #region Index
    [HttpGet]
	[Route("/trips")]
	public async Task<IActionResult> Index(string? statusMessage)
	{
		var viewModel = new RidesViewModel
		{
			SearchModel = new RideSearchModel() 
		};

		if (statusMessage != null)
		{
			ViewData["StatusMessage"] = statusMessage;
		}
		
		var result = await _rideService.GetAllNotApprovedRidesAsync();

     
        if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
		{
            var rides = (List<RideEntity>)result.ContentResult!;

            var futureRides = rides
            .Where(r => r.DepartureTime >= DateTime.Now)
            .OrderByDescending(r => r.DepartureTime)
            .ToList();


            foreach (var ride in futureRides)
			{
				var userEntity = await _userManager.FindByIdAsync(ride.DriverId); 
				var rideItem = new RideModel
				{
					Id = ride.Id,
					Origin = ride.Origin,
					Destination = ride.Destination,
					DepartureTime = ride.DepartureTime,
					DriverName = userEntity!.FirstName + " " + userEntity.LastName,
					UserImgUrl = userEntity.ProfileImgUrl,
					Price = ride.Price,
					Free = ride.Free,
					TripDetails = ride.TripDetails
				};
				viewModel.Rides.Add(rideItem);
			}
            			
			//viewModel.Rides = viewModel.Rides.OrderByDescending(r => r.DepartureTime).ToList();
		}
		
		return View(viewModel);
	}
	
    [HttpPost]
    [Route("/trips")]
    public async Task<IActionResult> Index(RidesViewModel viewModel)
    {
        if (viewModel.SearchModel != null)
        {
           
            var searchModel = viewModel.SearchModel;
            var result = await _rideService.SearchRidesAsync(searchModel);

            if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
            {
                var rides = (List<RideEntity>)result.ContentResult!;

                var upcomingRides = rides
                  .Where(ride => ride.DepartureTime >= DateTime.Now)
                  .ToList();

                viewModel.Rides = upcomingRides.Select(ride => new RideModel
                {
                    Id = ride.Id,
                    Origin = ride.Origin,
                    Destination = ride.Destination,
                    DepartureTime = ride.DepartureTime,
                    Price = ride.Price,
                    Free = ride.Free
                }).ToList();

                return View(viewModel);
            }

            ViewData["StatusMessage"] = "warning|No rides found matching your search.";
            return View(viewModel);
        }

        ViewData["StatusMessage"] = "warning|Invalid search criteria.";
        viewModel.Rides = []; 
        return View(viewModel);
    }


	#endregion

	#region Single trip
	[Authorize]
	[HttpGet]
    [Route("/trip")]
    public async Task<IActionResult> SingleRide(int id)
    {
        var result = await _rideService.GetRideAsync(id);
        var ride = (RideEntity)result.ContentResult!;
        var userEntity = _userManager.FindByIdAsync(ride.DriverId);
        var viewModel = new RideModel
        {
            Id = ride.Id,
            Origin = ride.Origin,
            Destination = ride.Destination,
            DepartureTime = ride.DepartureTime,
            DriverName = userEntity.Result!.FirstName + " " + userEntity.Result!.LastName,
            UserImgUrl = userEntity.Result!.ProfileImgUrl,
            Price = ride.Price,
            Free = ride.Free,
            TripDetails = ride.TripDetails,
            AvailableSeats = ride.AvailableSeats,
            DriverId= ride.DriverId,
            Messages = ride.Messages
            .OrderBy(m => m.Timestamp)
            .Select(m => new MessageModel
            {
                Sender = _context.Users
                    .Where(u => u.Email == m.SenderId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? m.SenderId, 

                Text = m.MessageContent,
                Timestamp = m.Timestamp
            }).ToList()
        };

        return View(viewModel);
    }
    #endregion

    #region Booking
    [Authorize] 
    [HttpPost]
    [Route("/booking")]
    public async Task<IActionResult> Booking(RideModel model)
    {
        try
        {
            var userEntity = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                var bookingEntity = new BookingEntity
                {
                    RideId = model.Id,
                    PassengerId = userEntity.Id,
                    BookingTime = DateTime.Now,
                    NumberOfSeatsBooked = model.RequiredSeats,
                    BookingStatus = BookingStatus.Pending,
                    BookingDetails = model.PassangerMessage
                };

                var response = await _bookingService.AddBookingAsync(bookingEntity);
                if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
                {
                    var statusMessage = "success|Bokningen har skickats!";
                    return RedirectToAction("Index", new { statusMessage });
                } else if(response.StatusCode == Infrastructure.Models.StatusCode.Exists)
                {
                    var statusMessage = "warning|Du har redan bokat denna resa.";
                    return RedirectToAction("Index", new { statusMessage });
                }
                else if (response.StatusCode == Infrastructure.Models.StatusCode.Error && response.Message == "Du kan inte boka din egen resa.")
                {
                    var statusMessage = "warning|Du kan inte boka din egen resa.";
                    return RedirectToAction("Index", new { statusMessage });
                }
            }

            ViewData["StatusMessage"] = "danger|Incorrect data!";
            return View(model);
        }
        catch (Exception ex)
        {
            // Optional: log the error
            ViewData["StatusMessage"] = "danger|Ett fel inträffade vid bokningen. Försök igen senare.";
            return View(model);
        }
    }
    #endregion

    #region Deleting
    [Authorize]
    [HttpPost]
    [Route("/delete")]
    public async Task<IActionResult> Delete([FromForm] int rideId)
    {
        var response = await _rideService.DeleteRideAsync(rideId) ;
            if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
            {
                TempData["StatusMessage"] = "success|Resan har tagits bort!";
                return RedirectToAction("Driver", "Account");
            }
        

        TempData["StatusMessage"] = "error|Något gick fel. Försök igen!";
        return View();
    }
    #endregion
}
