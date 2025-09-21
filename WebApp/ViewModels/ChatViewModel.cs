using Infrastructure.Models;

namespace WebApp.ViewModels;

public class ChatViewModel
{
    public int Id { get; set; } // RequestId or RideId
    public List<MessageModel> Messages { get; set; } = new();
    public string? IsCurrentUser { get; set; }
}
