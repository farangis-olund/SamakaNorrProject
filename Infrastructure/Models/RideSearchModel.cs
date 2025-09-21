using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Models;
public class RideSearchModel
{
    [Display(Name = "Från", Prompt = "Ange avreseplats")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Avreseplats krävs")]
    public string Origin { get; set; } = null!;

    [Display(Name = "Till", Prompt = "Ange destination plats")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Destination plats krävs")]
    public string Destination { get; set; } = null!;

    [Display(Name = "Datum", Prompt = "Ange datum för resan")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "datum krävs")]
    public DateTime? DepartureTime { get; set; }
}
