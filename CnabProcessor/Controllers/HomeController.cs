using Microsoft.AspNetCore.Mvc;

namespace CnabProcessor.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
