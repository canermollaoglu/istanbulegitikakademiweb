using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using System.Text.RegularExpressions;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
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
    }
}