using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Services;

public class BookingService(BookingRepository bookingRepository, UserManager<UserEntity> userManager, RideService rideService)
{
	private readonly BookingRepository _bookingRepository = bookingRepository;
	private readonly UserManager<UserEntity> _userManager = userManager;
	private readonly RideService _rideService = rideService;
    public async Task<ResponseResult> AddBookingAsync(BookingEntity entity)
    {
        try
        {
            var response = await _rideService.GetRideAsync(entity.RideId);

            if (response.StatusCode != StatusCode.Ok)
            {
                return ResponseFactory.NotFound("Trip not found, first you should register your account!");
            }

            var ride = (RideEntity)response.ContentResult!;
            // Prevent booking own ride
            if (ride.DriverId == entity.PassengerId)
            {
                return ResponseFactory.Error("Du kan inte boka din egen resa.");
            }

            // Check if the user already booked this ride
            var existingBooking = await GetByUserAndRideAsync(entity.PassengerId, entity.RideId);
            if (existingBooking.StatusCode == StatusCode.Ok)
            {
                return ResponseFactory.Exists("Du har redan bokat denna resa.");
            }

            var result = await _bookingRepository.AddAsync(entity);
            return ResponseFactory.Ok(result);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }


    public async Task<ResponseResult> GetBookingByPassengerIdAsync(string id)
	{
		try
		{
			var result = await _bookingRepository.GetOneAsync(c => c.PassengerId == id);
			if (result.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(result.ContentResult!);
			return ResponseFactory.NotFound();

		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> GetBookingByRideIdAsync(int id)
    {
        try
        {
            var result = await _bookingRepository.GetOneAsync(c => c.RideId == id);
            if (result.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(result.ContentResult!);
            return ResponseFactory.NotFound();

        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetBookingAsync(int id)
	{
		try
		{
			var result = await _bookingRepository.GetOneAsync(c => c.Id == id);
			if (result.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(result.ContentResult!);
			return ResponseFactory.NotFound();

		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> GetByUserAndRideAsync(string passengerId, int rideId)
    {
        try
        {
            var result = await _bookingRepository.GetOneAsync(c => c.PassengerId == passengerId && c.RideId == rideId);

            if (result.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(result);

            return ResponseFactory.NotFound("Ingen bokning hittades för denna användare och resa.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }


    public async Task<ResponseResult> GetAllBookingsAsync()
	{
		try
		{
			var courseEntities = await _bookingRepository.GetAllAsync();

			if (courseEntities.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(courseEntities.ContentResult!);
			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult> GetAllBookingsAsync(string userId)
	{
		try
		{
			var courseEntities = await _bookingRepository.GetAllAsync(c => c.PassengerId == userId);

			if (courseEntities.StatusCode == StatusCode.Ok)
				return ResponseFactory.Ok(courseEntities.ContentResult!);
			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> GetPendingRidesByDriverAsync(string driverId)
    {
        try
        {
            var pendingRides = await _bookingRepository.GetAllAsync(c => c.Ride.DriverId == driverId && c.BookingStatus == BookingStatus.Pending);

            if (pendingRides.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(pendingRides.ContentResult!);

            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }


    public async Task<ResponseResult> ConfirmBookingAsync(int bookingId)
    {
        try
        {
            var response = await _bookingRepository.GetOneAsync(c => c.Id == bookingId);

            if (response.StatusCode != StatusCode.Ok || response.ContentResult == null)
                return ResponseFactory.NotFound("Booking not found.");

            
            var bookingEntity = response.ContentResult as BookingEntity;

            if (bookingEntity == null)
            {
                var bookingList = response.ContentResult as IEnumerable<BookingEntity>;
                bookingEntity = bookingList?.FirstOrDefault();
            }

            if (bookingEntity == null)
                return ResponseFactory.NotFound("Booking not found.");

            bookingEntity.BookingStatus = BookingStatus.Confirmed;

            var updateResponse = await _bookingRepository.UpdateAsync(bookingEntity);
            if (updateResponse.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(bookingEntity, "Booking status updated to Confirmed.");

            return ResponseFactory.Error("Failed to update booking status.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error confirming booking: {ex.Message}");
        }
    }


    public async Task<ResponseResult> RejectBookingAsync(int bookingId)
    {
        try
        {
            var response = await _bookingRepository.GetOneAsync(c => c.Id == bookingId);

            if (response.StatusCode != StatusCode.Ok || response.ContentResult == null)
                return ResponseFactory.NotFound("Booking not found.");


            var bookingEntity = response.ContentResult as BookingEntity;

            if (bookingEntity == null)
            {
                var bookingList = response.ContentResult as IEnumerable<BookingEntity>;
                bookingEntity = bookingList?.FirstOrDefault();
            }

            if (bookingEntity == null)
                return ResponseFactory.NotFound("Booking not found.");

            bookingEntity.BookingStatus = BookingStatus.Rejected;

            var updateResponse = await _bookingRepository.UpdateAsync(bookingEntity);
            if (updateResponse.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(bookingEntity, "Booking status updated to Rejected.");

            return ResponseFactory.Error("Failed to update booking status.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error rejecting booking: {ex.Message}");
        }
    }


    public async Task<ResponseResult> GetBookingInfoByRideAndBookingIdAsync(int rideId, int bookingId)
    {
        try
        {
            var bookingResponse = await _bookingRepository.GetAllAsync(b => b.RideId == rideId && b.Id == bookingId);
            if (bookingResponse.StatusCode == StatusCode.Ok && bookingResponse.ContentResult != null)
            {
                var booking = ((List<BookingEntity>)bookingResponse.ContentResult).FirstOrDefault();
                if (booking != null)
                    return ResponseFactory.Ok(booking);
            }
            return ResponseFactory.NotFound("No pending booking found for this ride.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error fetching booking info: {ex.Message}");
        }
    }

    public async Task<ResponseResult> CancelBookingAsync(int bookingId)
    {
        try
        {
            var response = await _bookingRepository.GetAllAsync(c => c.Id == bookingId);

            if (response.StatusCode != StatusCode.Ok || response.ContentResult == null)
                return ResponseFactory.NotFound("Booking not found.");


            var bookingEntity = response.ContentResult as BookingEntity;

            if (bookingEntity == null)
            {
                var bookingList = response.ContentResult as IEnumerable<BookingEntity>;
                bookingEntity = bookingList?.FirstOrDefault();
            }

            if (bookingEntity == null)
                return ResponseFactory.NotFound("Booking not found.");

            bookingEntity.BookingStatus = BookingStatus.Cancelled;

            var updateResponse = await _bookingRepository.UpdateAsync(bookingEntity);
            if (updateResponse.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(bookingEntity, "Booking status updated to Cancelled.");

            return ResponseFactory.Error("Failed to update booking status.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error cancelling booking: {ex.Message}");
        }
    }

    //public async Task<ResponseResult> UpdateBookingAsync(int id, bookingEntity booking)
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

    public async Task<ResponseResult> DeleteBookingAsync(int id)
	{
		try
		{
			var existingcourse = await _bookingRepository.GetOneAsync(x => x.Id == id);

			if (existingcourse != null)
			{
				await _bookingRepository.RemoveAsync(c => c.Id == id);
				return ResponseFactory.Ok("Successfully removed!");
			}

			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

    public async Task<ResponseResult> CompleteBookingAsync(int bookingId)
    {
        try
        {
            var response = await _bookingRepository.GetAllAsync(b => b.Id == bookingId);

            if (response.StatusCode != StatusCode.Ok || response.ContentResult == null)
                return ResponseFactory.NotFound("Booking not found.");

            var bookingEntity = response.ContentResult as BookingEntity;

            if (bookingEntity == null)
            {
                var bookingList = response.ContentResult as IEnumerable<BookingEntity>;
                bookingEntity = bookingList?.FirstOrDefault();
            }

            if (bookingEntity == null)
                return ResponseFactory.NotFound("Booking not found.");

            bookingEntity.BookingStatus = BookingStatus.Completed;

            var updateResponse = await _bookingRepository.UpdateAsync(bookingEntity);
            if (updateResponse.StatusCode == StatusCode.Ok)
                return ResponseFactory.Ok(bookingEntity, "Booking status updated to Completed.");

            return ResponseFactory.Error("Failed to update booking status.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"Error completing booking: {ex.Message}");
        }
    }



}
