
namespace Infrastructure.Models;

public enum StatusCode
{
    Ok = 200,
    Error = 400,
    NotFound = 404,
    Exists = 409,
}
public class ResponseResult
{
    public StatusCode StatusCode { get; set; }
    public object? ContentResult { get; set; }
    public string? Message { get; set; }

}
