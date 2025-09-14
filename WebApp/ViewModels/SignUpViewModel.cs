using Infrastructure.Models;

namespace WebApp.ViewModels
{
	public class SignUpViewModel
	{
		public string Title { get; set; } = "Registrera";
		public SignUpModel Form { get; set; } = new SignUpModel();
	}
}
