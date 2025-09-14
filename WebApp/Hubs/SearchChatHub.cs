using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace WebApp.Hubs;

public class SearchChatHub : Hub
{
    private readonly DataContext _context;

    public SearchChatHub(DataContext context)
    {
        _context = context;
    }

    public async Task SendMessage(string requestId, string sender, string message)
    {
        try
        {
            // ✅ Lookup sender by email
            var senderUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == sender);

            if (senderUser == null)
            {
                await Clients.Group(requestId).SendAsync("ReceiveMessage", "Unknown User", message);
                return;
            }

            string senderFullName = $"{senderUser.FirstName} {senderUser.LastName}";

            // ✅ Validate requestId only once
            if (!int.TryParse(requestId, out var id))
            {
                throw new Exception($"Invalid requestId (not an int): {requestId}");
            }

            var searchRequestExists = await _context.SearchRequests.AnyAsync(r => r.Id == id);
            if (!searchRequestExists)
            {
                throw new Exception($"No SearchRequest found with Id: {id}");
            }

            // ✅ Create and save message
            var messageEntity = new SearchMessageEntity
            {
                SearchRequestId = id,
                MessageContent = message,
                Timestamp = DateTime.Now,
                SenderId = senderUser.Email,
               
            };

            _context.SearchMessages.Add(messageEntity);
            await _context.SaveChangesAsync();

            // ✅ Broadcast to group
            await Clients.Group(requestId).SendAsync("ReceiveMessage", senderFullName, message);

            Console.WriteLine($"✅ Message sent: [{id}] {senderFullName}: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in SendMessage: {ex}");
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "An error occurred while sending your message.");
        }
    }

    public override async Task OnConnectedAsync()
    {
        var requestId = Context.GetHttpContext()?.Request.Query["requestId"];
        if (!string.IsNullOrEmpty(requestId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
        }

        await base.OnConnectedAsync();
    }
}
