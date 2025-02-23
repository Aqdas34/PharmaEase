using Microsoft.AspNetCore.Mvc;

namespace PharmaProject.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
