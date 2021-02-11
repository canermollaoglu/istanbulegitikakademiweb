using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class HelpDocumentsController : BaseController
    {
        public IActionResult Index(string documentName)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminHelpDocuments");
            var url = "/help-doc/"+documentName+".pdf";
            ViewData["DocumentUrl"] = url;
            return View();
        }
    }
}
