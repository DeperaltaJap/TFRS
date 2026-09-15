using Microsoft.AspNetCore.Mvc;

namespace TFRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly TFRS.Services.IMainTableService _mainTableService;

        public HomeController(TFRS.Services.IMainTableService mainTableService)
        {
            _mainTableService = mainTableService;
        }

        public IActionResult Index()
        {
            try
            {
                var records = _mainTableService.GetAll();
                ViewBag.MainRecords = records;
            }
            catch
            {
                ViewBag.MainRecords = new List<TFRS.Models.MainTableRecord>();
            }

            return View();
        }
    }
}