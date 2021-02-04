using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        private readonly UserUnitOfWork _userUnitOfWork;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserProfileController(SignInManager<ApplicationUser> signInManager ,UnitOfWork unitOfWork, UserUnitOfWork userUnitOfWork, UserManager<ApplicationUser> userManager, IStorageService storageService, IWebHostEnvironment hostingEnvironment)
        {
            _userUnitOfWork = userUnitOfWork;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _hostingEnvironment = hostingEnvironment;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
            _signInManager = signInManager;
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("hesap/panelim")]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var model = _userUnitOfWork.User.GetCustomerInfo(userId);
            var model = _userUnitOfWork.User.GetPanelInfo(userId);
            return View(model);
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("gruplarim/{ticketId?}")]
        public IActionResult MyGroup(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            var model = _userUnitOfWork.Group.GetMyGroupVm(ticketId.Value);
            if (model == null)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            return View(model);
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("faturalarim")]
        public IActionResult MyInvoices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetUserInvoices(userId);
            if (model == null)
                return Redirect($"/profil/{userId}");
            return View(model);
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Cancellation(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }
        [Route("hesap/menu")]
        public IActionResult MyMenu()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userUnitOfWork.User.GetMyAccountSidebarInfo(userId, null));
        }

        [Route("hesap/kurslarim")]
        public IActionResult MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetPurschasedEducationsByUserIdMyCoursesPage(userId);
            return View(model);
        }

        [Route("hesap/sertifikalarim")]
        public IActionResult MyCertificates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetUserCertificates(userId);
            return View(model);
        }

        [Route("hesap/kurs-detay/{groupId}")]
        public IActionResult MyCourseDetail(Guid groupId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool check = _userUnitOfWork.User.CheckPurschasedEducation(userId, groupId);
            if (!check)
                return RedirectToAction("MyCourses");

            var model = _userUnitOfWork.User.GetPurschasedEducationDetail(userId, groupId);
            if (model == null)
                return RedirectToAction("MyCourses");
            return View(model);
        }
        [Route("hesap/yorumlarim")]
        public IActionResult MyComments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerComments(userId);
            return View(model);
        }
        [Route("hesap/faturalarim")]
        public IActionResult MyInvoiceList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerInvoices(userId);
            return View(model);
        }
        [Route("hesap/fatura-detay")]
        public IActionResult InvoiceDetails(Guid invoiceId)
        {
            var model = _userUnitOfWork.User.GetCustomerInvoiceDetails(invoiceId);
            return View(model);
        }
        [Route("hesap/indirim-kuponlarim")]
        public IActionResult MyCoupons()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerPromotions(userId);
            return View(model);
        }
        [Route("hesap/favori-kurslarim")]
        public IActionResult MyFavoriteCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetUserFavoriteEducationsByUserId(userId);
            return View(model);
        }
        [Route("hesap/ayarlar")]
        public IActionResult AccountSettings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetAccoutSettingsPageData(userId);
            return View(model);
        }

        [Route("sana-ozel/{catSeoUrl?}")]
        public IActionResult ForYou(string catSeoUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetForYouPageData(userId);
            if (string.IsNullOrEmpty(catSeoUrl))
            {
                model.CategoryName = "Tüm Teknoloji Eğitimleri";
                model.CategoryShortDescription = "Tüm Teknoloji Eğitimleri";
            }
            else
            {
                var category = _unitOfWork.EducationCategory.GetCategoryBySeoUrl(catSeoUrl);
                if (category != null)
                {
                    model.CategoryId = category.Id;
                    model.CategoryName = category.Name;
                    model.CategoryShortDescription = category.Description;
                }
                else
                {
                    model.CategoryName = "Tüm Kategoriler";
                    model.CategoryShortDescription = "Geleceğin web sitelerini kodlayın";
                }
            }
            model.Categories = _unitOfWork.EducationCategory.GetCoursesPageCategories();
            model.OrderTypes = EnumHelpers.ToKeyValuePair<OrderCriteria>();
            model.EducationHostCities = EnumHelpers.ToKeyValuePair<HostCity>();
            model.TotalEducationCount = _unitOfWork.Education.TotalEducationCount();
            model.Educators = _unitOfWork.Educator.GetEducatorsAboutUsPage();
            model.FeaturedEducation = _unitOfWork.Education.GetFeaturedEducation();
            var popularCategories = _unitOfWork.EducationCategory.GetPopularCategories();
            foreach (var category in popularCategories)
            {
                category.IconUrl = _storageService.BlobUrl + category.IconUrl;
                category.BackgroundImageUrl = _storageService.BlobUrl + category.BackgroundImageUrl;
            };
            model.PopularCategories = popularCategories;
            return View(model);
        }

        [Route("profil-resmi-degistir")]
        public async Task<IActionResult> ChangeProfileImage(IFormFile ProfileImage)
        {
            var dbPath = "";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var fileName = StringHelpers.FormatForTag($"{user.Name} {user.Surname}");
            using (var output = new MemoryStream())
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(ProfileImage.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(250, 250));
                image.SaveAsJpeg(output);
                output.Position = 0;
                dbPath = await _storageService.UploadFile(output, $"{fileName}-{ProfileImage.FileName}", "user-avatars");
            }
            user.AvatarPath = dbPath;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profil resminiz başarıyla güncellendi!";
            return RedirectToAction("AccountSettings", "UserProfile");
        }

        [Route("haftaya-ozel-kurslar")]
        public IActionResult GetEducationsOfTheWeeks(int week)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _unitOfWork.Suggestions.GetEducationsOfTheWeek(week, userId);
            foreach (var education in model)
            {
                education.Image= _storageService.BlobUrl + education.Image;
            }
            return Json(new ResponseData
            {
                Success = true,
                Data = model
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-iletisim-bilgisi-guncelle")]
        public IActionResult UpdateUserContactInfo(UpdateUserContactInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                TempData["Error"] = errors.Aggregate((x, y) => x + "<br>" + y);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;
            _userUnitOfWork.User.UpdateUserContactInfo(model);
            TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
            return RedirectToAction("AccountSettings", "UserProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-kisisel-bilgiler-guncelle")]
        public IActionResult UpdateStudentInfo(UpdateStudentInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                TempData["Error"] = errors.Aggregate((x, y) => x + "<br>" + y);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;
            _userUnitOfWork.User.UpdateStudentInfo(model);
            TempData["Success"] = "Kişisel bilgileriniz başarıyla güncellendi!";
            return RedirectToAction("AccountSettings", "UserProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-bilgileri-guncelle")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                TempData["Error"] = errors.Aggregate((x, y) => x + "<br>" + y);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (model.OldPassword != null && model.NewPassword != null && model.ConfirmNewPassword != null)
            {
                if (model.NewPassword == model.ConfirmNewPassword)
                {
                    var retVal = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (retVal.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        TempData["Message"] = "Şifreniz güncellendi! Yeni şifreniz ile giriş yapabilirsiniz.";
                        return RedirectToAction("Login","Account");
                    }
                    else
                    {
                        TempData["Error"] = "Eski şifrenizi yanlış girdiniz! Şifreniz güncellenemedi.";
                        return RedirectToAction(nameof(AccountSettings));
                    }
                }
                TempData["Error"] = "Şifreleriniz uyuşmuyor!";
                return RedirectToAction(nameof(AccountSettings));
            }
            else
            {
                TempData["Error"] = "Bilgileriniz güncellenemedi!";
                return RedirectToAction(nameof(AccountSettings));
            }

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //model.UserId = userId;
            //if (model.ProfileImage != null)
            //{

            //    var fileName = StringHelpers.FormatForTag($"{user.Name} {user.Surname}");
            //    var dbPath = await _storageService.UploadFile(model.ProfileImage.OpenReadStream(), $"{fileName}-{model.ProfileImage.FileName}", "user-avatars");
            //    user.AvatarPath = dbPath;
            //    await _userManager.UpdateAsync(user);
            //}

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("default-adres-guncelle")]
        public IActionResult UpdateDefaultAddress(int? defaultAddressId)
        {
            if (!defaultAddressId.HasValue)
            {
                TempData["Error"] = "Lütfen sayfayı yenileyerek tekrar deneyiniz.";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                TempData["Error"] = errors.Aggregate((x, y) => x + "<br>" + y);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _unitOfWork.Address.UpdateDefaultAddress(defaultAddressId.Value, userId);
            TempData["Success"] = "Varsayılan adres bilginiz başarıyla güncellendi.";
            return RedirectToAction("AccountSettings", "UserProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("nbuy-egitim-bilgileri-guncelle")]
        public IActionResult UpdateNbuyEducationInfos(UpdateNBUYEducationInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                TempData["Error"] = errors.Aggregate((x, y) => x + "<br>" + y);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;
            _userUnitOfWork.User.UpdateNbuyEducationInfo(model);

            TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
            return RedirectToAction("AccountSettings", "UserProfile");
        }

        [Route("download-student-certificate")]
        public IActionResult DownloadStudentCertificate(Guid? groupId)
        {
            if (!groupId.HasValue)
            {
                TempData["Error"] = "Lütfen sayfayı yenileyerek tekrar deneyiniz.";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTcwOTE0QDMxMzcyZTMzMmUzMFFSQWFBQ05aQnljWmc2VXdZb1FtZ2VQcmlMK3ZEWWZZQzFRaW9aUTZhYnM9");
            if (!Syncfusion.Licensing.SyncfusionLicenseProvider.ValidateLicense(Syncfusion.Licensing.Platform.FileFormats, out var message))
                throw new Exception("Syncfusion license is invalid: " + message);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var zip = CreateCertificateZip(userId, groupId.Value);
            return File(zip.Content, System.Net.Mime.MediaTypeNames.Application.Zip, $"{zip.FileName}.zip");
        }


        #region Helpers

        public CertificateZipStreamVm CreateCertificateZip(string studentId,Guid groupId)
        {
            byte[] fileBytes = null;
            var docStream = CreateCertificatePDF(studentId, groupId);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    ZipArchiveEntry zipItem = zip.CreateEntry(docStream.FileName);
                    using (MemoryStream originalFileMemoryStream = docStream.Stream)
                    {
                        using (Stream entryStream = zipItem.Open())
                        {
                            originalFileMemoryStream.Position = 0;
                            originalFileMemoryStream.CopyTo(entryStream);
                        }
                    }
                }
                fileBytes = memoryStream.ToArray();
            }
            return new CertificateZipStreamVm
            {
                FileName = docStream.FileName,
                Content = fileBytes
            };
        }

        public CertificateStreamVm CreateCertificatePDF(string studentId,Guid groupId)
        {
            var certificate = _unitOfWork.CustomerCertificate.Get(x => x.CustomerId == studentId && x.GroupId == groupId).FirstOrDefault();
            if (certificate ==null)
            {
               var certificateId= _unitOfWork.CustomerCertificate.Insert(new CustomerCertificate
                {
                    CustomerId = studentId,
                    GroupId = groupId,
                });
                certificate = _unitOfWork.CustomerCertificate.GetById(certificateId);
            }


            var student = _unitOfWork.Customer.GetCustomerDetail(certificate.CustomerId);
            var group = _unitOfWork.EducationGroup.GetByIdWithEducation(certificate.GroupId);

            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Margins.All = 0;
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = new Syncfusion.Drawing.SizeF(960, 720);
            PdfPage page = doc.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            /*Başlangıç*/
            string webrootPath = _hostingEnvironment.WebRootPath;

            var bgImage = new FileStream(Path.Combine(webrootPath, "img/bireysel-egitim-sertifika-sablonu.jpg"), FileMode.Open, FileAccess.Read);
            PdfBitmap background = new PdfBitmap(bgImage);
            graphics.DrawImage(background, 0, 0, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height);

            //Yazı tipini belirle
            string path = Path.Combine(webrootPath, "assets/fonts/ProximaNova-Bold.ttf");
            Stream fontStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            //Create a new PDF true type font.
            PdfTrueTypeFont tFont = new PdfTrueTypeFont(fontStream, 12, PdfFontStyle.Regular);
            // Ad soyad yazdır

            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Layout = PdfLayoutType.OnePage;

            Syncfusion.Drawing.SizeF clipBounds = page.Graphics.ClientSize;

            string text = $"Sayın {student.Name.ToUpper()} {student.Surname.ToUpper()}";
            PdfStringFormat nameSurnameFormat = new PdfStringFormat();
            nameSurnameFormat.Alignment = PdfTextAlignment.Center;
            graphics.DrawString(text, tFont, PdfBrushes.Black, new Syncfusion.Drawing.RectangleF(0, 210, clipBounds.Width, 150), nameSurnameFormat);
            /*QR KOD Oluştur*/
            PdfQRBarcode barcode = new PdfQRBarcode();

            barcode.XDimension = 3;
            var siteUrl = $"https://niteliklitest2.azurewebsites.net/sertifika-dogrula/{certificate.Id}";

            barcode.Text = siteUrl;
            barcode.Size = new Syncfusion.Drawing.SizeF(100, 100);
            barcode.Draw(page, new Syncfusion.Drawing.PointF(760, 520));

            MemoryStream stream = new MemoryStream();

            doc.Save(stream);
            doc.Close(true);
            return new CertificateStreamVm { Stream = stream, FileName = $"{student.Name.ToUpper()}_{student.Surname.ToUpper()}_{MUsefulMethods.StringHelpers.FormatForTag(group.Education.Name)}_basari_sertifikasi.pdf" };
        }
        #endregion
    }
}
