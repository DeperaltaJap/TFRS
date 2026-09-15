using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
