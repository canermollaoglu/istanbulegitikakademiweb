using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.EducatorArea.Controllers
{
    [Area("EducatorArea"), Authorize("Educator")]
    public class ManagementController : Controller
    {
        [Route("egitmen/yonetim-paneli")]
        public IActionResult Index()
        {
            return View();
        }
    }
}