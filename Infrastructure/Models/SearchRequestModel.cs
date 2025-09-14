using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class SearchRequestModel : RideSearchModel
{
    [Display(Name = "Platser", Prompt = "Antal platser som behövs")]
    [Range(1, 10, ErrorMessage = "Ange minst 1 plats")]
    public int SeatsRequired { get; set; }

    [Display(Name = "Meddelande", Prompt = "Lägg till extra information")]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }

    // For displaying request results
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? UserImgUrl { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime EstimatedArrival { get; set; }

    public List<SearchMessageModel>? Messages { get; set; } = new();
}

public class SearchViewModel
{
    public RideSearchModel? SearchModel { get; set; } = new();
    public SearchRequestModel? NewRequest { get; set; } = new();
    public List<SearchRequestModel> Requests { get; set; } = new();
}