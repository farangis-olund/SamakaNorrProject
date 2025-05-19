
using Infrastructure.Models;

namespace Infrastructure.Factories;

public class ResponseFactory
{
    public static ResponseResult Ok()
    {
        return new ResponseResult
        {
            Message = "Succeded!",
            StatusCode = StatusCode.Ok
        };
    }

    public static ResponseResult Ok(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Succeded!",
            StatusCode = StatusCode.Ok
        };
    }

    public static ResponseResult Ok(object obj, string? message=null)
    {
        return new ResponseResult
        {
            ContentResult = obj,
            Message = message ?? "Succeded!",
            StatusCode = StatusCode.Ok
        };
    }

    public static ResponseResult Error(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Failed!",
            StatusCode = StatusCode.Error
        };
    }

    public static ResponseResult NotFound(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Not found!",
            StatusCode = StatusCode.NotFound
        };
    }

    public static ResponseResult Exists(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Already exists!",
            StatusCode = StatusCode.Exists
        };
    }
}
