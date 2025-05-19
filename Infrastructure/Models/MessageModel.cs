
namespace Infrastructure.Models;

public class MessageModel
{
	public string Sender { get; set; } = null!;
	public string Receiver { get; set; } = null!;
	public string Text { get; set; } = null!;
	public DateTime Timestamp { get; set; }
	public bool IsRead { get; set; }
}
