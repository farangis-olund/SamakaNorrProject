using Infrastructure.Contexts;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApp.Controllers
{
	public class MessageController : Controller
	{
		private readonly DataContext _context;

		public MessageController(DataContext context)
		{
			_context = context;
		}
		[HttpPost]
		public async Task<IActionResult> MarkAsRead([FromBody] int rideId)
		{
			var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

			var unreadMessages = await _context.Messages
				.Where(m => m.RideId == rideId && m.ReceiverId == currentUserId && m.IsRead==false)
				.ToListAsync();

			foreach (var msg in unreadMessages)
			{
				msg.IsRead = true;
				
			}

			await _context.SaveChangesAsync();
			return Ok();
		}

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(int rideId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var count = await _context.Messages
                .Where(m => m.RideId == rideId && m.ReceiverId == currentUserId && m.IsRead==false)
                .CountAsync();

            return Ok(count);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCountForPassenger(int rideId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var count = await _context.Messages
                .Where(m => m.RideId == rideId && m.IsRead == false)
                .CountAsync();

            return Ok(count);
        }


        public IActionResult RenderMessage(MessageModel message, string currentUser)
        {
            var cssClass = message.Sender == currentUser ? "sent" : "received";
            return PartialView("~/Views/Shared/Sections/_SingleMessagePartial.cshtml",
                new MessageModel
                {
                    CssClass = cssClass,
                    Sender = message.Sender,
                    Text = message.Text,
                    Timestamp = message.Timestamp
                });
        }

    }
}
