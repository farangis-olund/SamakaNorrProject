
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class AccountSecurityModel
{

	[Display(Name = "Current Password", Prompt = "Enter your current password", Order = 0)]
	[Required(ErrorMessage = "Current password is required")]
	[DataType(DataType.Password)]
	public string CurrentPassword { get; set; } = null!;


	[Display(Name = "New Password", Prompt = "Enter your new password", Order = 1)]
	[Required(ErrorMessage = "New password is required")]
	[DataType(DataType.Password)]
	[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=])[A-Za-z\d!@#$%^&*()_\-+=]{8,}$", ErrorMessage = "Invalid password")]
	public string Password { get; set; } = null!;


	[Display(Name = "Confirm new password", Prompt = "Confirm new password", Order = 2)]
	[Required(ErrorMessage = "Confirm new password")]
	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "Password does not match")]
	public string ConfirmPassword { get; set; } = null!;


	[Display(Name = "Yes, I want to delete my account.", Order = 3)]
	public bool DeleteAccount { get; set; } = false;

}
