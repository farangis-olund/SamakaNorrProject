
using Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class BookingEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RideId { get; set; }

    [Required]
    public string PassengerId { get; set; } = null!;

    [Required]
    public DateTime BookingTime { get; set; }

    [Required]
    public int NumberOfSeatsBooked { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(20)")]

    public BookingStatus BookingStatus { get; set; }

    public string? BookingDetails { get; set; }

    // Navigation property for related entities
    public UserEntity Passenger { get; set; } = null!;
    public RideEntity Ride { get; set; } = null!;
}
