using Infrastructure.Models;

namespace WebApp.ViewModels;

public class SignInViewModel
{
	public string Title { get; set; } = "Logga in";
	public SignInModel Form { get; set; } = new SignInModel();
	public string? ErrorMessage { get; set; }
}
