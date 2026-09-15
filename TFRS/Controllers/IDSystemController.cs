using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class IDSystemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
