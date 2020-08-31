using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.App.VmCreator;
using NitelikliBilisim.Business.Factory;
using NitelikliBilisim.Business.PaymentFactory;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Cart;
using NitelikliBilisim.Core.ViewModels.Main.Sales;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    public class SaleController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UnitOfWork _unitOfWork;
        private readonly SaleVmCreator _vmCreator;
        private readonly UserUnitOfWork _userUnitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly EmailSender _emailSender;

        public SaleController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, IPaymentService paymentService, UserUnitOfWork userUnitOfWork)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _vmCreator = new SaleVmCreator(_unitOfWork);
            _userUnitOfWork = userUnitOfWork;
            _emailSender = new EmailSender();
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("sepet")]
        public IActionResult Cart()
        {
            return View();
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost, IgnoreAntiforgeryToken, Route("get-cart-items")]
        public IActionResult GetCartItems(GetCartItemsData data)
        {
            if (data == null || data.Items == null)
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = new
                    {
                        items = new List<CartItemVm>(),
                        total = 0m.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
                    }
                });

            var cartItems = _vmCreator.GetCartItems(data.Items);

            var sum = 0m;

            if (cartItems.Count > 0)
                cartItems.ForEach(x => sum += x.PriceNumeric);

            var model = new
            {
                items = cartItems,
                total = sum.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
            };

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("odeme")]
        public IActionResult Payment()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/giris-yap?returnUrl=/odeme");

            return View();
        }
        [HttpPost,Route("getinstallmentinfo")]
        public IActionResult GetInstallmentInfo(InstallmentInfoVm data)
        {
            var binNumber = data.BinNumber.Replace(" ", "").Substring(0, 6);
            InstallmentInfo info = _paymentService.CheckInstallment(
                conversationId: data.ConversationId.ToString(),
                binNumber: binNumber,
                price: GetPriceSumForCartItems(data.CartItems)
                );
            if (info.Status == "success")
            {
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = info
                });
            }
            else
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Taksit bilgileri alınamadı. Sayfayı yenileyerek tekrar deneyiniz." }
                });
            }
            
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost, ValidateAntiForgeryToken, Route("pay")]
        public async Task<IActionResult> Pay(PayData data)
        {
            #region Validation
            if (!HttpContext.User.Identity.IsAuthenticated || data.CartItemsJson == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sepette ürün bulunmamaktadır" }
                });

            data.CartItems = JsonConvert.DeserializeObject<List<_CartItem>>(data.CartItemsJson);

            if (data.CartItems == null || data.CartItems.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sepette ürün bulunmamaktadır" }
                });
            if (!data.IsDistantSalesAgreementConfirmed)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Mesafeli Satış sözleşmesini onaylayınız" }
                });
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });
            if (!data.InvoiceInfo.IsIndividual && data.CorporateInvoiceInfo == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "?" }
                });
            #endregion

            data.CardInfo.NumberOnCard = FormatCardNumber(data.CardInfo.NumberOnCard);
            data.SpecialInfo.Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            data.SpecialInfo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            InstallmentInfo info = _paymentService.CheckInstallment(
                conversationId: data.ConversationId.ToString(),
                binNumber: data.CardInfo.NumberOnCard.Substring(0, 6),
                price: GetPriceSumForCartItems(data.CartItems));

            var cardInfoChecker = new CardInfoChecker();
            var transactionType = cardInfoChecker.DecideTransactionType(info, data.Use3d);

            var manager = new PaymentManager(_paymentService, transactionType);
            string content = "";
            var rootPath = _hostingEnvironment.WebRootPath;
            using (var sr = new StreamReader(Path.Combine(rootPath, "data/cities.json")))
            {
                content = sr.ReadToEnd();
            }
            var cityTowns = JsonConvert.DeserializeObject<CityTownModel>(content);
            var cityId = data.InvoiceInfo.City;
            var townId = data.InvoiceInfo.Town;
            var city = cityTowns.data.FirstOrDefault(x => x._id == cityId);
            data.InvoiceInfo.City = city.name;
            data.InvoiceInfo.Town = city.towns.FirstOrDefault(x => x._id == townId).name;

            var result = manager.Pay(_unitOfWork, data);

            if (result.TransactionType == TransactionType.Normal)
            {
                var model = manager.CreateCompletionModel(result.PaymentForNormal);
                _unitOfWork.Sale.CompletePayment(model, result.Success.InvoiceId, result.Success.InvoiceDetailIds);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var customerEmail = _userUnitOfWork.User.GetCustomerInfo(userId).PersonalAndAccountInfo.Email;
                if (customerEmail != null)
                {
                    await _emailSender.SendAsync(new EmailMessage
                    {
                        Subject = "Eğitim ödemeniz alınmıştır | Nitelikli Bilişim",
                        Body = "Eğitim ödemeniz alınmıştır.",
                        Contacts = new string[] { customerEmail }
                    });
                }

                return Redirect("/odeme-sonucunuz");
            }

            if (result.TransactionType == TransactionType.Secure3d)
            {
                if (result.Status == "success")
                {
                    _unitOfWork.TempSaleData.Create(result.ConversationId, result.Success);
                    HttpContext.Session.SetString("html_content", result.HtmlContent);

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var customerEmail = _userUnitOfWork.User.GetCustomerInfo(userId).PersonalAndAccountInfo.Email;
                    if (customerEmail != null)
                    {
                        await _emailSender.SendAsync(new EmailMessage
                        {
                            Subject = "Eğitim ödemeniz alınmıştır | Nitelikli Bilişim",
                            Body = "Eğitim ödemeniz alınmıştır.",
                            Contacts = new string[] { customerEmail }
                        });
                    }

                    return Redirect("/secure3d");
                }
            }

            return Redirect("/");
        }

        [Route("secure3d")]
        public IActionResult Secure3d()
        {
            return View(new Secure3dModel
            {
                HtmlContent = HttpContext.Session.GetString("html_content")
            });
        }

        [HttpGet, Route("odeme-sonucunuz")]
        public IActionResult NormalPaymentResult()
        {
            return View();
        }

        [HttpPost, Route("odeme-sonucu")]
        public IActionResult PaymentResult(CreateThreedsPaymentRequest data)
        {
            if (data != null)
            {
                data.Locale = Locale.TR.ToString();
                var result = _paymentService.Confirm3DsPayment(data);
                var manager = new PaymentManager(_paymentService, TransactionType.Secure3d);
                var model = manager.CreateCompletionModel(result);
                if (model == null)
                    return View();

                var paymentModelSuccess = _unitOfWork.TempSaleData.Get(data.ConversationId);
                _unitOfWork.TempSaleData.Remove(data.ConversationId);
                _unitOfWork.Sale.CompletePayment(model, paymentModelSuccess.InvoiceId, paymentModelSuccess.InvoiceDetailIds);
            }

            return View();
        }

        public IActionResult GetCardInfo()
        {
            var conversationId = Guid.NewGuid().ToString();
            var info = _paymentService.CheckInstallment(conversationId, "111111", 800);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = new
                {
                    installmentOptions = info.InstallmentDetails[0].InstallmentPrices
                }
            });
        }

        [NonAction]
        public string FormatCardNumber(string cardNumber)
        {
            var splitted = cardNumber.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            return string.Join(null, splitted);
        }
        [NonAction]
        public decimal GetPriceSumForCartItems(List<_CartItem> itemIds)
        {
            var educationIds = itemIds.Select(x => x.EducationId).ToList();
            return _unitOfWork.Education.Get(x => educationIds.Contains(x.Id), null).Sum(x => x.NewPrice.Value);
        }
    }

    public class GetCartItemsData
    {
        public List<_CartItem> Items { get; set; }
    }
    public class Secure3dModel
    {
        public string HtmlContent { get; set; }
    }
}