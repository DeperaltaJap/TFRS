using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class FranchiseController : Controller
    {
        private readonly TFRS.Services.IMainTableService _mainTableService;

        public FranchiseController(TFRS.Services.IMainTableService mainTableService)
        {
            _mainTableService = mainTableService;
        }

        public IActionResult Index() => Certificate();

        public IActionResult Certificate()
        {
            try
            {
                ViewBag.MainRecords = _mainTableService.GetAll();
            }
            catch
            {
                ViewBag.MainRecords = new List<TFRS.Models.MainTableRecord>();
            }

            return View("~/Views/Home/certificateRegistration.cshtml");
        }
    }
}
