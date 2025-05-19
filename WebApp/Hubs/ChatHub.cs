using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.SignalR;


namespace WebApp.Hubs;

public class ChatHub : Hub
{
	private readonly DataContext _context;

	public ChatHub(DataContext context)
	{
		_context = context;
	}
	public async Task SendMessage(string rideId, string sender, string receiver, string message)
    {
		
		var messageEntity = new MessageEntity
		{
			RideId = int.Parse(rideId),
			MessageContent = message,
			Timestamp = DateTime.Now,
			SenderId = sender,
			ReceiverId = receiver
		};

        _context.Messages.Add(messageEntity);
        await _context.SaveChangesAsync();

        await Clients.Group(rideId).SendAsync("ReceiveMessage", sender, message);
    }

    public override async Task OnConnectedAsync()
    {
        var rideId = Context.GetHttpContext()?.Request.Query["rideId"];
        if (!string.IsNullOrEmpty(rideId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, rideId);
        }

        await base.OnConnectedAsync();
    }
}
