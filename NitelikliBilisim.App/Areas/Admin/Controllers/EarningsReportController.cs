using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EarningsReportController : Controller
    {
        [Route("raporlar/satis")]
        public IActionResult IncomeReport()
        {
            return View();
        }

        public IActionResult FetchIncomeReport(int year, int month)
        {
            
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}