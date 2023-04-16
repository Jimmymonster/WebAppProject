using Microsoft.AspNetCore.Mvc;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Worker()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
    }
}
