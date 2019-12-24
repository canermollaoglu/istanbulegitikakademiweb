using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult SearchResults(string searchText)
        {
            return View();
        }
        public IActionResult SearchEducation(string searchText)
        {
            return Json("");
        }
    }
}