using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class TempSecurityController : Controller
    {

    }
}