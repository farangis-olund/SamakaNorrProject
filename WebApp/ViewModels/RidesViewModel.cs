using Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class RidesViewModel
{
	public string Title { get; set; } = "Resor";
    public string SearchQuery { get; set; } = null!;
    
    [Display(Name = "Avresa", Prompt = "Ange avresa")]
    public string OriginQuery { get; set; } = null!;
    
    [Display(Name = "Destination", Prompt = "Ange destination")]
    public string DestinationQuery { get; set; } = null!;
    
    [Display(Name = "Datum", Prompt = "Ange datum för resan")]
	[DataType(DataType.Date)]
	public DateTime DepartureDateQuery { get; set; } = DateTime.Now.Date;
    public int SelectedRideId { get; set; }
	public int SelectedrideId { get; set; }
	public List<RideModel> Rides { get; set; } = [];
    public RideSearchModel? SearchModel { get; set; } = new RideSearchModel();

}

