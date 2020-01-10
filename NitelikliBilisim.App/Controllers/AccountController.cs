using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Models.Account;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels.Account;
using NitelikliBilisim.Enums;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UnitOfWork _unitOfWork;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UnitOfWork unitOfWork)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [Route("kayit-ol")]
        public IActionResult Register()
        {
            var model = new RegisterGetVm
            {
                EducationCenters  = EnumSupport.ToKeyValuePair<EducationCenter>(),
                EducationCategories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, x => x.OrderBy(o => o.Name))
            };

            return View(model);
        }

        [HttpPost, Route("kayit-ol")]
        public async Task<IActionResult> Register(RegisterPostVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });
            }

            var user = new ApplicationUser()
            {
                Name = model.Name,
                UserName = model.UserName,
                Surname = model.Surname,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result == IdentityResult.Success)
            {
                result = await _userManager.AddToRoleAsync(user,
                    _userManager.Users.Count() == 1
                        ? IdentityRoleList.Admin.ToString()
                        : IdentityRoleList.User.ToString());
                var customer = new Customer
                {
                    Id = user.Id,
                    CustomerType = CustomerType.Individual,
                    IsNbuyStudent = model.IsNbuyStudent
                };
                _unitOfWork.Customer.Insert(customer);
                if (model.IsNbuyStudent)
                {
                    var studentEducationInformation = new StudentEducationInfo
                    {
                        CustomerId = user.Id,
                        StartedAt = model.StartedAt.Value,
                        EducationCenter = (EducationCenter)model.EducationCenter,
                        CategoryId = model.EducationCategory
                    };
                    _unitOfWork.StudentEducationInfo.Insert(studentEducationInformation);
                }
                if (result.Succeeded)
                {
                    //TODO mail gönder
                }
            }
            else
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Kullanıcı oluşturulurken bir hata oluştu" }
                });
            }

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("giris-yap")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            //return Redirect("/yakinda");
            if (HttpContext.User.Identity.IsAuthenticated)
                return Redirect("/");
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost, Route("giris-yap")]
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

            ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı bulunmamaktadır!");

            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            //var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var redirectUrl = Url.Action("ExternalLoginCallback", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // Store the access token and resign in so the token is included in the cookie
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var props = new AuthenticationProperties();
                props.StoreTokens(info.AuthenticationTokens);
                await _signInManager.SignInAsync(user, props, info.LoginProvider);
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Home");
                return Redirect(Url.Link(returnUrl, null));
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                //If the user does not have an account, then ask the user to create an account.
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var model = new ExternalLoginViewModel()
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        ReturnUrl = returnUrl,
                        LoginProvider = info.LoginProvider,
                    };
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                    {
                        model.UserName =
                            StringHelper.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.GivenName));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        model.Name =
                            StringHelper.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.Name));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                    {
                        model.Surname =
                            StringHelper.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.Surname));
                    }
                    if (info.Principal.HasClaim(c => c.Type == "photo"))
                    {
                        model.Photo = info.Principal.FindFirstValue("photo");
                    }
                    return View("ExternalLogin", model);
                }

            }
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ExternalLoginConfirm(ExternalLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ExternalLogin", model);
            }
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                AvatarPath = model.Photo,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, IdentityRoleList.User.ToString());
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    // Copy over the gender claim as well
                    result = await _userManager.AddClaimAsync(user, info.Principal.FindFirst(ClaimTypes.Email));

                    // Include the access token in the properties
                    var props = new AuthenticationProperties();
                    props.StoreTokens(info.AuthenticationTokens);

                    await _signInManager.SignInAsync(user, props, authenticationMethod: info.LoginProvider);
                    if (string.IsNullOrEmpty(model.ReturnUrl))
                        return RedirectToAction("Index", "Home");
                    return Redirect(Url.Link(model.ReturnUrl, null));
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(nameof(ExternalLogin), model);
        }
        [Authorize, Route("cikis-yap")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}