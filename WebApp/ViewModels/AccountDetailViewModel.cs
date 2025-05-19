using Infrastructure.Models;

namespace WebApp.ViewModels;

public class AccountDetailViewModel
{
	public string Title { get; set; } = "Kontouppgifter";
	public AccountProfileModel ProfileInfo { get; set; } = null!;
	public AccountDetailBasicInfoModel BasicInfo { get; set; } = null!;
	public AccountDetailAddressInfoModel AddressInfo { get; set; } = null!;
	public AccountSecurityModel SecurityInfo { get; set; } = null!;
	public RidesViewModel SavedRides { get; set; } = null!;
    public List<BookedRideModel> PassengerRides { get; set; } = [];

}
