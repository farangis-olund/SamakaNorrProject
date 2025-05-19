
namespace Infrastructure.Models;

public class AccountProfileModel
{
	public string? ProfileImgUrl { get; set; }
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string UserName { get; set; } = null!;
	public bool IsExternalAccount { get; set; }
}
