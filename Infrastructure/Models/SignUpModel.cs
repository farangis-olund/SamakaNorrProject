
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Models;

public class SignUpModel
{
    [Display(Name = "Förnamn", Prompt = "Ange ditt förnamn", Order = 0)]
    [Required(ErrorMessage = "Förnamn är obligatoriskt")]
    [MinLength(2, ErrorMessage = "Namnet måste innehålla minst 2 tecken")]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Efternamn", Prompt = "Ange ditt efternamn", Order = 1)]
    [Required(ErrorMessage = "Efternamn är obligatoriskt")]
    [MinLength(2, ErrorMessage = "Namnet måste innehålla minst 2 tecken")]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [Display(Name = "E-postadress", Prompt = "Ange din e-postadress", Order = 2)]
    [Required(ErrorMessage = "E-postadress är obligatorisk")]
    [EmailAddress]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Ogiltig e-postadress")]
    public string Email { get; set; } = null!;

    [Display(Name = "Lösenord", Prompt = "Ange ditt lösenord", Order = 3)]
    [Required(ErrorMessage = "Lösenord är obligatoriskt")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=])[A-Za-z\d!@#$%^&*()_\-+=]{8,}$", ErrorMessage = "Ogiltigt lösenord")]
    public string Password { get; set; } = null!;

    [Display(Name = "Bekräfta lösenord", Prompt = "Bekräfta ditt lösenord", Order = 4)]
    [Required(ErrorMessage = "Du måste bekräfta ditt lösenord")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Lösenorden stämmer inte överens")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Jag godkänner villkoren", Order = 5)]
    [CheckBoxRequired(ErrorMessage = "Du måste godkänna villkoren")]
    public bool TermsAndCondition { get; set; } = false;
}
