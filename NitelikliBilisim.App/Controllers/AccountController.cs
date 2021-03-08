using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.helper;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace NitelikliBilisim.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserUnitOfWork _userUnitOfWork;
        private readonly TransactionLogRepository _transactionLogRepository;
        private readonly IMessageService _emailSender;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;


        public AccountController(IWebHostEnvironment env,TransactionLogRepository transactionLogRepository, IConfiguration configuration, UserUnitOfWork userUnitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UnitOfWork unitOfWork, IMessageService emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _configuration = configuration;
            _userUnitOfWork = userUnitOfWork;
            _env = env;
            _transactionLogRepository = transactionLogRepository;
        }

        [Route("kayit-ol")]
        public IActionResult Register()
        {
            var model = new RegisterGetVm
            {
                EducationCenters = EnumHelpers.ToKeyValuePair<EducationCenter>(),
                EducationCategories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, x => x.OrderBy(o => o.Name))
            };

            return View(model);
        }

        [HttpPost, Route("kayit-ol")]
        public async Task<IActionResult> Register(RegisterPostVm model)
        {
            if (!model.AcceptedTerms || !ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = ModelStateUtil.GetErrors(ModelState).Aggregate((x, y) => x + "<br>" + y)
                });


            var retVal = await _userUnitOfWork.User.RegisterUser(model);
            if (retVal.Success)
            {
                var user = (ApplicationUser)retVal.Data;
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var htmlToken = HttpUtility.UrlEncode(token);
                var template = _unitOfWork.EmailTemplate.Get(x => x.Type == EmailTemplateType.General).First();
                var builder = template.Content;
                
                var confirmationLink = $"{ Request.Scheme}://{Request.Host}{Request.PathBase}/email-aktivasyonu?token={htmlToken}&email={user.Email}";
                builder = builder.Replace("[##subject##]", "E-Posta Aktivasyonu!");
                builder = builder.Replace("[##content##]", $"E-posta adresinizi onaylamak için <a href=\"{confirmationLink}\">tıklayınız.</a>");
                builder = builder.Replace("[##content2##]", "");
                var message = new EmailMessage
                {
                    Contacts = new string[] { user.Email },
                    Subject = "Nitelikli Bilişim Email Aktivasyonu",
                    Body = builder
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(message));
                TempData["Message"] = "Hesabınız başarılı bir şekilde oluşturuldu. E-Posta onayı sonrası giriş yapabilirsiniz.";
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            else
            {
                string[] adminEmails = _configuration.GetSection("SiteGeneralOptions").GetSection("AdminEmails").Value.Split(";");
                string mailBody = $"Kullanıcı Adı :{model.Email}<br>Telefon:{model.Phone}<br>Ad:{model.Name}<br>Soyad:{model.Surname}";
                mailBody += $"<br>Hata : {JsonConvert.SerializeObject(retVal.Data)} + {retVal.Message}";
                var eMailMessage = new EmailMessage
                {
                    Body = mailBody,
                    Subject = "Nitelikli Bilişim Kullanıcı Kayıt Hatası!",
                    Contacts = adminEmails
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(eMailMessage));
                IEnumerable<IdentityError> identityErrors = null;
                if (retVal.Data != null)
                {
                    identityErrors = ((IEnumerable<IdentityError>)retVal.Data);
                }

                var message = "";
                if (identityErrors != null)
                {
                    if (identityErrors.Any(x => x.Code.Contains("DuplicateEmail")))
                    {
                        message = "Bu e-posta sistemde kayıtlı! Eğer şifrenizi hatırlamıyorsanız <a href=\"/sifremi-unuttum\">buraya</a> tıklayarak yeni şifre oluşturabilirsiniz.";
                    }
                    else
                    {
                        message = identityErrors.Select(x => x.Description).Aggregate((x, y) => x + " " + y);
                    }
                }

                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = message
                });
            }
        }


        [HttpGet]
        [Route("email-aktivasyonu")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                ViewData["Message"] = "Eski veya geçersiz bir başvuruda bulundunuz!";
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewData["Message"] = "Email adresiniz onaylanmıştır. Giriş yapabilirsiniz.";
            }
            else
            {
                ViewData["Message"] = "Eski veya geçersiz bir başvuruda bulundunuz!";
            }
            return View();
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
                var user = await _userManager.FindByNameAsync(model.UserName);
                string sessionId = HttpContext.Session.GetString("userSessionId");
                if (!string.IsNullOrEmpty(sessionId))
                {
                    UpdateTransactionLogsSetUserId(user.Id, sessionId);
                }


                //var isNbuy = _unitOfWork.Customer.IsNbuyStudent(user.Id);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return Redirect(model.ReturnUrl ?? "/");
            }
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı!");
            return View(model);
        }

        /// <summary>
        /// Kullanıcı giriş yaptığında sahip olduğu sessionId ile tutulan ve userId alanı boş olan tüm loglara userId import ediliyor.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private void UpdateTransactionLogsSetUserId(string userId, string sessionId)
        {
            var logList = _transactionLogRepository.GetList(x => x.SessionId==sessionId&& string.IsNullOrEmpty(x.UserId));

            foreach (var log in logList)
            {
                log.UserId = userId;
                _transactionLogRepository.Update(log.Id.ToString(), log);
            }
        }


        [Route("sifremi-unuttum")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("sifremi-unuttum")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                var template = _unitOfWork.EmailTemplate.Get(x => x.Type == EmailTemplateType.General).First();
                var builder = template.Content;
                builder = builder.Replace("[##subject##]", "Şifre Sıfırlama!");
                builder = builder.Replace("[##content##]", $"Merhaba {user.Name} {user.Surname},<br/> Şifrenizi yenilemek için <a href=\"{passwordResetLink}\" target=\"_blank\"><b>buraya</b></a> tıklayınız.");
                builder = builder.Replace("[##content2##]", "");
                var emailMessage = new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Şifre Yenileme",
                    Contacts = new string[] { model.Email },
                    Body = builder
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(emailMessage));
                TempData["Message"] = "Şifre yenileme linki E-posta adresinize gönderilmiştir.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Bu E-posta adresi ile bağlantılı bir kayıt bulunmamaktadır.");
                return View(model);
            }

        }


        [HttpGet]
        [Route("sifre-sifirla")]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Geçersiz istek.");
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("sifre-sifirla")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Yeni şifreniz ile giriş yapabilirsiniz.";
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamıyor. Yönetici ile irtibata geçebilirsiniz.");
            }
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
                            StringHelpers.CharacterConverter(info.Principal.FindFirstValue(ClaimTypes.GivenName));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        model.Name =
                            StringHelpers.CharacterConverter(info.Principal.FindFirstValue(ClaimTypes.Name));
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                    {
                        model.Surname =
                            StringHelpers.CharacterConverter(info.Principal.FindFirstValue(ClaimTypes.Surname));
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