using System;
using Microsoft.AspNetCore.Mvc;
using TFRS.Models;

namespace TFRS.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly TFRS.Services.IMainTableService _mainTableService;

        public ApplicationController(TFRS.Services.IMainTableService mainTableService)
        {
            _mainTableService = mainTableService;
        }
        public IActionResult Create()
        {
            var model = new MainTableRecord
            {
                DateSubmitted = DateTime.Now.ToString("yyyy-MM-dd")
            };

            return View(model);
        }

        [HttpGet]
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

            return View("~/Views/Home/Applicationpage.cshtml");
        }

        [HttpPost]
        public IActionResult Save(MainTableRecord model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mainTableService.Add(model);
                    return RedirectToAction("Index", "Records");
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to save record.");
                }
            }

            return View("~/Views/Home/Applicationpage.cshtml", model);
        }
    }
}

