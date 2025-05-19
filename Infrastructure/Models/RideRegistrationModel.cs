
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class RideRegistrationModel 
{
   
    public string? DriverEmail { get; set; } 

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

}
