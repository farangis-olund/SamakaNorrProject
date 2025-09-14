
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class AccountDetailBasicInfoModel
{
	public string UserId { get; set; } = null!;

	[DataType(DataType.ImageUrl)]
	public string? ProfileImgUrl { get; set; }

	[Display(Name = "Förnamn", Prompt = "Ange ditt förnamn", Order = 0)]
	[Required(ErrorMessage = "Förnamn är obligatoriskt")]
	[DataType(DataType.Text)]
	public string FirstName { get; set; } = null!;

	[Display(Name = "Efternamn", Prompt = "Ange ditt efternamn", Order = 1)]
	[Required(ErrorMessage = "Efternamn är obligatoriskt")]
	[DataType(DataType.Text)]
	public string LastName { get; set; } = null!;

	[Display(Name = "E-postadress", Prompt = "Ange din e-postadress", Order = 2)]
	[Required(ErrorMessage = "E-postadress är obligatorisk")]
	[EmailAddress]
	[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Ogiltig e-postadress")]
	public string Email { get; set; } = null!;

	[Display(Name = "Telefon", Prompt = "Ange ditt telefonnummer", Order = 3)]
	[Required(ErrorMessage = "Telefonnummer är obligatoriskt")]
	[DataType(DataType.PhoneNumber)]
	public string Phone { get; set; } = null!;

	[Display(Name = "Biografi", Prompt = "Lägg till en kort beskrivning...", Order = 4)]
	[DataType(DataType.MultilineText)]
	public string? Biography { get; set; }


}
