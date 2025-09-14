
namespace Infrastructure.Entities;

public class SearchRequestEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public int SeatsRequired { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<SearchMessageEntity> Messages { get; set; } = [];
}
