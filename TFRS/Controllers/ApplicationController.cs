using System;
using System.Collections.Generic;
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
            return View("~/Views/Home/Applicationpage.cshtml", model);
        }

        [HttpGet]
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
            return View("~/Views/Home/Applicationpage.cshtml");
        }

        [HttpGet]
        public IActionResult Edit(string franchiseNumber)
        {
            if (string.IsNullOrEmpty(franchiseNumber)) return BadRequest();
            var record = _mainTableService.GetByFranchiseNumber(franchiseNumber);
            if (record == null) return NotFound();
            ViewBag.IsEditMode = true;
            return View("~/Views/Home/Applicationpage.cshtml", record);
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
