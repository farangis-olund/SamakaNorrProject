
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class AccountDetailAddressInfoModel
{
	[Display(Name = "Adressrad 1", Prompt = "Ange din adress", Order = 0)]
	[Required(ErrorMessage = "Adressrad 1 är obligatorisk")] 
	[DataType(DataType.Text)]
	public string Addressline_1 { get; set; } = null!;

	[Display(Name = "Adressrad 2", Prompt = "Ange din andra adressrad", Order = 1)]
	[DataType(DataType.Text)]
	public string? Addressline_2 { get; set; }

	[Display(Name = "Postnummer", Prompt = "Ange ditt postnummer", Order = 2)]
	[Required(ErrorMessage = "Postnummer är obligatoriskt")]
	[DataType(DataType.PostalCode)]
	public string PostalCode { get; set; } = null!;

	[Display(Name = "Stad", Prompt = "Ange din stad", Order = 3)]
	[Required(ErrorMessage = "Stad är obligatorisk")]
	public string City { get; set; } = null!;

}
