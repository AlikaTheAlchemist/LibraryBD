using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Properties
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
