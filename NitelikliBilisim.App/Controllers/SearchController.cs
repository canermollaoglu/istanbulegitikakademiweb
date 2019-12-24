using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult MainSearch(string searchText)
        {
            return Json("");
        }
    }
}