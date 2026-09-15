using Microsoft.AspNetCore.Mvc;

namespace TFRS.Controllers
{
    public class RecordsController : Controller
    {
        private readonly TFRS.Services.IMainTableService _mainTableService;

        public RecordsController(TFRS.Services.IMainTableService mainTableService)
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
            catch (Exception ex)
            {
                // log or handle
                ViewBag.MainRecords = new List<TFRS.Models.MainTableRecord>();
            }

            return View("~/Views/Home/TFrecords.cshtml");
        }

        [HttpGet]
        public IActionResult GetByFranchise(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var svc = HttpContext.RequestServices.GetService(typeof(TFRS.Services.IMainTableService)) as TFRS.Services.IMainTableService;
            if (svc == null) return StatusCode(500);

            var r = svc.GetByFranchiseNumber(id);
            if (r == null) return NotFound();

            return Json(r);
        }


    }
}
