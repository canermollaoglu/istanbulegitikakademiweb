using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUsefullMethods;
using Nest;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Models.Account;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.helper;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels.Account;
using NitelikliBilisim.Support.Enums;

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
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Register()
        {
            var model = new RegisterGetVm
            {
                EducationCenters = EnumSupport.ToKeyValuePair<EducationCenter>(),
                EducationCategories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, x => x.OrderBy(o => o.Name))
            };

            return View(model);
        }

        [HttpPost, Route("kayit-ol")]
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public async Task<IActionResult> Register(RegisterPostVm model)
        {
            if (!model.AcceptedTerms || !ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

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
                        StartedAt = model.StartedAt.GetValueOrDefault(),
                        EducationCenter = (EducationCenter)model.EducationCenter,
                        CategoryId = model.EducationCategory.Value
                    };
                    var studentEducationInfoId = _unitOfWork.StudentEducationInfo.Insert(studentEducationInformation);
                    if (model.EducationCategory.HasValue && model.StartedAt.HasValue)
                        CreateEducationDays(studentEducationInfoId, model.EducationCategory.Value, model.StartedAt.Value);
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
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("giris-yap")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            //return Redirect("/yakinda");
            if (HttpContext.User.Identity.IsAuthenticated)
                return Redirect("/");
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
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
                return Redirect(model.ReturnUrl ?? "/");
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
                            StringHelpers.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.GivenName));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        model.Name =
                            StringHelpers.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.Name));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                    {
                        model.Surname =
                            StringHelpers.UrlFormatConverter(info.Principal.FindFirstValue(ClaimTypes.Surname));
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
        [Route("cikis-yap")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #region Helper Functions
        /// <summary>
        /// Girilen parametrelere göre NBUY eğitimi alan öğrencinin eğitim günlerini db ye ekler.
        /// </summary>
        /// <param name="studentEducationInfoId">Öğrencinin aldığı NBUY eğitiminin tutulduğu tablo.</param>
        /// <param name="categoryId">NBUY eğitimi alan öğrencinin aldığı eğitim kategorisi.</param>
        /// <param name="startDate">NBUY eğitim başlangıç tarihi.</param>
        private void CreateEducationDays(Guid studentEducationInfoId, Guid categoryId, DateTime startDate)
        {
            //Tatil Günleri
            var offDays = _unitOfWork.OffDay.Get(x => x.Year == DateTime.Now.Year || x.Year == DateTime.Now.Year - 1 || x.Year == DateTime.Now.Year + 1).ToList();
            //Kullanıcının aldığı Nbuy Eğitimi ve eğitimin süresi
            var nbuyCategory = _unitOfWork.EducationCategory.GetById(categoryId);
            var educationDayCount = nbuyCategory.EducationDayCount.HasValue ? nbuyCategory.EducationDayCount.Value : 0;
            //Kullanıcının eğitime başlangıç tarihi 
            var activeDate = startDate;
            for (int i = 0; i < educationDayCount; i++)
            {
                activeDate = activeDate.AddDays(1);
                if (checkWeekdays(activeDate) && checkNotHoliday(activeDate, offDays))
                {
                    _unitOfWork.EducationDay.Insert(new EducationDay
                    {
                        Date = activeDate,
                        Day = i + 1,
                        StudentEducationInfoId = studentEducationInfoId
                    }, true);
                }
                else
                {
                    i--;
                }
            }
            _unitOfWork.EducationDay.Save();
        }

        /// <summary>
        /// Parametre olarak gönderilen tarihin hafta içi olması durumunu kontrol eder.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Hafta içi ise true döner.</returns>
        private bool checkWeekdays(DateTime date)
        {
            if (!(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Parametre olarak gönderilen aktif günün tatil olup olmaması durumunu kontrol eder.
        /// </summary>
        /// <param name="date">Geçerli gün</param>
        /// <param name="offDays">Tatil günleri listesi</param>
        /// <returns>Tatil değil ise true döner.</returns>
        private bool checkNotHoliday(DateTime date, List<OffDay> offDays)
        {
            foreach (var offDay in offDays)
            {
                if (offDay.Date.Date == date.Date)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}