using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Route("Notifications")]
public class NotificationsController : Controller
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly NotificationService _notificationService;

    public NotificationsController(UserManager<UserEntity> userManager, NotificationService notificationService)
    {
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [HttpGet("GetUnreadCount")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var response = await _notificationService.GetUnreadCountAsync(user.Id);

        if (response.StatusCode == Infrastructure.Models.StatusCode.Ok)
            return Ok(response.ContentResult);

        return NotFound();
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var result = await _notificationService.GetAllNotificationsAsync(user.Id);

        if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
            return Json(result.ContentResult);

        return Json(new List<object>());
    }

    [HttpPost("MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);

        if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
            return Ok();

        return BadRequest();
    }
}
