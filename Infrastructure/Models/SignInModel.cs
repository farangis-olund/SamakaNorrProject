
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class SignInModel
{
	[Display(Name = "E-postadress", Prompt = "Ange din e-postadress")]
    [DataType(DataType.EmailAddress)]
	[Required(ErrorMessage = "E-postadress krävs")]
	public string Email { get; set; } = null!;
    
    [Display(Name = "Lösenord", Prompt = "Ange ditt lösenord")]
    [DataType(DataType.Password)]
	[Required(ErrorMessage = "Lösenord krävs")]
	public string Password { get; set; } = null!;

	[Display(Name = "Kom ihåg mig")]
	public bool RememberMe { get; set; } = false;
}
