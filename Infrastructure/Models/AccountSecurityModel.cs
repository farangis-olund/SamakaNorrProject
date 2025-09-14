
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class AccountSecurityModel
{

	[Display(Name = "Nuvarande lösenord", Prompt = "Ange ditt nuvarande lösenord", Order = 0)]
	[Required(ErrorMessage = "Nuvarande lösenord är obligatoriskt")]
	[DataType(DataType.Password)]
	public string CurrentPassword { get; set; } = null!;

	[Display(Name = "Nytt lösenord", Prompt = "Ange ditt nya lösenord", Order = 1)]
	[Required(ErrorMessage = "Nytt lösenord är obligatoriskt")]
	[DataType(DataType.Password)]
	[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=])[A-Za-z\d!@#$%^&*()_\-+=]{8,}$", ErrorMessage = "Ogiltigt lösenord")]
	public string Password { get; set; } = null!;

	[Display(Name = "Bekräfta nytt lösenord", Prompt = "Bekräfta nytt lösenord", Order = 2)]
	[Required(ErrorMessage = "Bekräftelse av nytt lösenord är obligatorisk")]
	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "Lösenorden matchar inte")]
	public string ConfirmPassword { get; set; } = null!;

	[Display(Name = "Ja, jag vill radera mitt konto.", Order = 3)]
	public bool DeleteAccount { get; set; } = false;


}
