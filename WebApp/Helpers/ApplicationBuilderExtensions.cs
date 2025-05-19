
namespace WebApp.Helpers;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseUserSessionValidation(this IApplicationBuilder buidler)
    {
        return buidler.UseMiddleware<UserSessionValidationMiddleware>();
    }
}
