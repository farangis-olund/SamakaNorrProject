
using Infrastructure.Entities;
using Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class RideModel
{
    public int Id { get; set; }

    [Display(Name = "Avresa", Prompt = "Ange avreseplats")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Avreseplats krävs")]
    public string Origin { get; set; } = null!;

    [Display(Name = "Destination", Prompt = "Ange destination plats")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Destination plats krävs")]
    public string Destination { get; set; } = null!;

    [Display(Name = "Datum", Prompt = "Ange datum för resan")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "datum krävs")]
    public DateTime DepartureTime { get; set; } 
    public bool Free {  get; set; }
    public double? Price { get; set; }
    public string? UserImgUrl { get; set; }
	public string? DriverId { get; set; } 
	public string DriverName { get; set; } = null!;
    public string? TripDetails { get; set; } = null!;
    public int AvailableSeats { get; set; }
	
    [Display(Name = "Plats")]
	public int RequiredSeats { get; set; }

	[Display(Name = "Meddelande till föraren (frivillig)", Prompt = "Ange meddelande till föraren")]
	[DataType(DataType.MultilineText)]
	public string? PassangerMessage { get; set; }

    public BookingStatus? BookingStatus { get; set; }

	public List<MessageModel>? Messages { get; set; } = [];
    public bool HasUnreadMessages { get; set; }

    public int BookingId { get; set; }

	// New fields
	public double DistanceKm { get; set; }
	public TimeSpan Duration { get; set; }
	public DateTime EstimatedArrival { get; set; }

}
