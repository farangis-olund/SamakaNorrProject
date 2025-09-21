
namespace Infrastructure.Models;

public class PublicProfileModel
{
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImgUrl { get; set; }
    public string? ReturnUrl { get; set; }
}
