using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HomeController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (await _roleManager.RoleExistsAsync("User") == false)
                await _roleManager.CreateAsync(new ApplicationRole() { Name = "User" });

            if (!_userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    Name = "Admin",
                    Surname = "Test",
                    Email = "test@email.com",
                    UserName = "admin"
                };
                var result = await _userManager.CreateAsync(user,"P@ssword1");
                if (result == IdentityResult.Success)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

           

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
    }
}
