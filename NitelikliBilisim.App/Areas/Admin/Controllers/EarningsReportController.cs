using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using System;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EarningsReportController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EarningsReportController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("raporlar/satis")]
        public IActionResult IncomeReport()
        {
            return View();
        }

        [Route("fetch-income-report/{year?}/{month?}")]
        public IActionResult FetchIncomeReport(int? year, int? month)
        {
            if (year == null || month == null)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
            var model = _unitOfWork.Report.FetchIncomeReport(year.Value, month.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("raporlar/grup-bazli-satis-raporu")]
        public IActionResult GroupBasedSalesReport(Guid groupId)
        {
            var model = new GroupBasedSalesReportGetVm
            {
                AllGroups = _unitOfWork.EducationGroup.GetAllGroupsDictionary()
            };
            return View(model);
        }

    }
}