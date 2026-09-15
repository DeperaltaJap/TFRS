using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
