using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
