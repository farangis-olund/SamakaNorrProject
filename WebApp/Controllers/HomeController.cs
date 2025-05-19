using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Förenkla din resa med Samåka Norr!";
        return View();
    }

    [Route("/error")]
    public IActionResult Error404(int statusCode) => View();
}
