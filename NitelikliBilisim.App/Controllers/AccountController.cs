using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Core.Entities.Identity;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Account;

namespace NitelikliBilisim.App.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser()
            {
                Name = model.Name,
                UserName = model.UserName,
                Surname = model.Surname,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result == IdentityResult.Success)
            {
                result = await _userManager.AddToRoleAsync(user,
                    _userManager.Users.Count() == 1
                        ? IdentityRoleList.Admin.ToString()
                        : IdentityRoleList.User.ToString());
                if (result.Succeeded)
                {
                    //TODO mail gönder, 
                    return RedirectToAction("Index", "Home");
                }

                var messages = string.Join(", ", result.Errors.Select(x => x.Description));
                ModelState.AddModelError(string.Empty, messages);
            }
            else
            {
                var messages = string.Join(", ", result.Errors.Select(x => x.Description));
                ModelState.AddModelError(string.Empty, messages);
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
               return RedirectToAction("Index", "Home");
            }
            
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}