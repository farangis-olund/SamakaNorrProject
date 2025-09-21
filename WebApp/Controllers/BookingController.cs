using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Route("Booking")]
public class BookingController : Controller
{
    private readonly BookingService _bookingService;
    private readonly RideService _rideService;
    private readonly UserManager<UserEntity> _userManager;
    private readonly NotificationService _notificationService;
    private readonly DataContext _context;

    public BookingController(BookingService bookingService, UserManager<UserEntity> userManager, RideService rideService, NotificationService notificationService, DataContext dataContext)
    {
        _bookingService = bookingService;
        _userManager = userManager;
        _rideService = rideService;
        _notificationService = notificationService;
        _context = dataContext;
    }

    [HttpGet("GetBookingInfo")]
    public async Task<IActionResult> GetBookingInfo(int rideId, int bookingId)
    {
        var response = await _bookingService.GetBookingInfoByRideAndBookingIdAsync(rideId, bookingId);
        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok && response.ContentResult != null)
        {
            var booking = (BookingEntity)response.ContentResult!;

            var passenger = await _userManager.FindByIdAsync(booking.PassengerId.ToString());

            if (passenger == null)
            {
                return Json(new { message = "Passenger info not found." });
            }

            var contact = !string.IsNullOrEmpty(passenger.PhoneNumber) ? passenger.PhoneNumber : passenger.Email;

            return Json(new
            {
                passengerName = passenger.FirstName + " " + passenger.LastName,
                contact = contact,
                numberOfSeats = booking.NumberOfSeatsBooked,
                message = booking.BookingDetails 
            });
        }
        return Json(new { message = "Booking info not found." });
    }

    [HttpGet("GetDriverInfo")]
    public async Task<IActionResult> GetDriverInfo(int rideId)
    {
        var rideResponse = await _rideService.GetRideAsync(rideId);
        if (rideResponse.StatusCode == Infrastructure.Models.StatusCode.Ok && rideResponse.ContentResult != null)
        {
            var ride = (RideEntity)rideResponse.ContentResult!;

            var driver = await _userManager.FindByIdAsync(ride.DriverId.ToString());

            if (driver == null)
            {
                return Json(new { message = "Driver info not found." });
            }

            var contact = !string.IsNullOrEmpty(driver.PhoneNumber)
                ? driver.PhoneNumber
                : driver.Email;

            return Json(new
            {
                driverName = $"{driver.FirstName} {driver.LastName}",
                contact = contact
            });
        }

        return Json(new { message = "Ride or driver not found." });
    }

    //// Confirm booking
    //[HttpPost("Confirm")]
    //public async Task<IActionResult> Confirm([FromForm] int bookingId)
    //{
    //    var response = await _bookingService.ConfirmBookingAsync(bookingId);

    //            if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
    //            {
    //                TempData["StatusMessage"] = "success|Bokningen har bekräftats!";
    //                return RedirectToAction("Driver", "Account");
    //            }
    //            TempData["StatusMessage"] = "danger|Misslyckades att bekräfta bokning!";
    //            return RedirectToAction("Driver", "Account");

    //}

    //// Complete booking
    //[HttpPost("Complete")]
    //public async Task<IActionResult> Complete([FromForm] int bookingId)
    //{
    //         var response = await _bookingService.CompleteBookingAsync(bookingId);

    //            if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
    //            {
    //                TempData["StatusMessage"] = "success|Bokningen har markerats som avslutad!";
    //                return RedirectToAction("Driver", "Account");
    //            }

    //            TempData["StatusMessage"] = "danger|Misslyckades att avsluta bokning!";
    //            return RedirectToAction("Driver", "Account");

    //}


    //// Cancel booking
    //[HttpPost("Cancel")]
    //public async Task<IActionResult> Cancel(int bookingId)
    //{
    //    var response = await _bookingService.DeleteBookingAsync(bookingId);

    //    if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
    //    {
    //        TempData["StatusMessage"] = "success|Bokningen har avbokat!";
    //        return RedirectToAction("Passenger", "Account");
    //    }

    //    TempData["StatusMessage"] = "danger|Misslyckades att avboka bokningen!";
    //    return RedirectToAction("Passenger", "Account");
    //}


    //// Reject  booking request
    //[HttpPost("Reject")]
    //public async Task<IActionResult> Reject(int bookingId)
    //{
    //    var response = await _bookingService.RejectBookingAsync(bookingId);

    //    if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
    //    {
    //        TempData["StatusMessage"] = "success|Bokningen har avvisad!";
    //        return RedirectToAction("Driver", "Account");
    //    }

    //    TempData["StatusMessage"] = "danger|Misslyckades att avvisa bokningen!";
    //    return RedirectToAction("Driver", "Account");
    //}

    // Delete booking


    // Confirm booking
    [HttpPost("Confirm")]
    public async Task<IActionResult> Confirm([FromForm] int bookingId)
    {
        var response = await _bookingService.ConfirmBookingAsync(bookingId);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
        {
            // 🔔 Add notification for passenger
            var booking = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Ride)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                await _notificationService.AddNotificationAsync(new NotificationEntity
                {
                    UserId = booking.PassengerId,
                    Title = "Bokning bekräftad ✅",
                    Message = $"Din bokning för resan {booking.Ride.Origin} → {booking.Ride.Destination} har bekräftats."
                });
            }

            TempData["StatusMessage"] = "success|Bokningen har bekräftats!";
            return RedirectToAction("Driver", "Account");
        }

        TempData["StatusMessage"] = "danger|Misslyckades att bekräfta bokning!";
        return RedirectToAction("Driver", "Account");
    }

    // Complete booking
    [HttpPost("Complete")]
    public async Task<IActionResult> Complete([FromForm] int bookingId)
    {
        var response = await _bookingService.CompleteBookingAsync(bookingId);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
        {
            var booking = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Ride)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                await _notificationService.AddNotificationAsync(new NotificationEntity
                {
                    UserId = booking.PassengerId,
                    Title = "Resan avslutad 🏁",
                    Message = $"Din resa {booking.Ride.Origin} → {booking.Ride.Destination} markerades som avslutad."
                });
            }

            TempData["StatusMessage"] = "success|Bokningen har markerats som avslutad!";
            return RedirectToAction("Driver", "Account");
        }

        TempData["StatusMessage"] = "danger|Misslyckades att avsluta bokning!";
        return RedirectToAction("Driver", "Account");
    }

    // Cancel booking
    [HttpPost("Cancel")]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var response = await _bookingService.DeleteBookingAsync(bookingId);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
        {
            var booking = await _context.Bookings
                .Include(b => b.Ride)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                await _notificationService.AddNotificationAsync(new NotificationEntity
                {
                    UserId = booking.Ride.DriverId,
                    Title = "Bokning avbokad ❌",
                    Message = $"En passagerare avbokade resan {booking.Ride.Origin} → {booking.Ride.Destination}."
                });
            }

            TempData["StatusMessage"] = "success|Bokningen har avbokat!";
            return RedirectToAction("Passenger", "Account");
        }

        TempData["StatusMessage"] = "danger|Misslyckades att avboka bokningen!";
        return RedirectToAction("Passenger", "Account");
    }

    // Reject booking request
    [HttpPost("Reject")]
    public async Task<IActionResult> Reject(int bookingId)
    {
        var response = await _bookingService.RejectBookingAsync(bookingId);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
        {
            var booking = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Ride)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                await _notificationService.AddNotificationAsync(new NotificationEntity
                {
                    UserId = booking.PassengerId,
                    Title = "Bokning avvisad ❌",
                    Message = $"Din bokning för resan {booking.Ride.Origin} → {booking.Ride.Destination} har avvisats."
                });
            }

            TempData["StatusMessage"] = "success|Bokningen har avvisad!";
            return RedirectToAction("Driver", "Account");
        }

        TempData["StatusMessage"] = "danger|Misslyckades att avvisa bokningen!";
        return RedirectToAction("Driver", "Account");
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete([FromForm] int bookingId)
    {
        var response = await _bookingService.DeleteBookingAsync(bookingId);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
        {
            TempData["StatusMessage"] = "success|Bokningen har raderats!";
            return RedirectToAction("Passenger", "Account");
        }

        TempData["StatusMessage"] = "danger|Misslyckades att radera bokningen!";
        return RedirectToAction("Passenger", "Account");
    }
}
