using Microsoft.AspNetCore.Mvc;

namespace PharmaProject.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
