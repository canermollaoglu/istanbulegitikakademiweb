using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.dashboard;
using System.Text.RegularExpressions;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public HomeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/panel")]
        public IActionResult Index()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminHomeIndex");
            return View();
        }

        [Route("admin/create-seo-url")]
        public IActionResult CreateSeoUrlByTitle(string title)
        {
            string url = title;
            if (string.IsNullOrEmpty(url))
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = url
                });
            url = url.ToLower();
            url = url.Trim();
            if (url.Length > 128)
            {
                url = url.Substring(0, 100);
            }
            url = url.Replace("İ", "I");
            url = url.Replace("ı", "i");
            url = url.Replace("ğ", "g");
            url = url.Replace("Ğ", "G");
            url = url.Replace("ç", "c");
            url = url.Replace("Ç", "C");
            url = url.Replace("ö", "o");
            url = url.Replace("Ö", "O");
            url = url.Replace("ş", "s");
            url = url.Replace("Ş", "S");
            url = url.Replace("ü", "u");
            url = url.Replace("Ü", "U");
            url = url.Replace("'", "");
            url = url.Replace("\"", "");
            char[] replacerList = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
            for (int i = 0; i < replacerList.Length; i++)
            {
                string strChr = replacerList[i].ToString();
                if (url.Contains(strChr))
                {
                    url = url.Replace(strChr, string.Empty);
                }
            }
            Regex r = new Regex("[^a-zA-Z0-9_-]");
            url = r.Replace(url, "-");
            while (url.IndexOf("--") > -1)
                url = url.Replace("--", "-");

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = url
            });


        }


        #region Dashboard
        [Route("admin/dashboard/widget-infos")]
        public IActionResult GetDashboardWidgetData()
        {
            var retVal = new AdminDashboardWidgetsVm();
            retVal.StudentInfoWidget = _unitOfWork.Dashboard.GetStudentInfo();
            retVal.SalesInfoWidget = _unitOfWork.Dashboard.GetSalesInfo();
            retVal.EducationGroupWidget = _unitOfWork.Dashboard.GetEducationGroupInfo();
            retVal.ProfitWidget = _unitOfWork.Dashboard.GetProfitInfo();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = retVal
            });
        }

        [Route("admin/dashboard/sales-chart-data")]
        public IActionResult GetDashboardSalesChartData()
        {
            var retVal = new AdminDashboardChartDatasVm();

            retVal.SalesChartData = _unitOfWork.Dashboard.GetSalesChartData();
            retVal.GroupExpensesChartData = _unitOfWork.Dashboard.GetGroupExpenseChartData();
            retVal.EducatorExpenseChartData = _unitOfWork.Dashboard.GetEducatorExpenseChartData();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = retVal
            });


        }

        #endregion

    }
}