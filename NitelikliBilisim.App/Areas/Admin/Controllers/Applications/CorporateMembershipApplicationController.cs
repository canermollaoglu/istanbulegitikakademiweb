using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Applications
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class CorporateMembershipApplicationController : Controller
    {
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminCorporateMembershipApplicationList");
            return View();
        }
    }
}
