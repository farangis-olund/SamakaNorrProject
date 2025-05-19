using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class RideEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string DriverId { get; set; } = null!;

    [Required]
    public string Origin { get; set; } = null!;

    [Required]
    public string Destination { get; set; } = null!;

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public int AvailableSeats { get; set; } 
    public bool Free { get; set; } = false;
    public double? Price { get; set; }

    public string? TripDetails { get; set; }

    // Navigation property for related entities
    public ICollection<BookingEntity> Bookings { get; set; } = [];
	public ICollection<MessageEntity> Messages { get; set; } = [];

}
