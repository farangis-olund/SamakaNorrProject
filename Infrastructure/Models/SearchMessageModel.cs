
namespace Infrastructure.Models;

public class SearchMessageModel
{
    public string Sender { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}
