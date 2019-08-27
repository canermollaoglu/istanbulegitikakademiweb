using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Core.Entities.Identity;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.App.Controllers
{
    public class HomeController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HomeController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
            CheckRoles().Wait();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        async Task CheckRoles()
        {
            var roles = Enum.GetNames(typeof(IdentityRoleList));
            foreach (var item in roles)
            {
                if (await _roleManager.RoleExistsAsync(item) == false)
                {
                    var result = await _roleManager.CreateAsync(new ApplicationRole()
                    {
                        Name = item
                    });
                }
            }
        }
    }
}