using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class AccountController(UserManager<UserEntity> userManager, 
							   SignInManager<UserEntity> signInManager, 
							   AccountService accountService, RideService rideService) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
	private readonly SignInManager<UserEntity> _signInManager = signInManager;
	private readonly AccountService _accountService = accountService;
    private readonly RideService _rideService = rideService;

    #region Index
    [HttpGet]
    [Route("/account")]
    public async Task<IActionResult> Index()
    {
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
			return NotFound();

		var viewModel = await _accountService.GetAccountDetailsAsync(user);
		return View(viewModel);

	}
	
	[HttpPost]
	[Route("/account")]
	public async Task<IActionResult> Index(AccountDetailViewModel viewModel)
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
			return NotFound();

		var success = await _accountService.UpdateAccountAsync(user, viewModel);
		if (!success)
		{
			ViewData["StatusMessage"] = "danger|Failed to update account details!";
			return View(viewModel);
		}

		return RedirectToAction("Index");

	}
	#endregion

	#region Security
	[HttpGet]
	[Route("/account/security")]
	public async Task<IActionResult> AccountSecurity()
	{
		var user = await _userManager.GetUserAsync(User);
		var viewModel = await _accountService.GetAccountDetailsAsync(user!);
		return View(viewModel);
	}

	[HttpPost]
	[Route("/account/security")]
	public async Task<IActionResult> AccountSecurity(AccountDetailViewModel viewModel)
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null)
		{
			if (viewModel.SecurityInfo != null)
			{
				if (viewModel.SecurityInfo.CurrentPassword != null && viewModel.SecurityInfo.Password != null && (viewModel.SecurityInfo.ConfirmPassword != null && viewModel.SecurityInfo.Password  == viewModel.SecurityInfo.ConfirmPassword))
				{
					var result = await _userManager.ChangePasswordAsync(user, viewModel.SecurityInfo.CurrentPassword, viewModel.SecurityInfo.Password);

					if (result.Succeeded)
						ViewData["ErrorMessage"] = " success|Password is saved successfully!";
					else
						ViewData["ErrorMessage"] = " danger|Password saving process failed!";
				}
				else
				{
					ViewData["StatusMessage"] = "danger|Validation is failed, you should enter correct password!";
				}

				if (viewModel.SecurityInfo.DeleteAccount == true)
				{
					var result = await _userManager.DeleteAsync(user);
					if (result.Succeeded)
						ViewData["SatusMessage"] = "success|User is successfully deleted!";
					else
						ViewData["StatusMessage"] = "danger|Deleting process is failed!";
					return RedirectToAction("SignIn", "Auth");
				}

				await _signInManager.RefreshSignInAsync(user);
			}
			viewModel = await _accountService.GetAccountDetailsAsync(user);
		}

		ViewData["Title"] = viewModel.Title;
		return View(viewModel);
	}
	#endregion

	#region Driver
	[HttpGet]
	[Route("/account/driver")]
	public async Task<IActionResult> Driver()
	{
		var user = await _userManager.GetUserAsync(User);
		var viewModel = await _accountService.GetAccountDetailsAsync(user!);
		
		return View(viewModel);
	}

    [HttpPost]
    public async Task<IActionResult> AddRide(RideRegistrationModel model)
    {
        
        if (ModelState.IsValid)
        {
            var driverEmail = User.Identity!.Name;
            if(driverEmail != null)
			{
               var newRide = new RideRegistrationModel
                {
                    Origin = model.Origin,
					Destination = model.Destination,
                    DepartureTime = model.DepartureTime,
                    AvailableSeats = model.AvailableSeats,
                    Free = model.Free,
                    Price = model.Price,
                    TripDetails = model.TripDetails,
                    DriverEmail = driverEmail
                };
                var result = await _rideService.AddRideAsync(newRide);
				if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
				{
                    TempData["StatusMessage"] = "success|Resan har lagts till!";
                    return RedirectToAction("Driver", "Account");
                }
				else
				{
                    TempData["StatusMessage"] = "error|Något gick fel. Försök igen.";
                    return View(model);
                }
                
            }
            TempData["StatusMessage"] = "error|Något gick fel. Försök igen.";
            return View(model);

        }

        TempData["StatusMessage"] = "error|Något gick fel. Försök igen.";
        return View(model);
    }
	#endregion

	#region Passenger
	[HttpGet]
	[Route("/account/passenger")]
	public async Task<IActionResult> Passenger()
	{
		var user = await _userManager.GetUserAsync(User);
		var viewModel = await _accountService.GetAccountDetailsAsync(user!);

		return View(viewModel);
	}
	#endregion

	[HttpPost]
	[Route("/account/upload")]
	public async Task<IActionResult> UploadProfileImage(IFormFile file)
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null && file != null && file.Length != 0)
		{
			var fileName = $"p_{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/profiles", fileName);

			using var fs = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(fs);

			user.ProfileImgUrl = fileName;
			await _userManager.UpdateAsync(user);
		}
		else
		{
			TempData["StatusMessage"] = "Unable to upload profile image!";
		}

		return RedirectToAction("Index", "Account");
	}
}
