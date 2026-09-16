using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TFRS.Models;

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
                ViewBag.MainRecords = _mainTableService.GetAll();
            }
            catch
            {
                ViewBag.MainRecords = new List<MainTableRecord>();
            }
            return View("~/Views/Home/TFrecords.cshtml");
        }

        [HttpGet]
        public IActionResult GetByFranchise(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var r = _mainTableService.GetByFranchiseNumber(id);
            if (r == null) return NotFound();
            return Json(r);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var record = _mainTableService.GetByFranchiseNumber(id);
            if (record == null) return NotFound();
            return View("~/Views/Home/Applicationpage.cshtml", record);
        }

        [HttpPost]
        public IActionResult Edit(MainTableRecord model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mainTableService.Update(model);
                    return RedirectToAction("Index", "Records");
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to update record.");
                }
            }
            return View("~/Views/Home/Applicationpage.cshtml", model);
        }

        [HttpPost]
        public IActionResult Delete(string franchiseNumber)
        {
            if (string.IsNullOrEmpty(franchiseNumber)) return BadRequest();
            try
            {
                _mainTableService.Delete(franchiseNumber);
                return RedirectToAction("Index", "Records");
            }
            catch
            {
                ModelState.AddModelError("", "Unable to delete record.");
                return RedirectToAction("Index", "Records");
            }
        }
    }
}
