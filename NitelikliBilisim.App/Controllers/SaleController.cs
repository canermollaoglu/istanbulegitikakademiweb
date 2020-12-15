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
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums.promotion;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Core.ViewModels.Cart;
using NitelikliBilisim.Core.ViewModels.Main.Cart;
using NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo;
using NitelikliBilisim.Core.ViewModels.Main.Sales;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        private readonly IEmailSender _emailSender;

        public SaleController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, IPaymentService paymentService, UserUnitOfWork userUnitOfWork, IEmailSender emailSender)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _vmCreator = new SaleVmCreator(_unitOfWork);
            _userUnitOfWork = userUnitOfWork;
            _emailSender = emailSender;
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("fatura-bilgileri")]
        public IActionResult InvoiceInformation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<InvoiceInfoAddressGetVm> addresses = _unitOfWork.Address.GetInvoiceAddressesByUserId(userId);
            var cities = _unitOfWork.City.Get().OrderBy(x=>x.Order).ToList();
            var defaultAddress = _unitOfWork.Address.GetDefaultAddress(userId);
            var invoiceInfos = new InvoiceInfoGetVm
            {
                Addresses = addresses,
                Cities = cities,
                DefaultAddressId = defaultAddress != null ? defaultAddress.Id : 0
            };
            return View(invoiceInfos);
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
                        total = 0m.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                        totalNumeric = 0
                    }
                });

            var cartItems = _vmCreator.GetCartItems(data.Items);

            var sum = 0m;

            if (cartItems.Count > 0)
                cartItems.ForEach(x => sum += x.PriceNumeric);

            var model = new
            {
                items = cartItems,
                total = sum.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                totalNumeric = sum
            };

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, IgnoreAntiforgeryToken, Route("get-basked-based-promotion")]
        public IActionResult GetBaskedBasedPromotion(GetCartItemsData data)
        {
            if (data == null || data.Items == null)
                return Json(new ResponseModel
                {
                    isSuccess = false

                });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var promotion = CheckBasketBasedPromotion(userId, data.Items);

            if (promotion != null)
            {
                var model = new BasketBasedPromotionVm
                {
                    DiscountAmount = promotion.DiscountAmount,
                    Name = promotion.Name
                };
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = model
                });
            }
            else
            {
                return Json(new ResponseModel
                {
                    isSuccess = false

                });
            }
        }

        [HttpPost, IgnoreAntiforgeryToken, Route("get-promotion")]
        public IActionResult GetPromotion(GetPromotionCodeData data)
        {
            if (string.IsNullOrEmpty(data.PromotionCode))
                return Json(new ResponseModel
                {
                    isSuccess = false

                });
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var retVal = CheckPromotionCode(userId, data.PromotionCode, data.Items);

            if (retVal.Success)
            {
                PromotionCodeVm promotionInfo = _unitOfWork.EducationPromotionCode.GetPromotionInfo(data.PromotionCode);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = promotionInfo
                });
            }
            else
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { retVal.Message }

                });
            }
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("odeme")]
        public IActionResult Payment()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/giris-yap?returnUrl=/odeme");

            return View();
        }
        [HttpPost, Route("getinstallmentinfo")]
        public IActionResult GetInstallmentInfo(InstallmentInfoVm data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var binNumber = FormatCardNumber(data.CardNumber).Substring(0, 6);
            decimal discountAmount = 0m;
            if (!string.IsNullOrEmpty(data.PromotionCode))
            {
                var response = CheckPromotionCode(userId, data.PromotionCode, data.CartItems);
                if (response.Success)
                {
                    var promotion = (EducationPromotionCode)response.Data;
                    discountAmount = promotion.DiscountAmount;
                }
                else
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { response.Message }
                    });
                }
            }
            var info = _paymentService.CheckInstallment(data.ConversationId.ToString(), binNumber, GetPriceSumForCartItems(data.CartItems, discountAmount));
            if (info.Status == PaymentServiceMessages.ResponseSuccess)
            {
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = new
                    {
                        installmentOptions = info.InstallmentDetails[0]
                    }
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
            #endregion

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            decimal discountAmount = 0m;
            EducationPromotionCode promotion = null;
            var basketBasedPromotion = CheckBasketBasedPromotion(userId, data.CartItems);
            if (!string.IsNullOrEmpty(data.PromotionCode))
            {
                decimal totalBasketAmount = GetPriceSumForCartItems(data.CartItems);
                var response = CheckPromotionCode(userId, data.PromotionCode, data.CartItems);
                if (response.Success)
                {
                    promotion = (EducationPromotionCode)response.Data;
                    discountAmount = promotion.DiscountAmount;
                    data.DiscountAmount = discountAmount;
                }
                else
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { response.Message }
                    });
                }
            }
            else if (basketBasedPromotion != null)
            {
                promotion = basketBasedPromotion;
                discountAmount = basketBasedPromotion.DiscountAmount;
                data.DiscountAmount = basketBasedPromotion.DiscountAmount;
            }

            data.CardInfo.NumberOnCard = FormatCardNumber(data.CardInfo.NumberOnCard);
            data.SpecialInfo.Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            data.SpecialInfo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            InstallmentInfo info = _paymentService.CheckInstallment(
                conversationId: data.ConversationId.ToString(),
                binNumber: data.CardInfo.NumberOnCard.Substring(0, 6),
                price: GetPriceSumForCartItems(data.CartItems, discountAmount));

            var cardInfoChecker = new CardInfoChecker();
            var transactionType = cardInfoChecker.DecideTransactionType(info, data.Use3d);

            var manager = new PaymentManager(_paymentService, transactionType);
            string content;
            var rootPath = _hostingEnvironment.WebRootPath;
            using (var sr = new StreamReader(Path.Combine(rootPath, "data/cities.json")))
            {
                content = await sr.ReadToEndAsync();
            }
            var address = _unitOfWork.Address.GetFullAddressById(data.AddressId);
            data.InvoiceAddress = address;

            var result = manager.Pay(_unitOfWork, data);
            NormalPaymentResultVm paymentResultModel = new NormalPaymentResultVm();

            if (result.TransactionType == TransactionType.Normal)
            {
                if (result.Status == PaymentServiceMessages.ResponseSuccess)
                {
                    var model = manager.CreateCompletionModel(result.PaymentForNormal);
                    _unitOfWork.Sale.CompletePayment(model, result.Success.InvoiceId, result.Success.InvoiceDetailIds);
                    if (promotion != null)
                    {
                        _unitOfWork.EducationPromotionItem.Insert(new EducationPromotionItem
                        {
                            UserId = userId,
                            EducationPromotionCodeId = promotion.Id,
                            InvoiceId = result.Success.InvoiceId,
                            CreatedDate = DateTime.Now
                        });
                    }
                    var customerEmail = _userUnitOfWork.User.GetCustomerInfo(userId).PersonalAndAccountInfo.Email;
                    if (customerEmail != null)
                    {
                        await _emailSender.SendAsync(new EmailMessage
                        {
                            Subject = "Eğitim ödemeniz alınmıştır | Nitelikli Bilişim",
                            Body = "Eğitim ödemeniz alınmıştır.",
                            Contacts = new[] { customerEmail }
                        });
                    }
                    paymentResultModel.Status = PaymentResultStatus.Success;
                    paymentResultModel.Message = "Ödemeniz başarılı bir şekilde gerçekleşmiştir.";
                }
                else
                {
                    paymentResultModel.Status = PaymentResultStatus.Failure;
                    paymentResultModel.Message = result.Error.ErrorMessage;
                }
                ViewData["PaymentResult"] = paymentResultModel;
                return RedirectToAction("NormalPaymentResult", "Sale");
            }

            if (result.TransactionType == TransactionType.Secure3d)
            {
                if (result.Status == PaymentServiceMessages.ResponseSuccess)
                {
                    string promotionId = promotion != null ? promotion.Id.ToString() : string.Empty;
                    _unitOfWork.TempSaleData.Create(result.ConversationId, result.Success, promotionId, userId);
                    HttpContext.Session.SetString("html_content", result.HtmlContent);
                    var customerEmail = _userUnitOfWork.User.GetCustomerInfo(userId).PersonalAndAccountInfo.Email;
                    if (customerEmail != null)
                    {
                        await _emailSender.SendAsync(new EmailMessage
                        {
                            Subject = "Eğitim ödemeniz alınmıştır | Nitelikli Bilişim",
                            Body = "Eğitim ödemeniz alınmıştır.",
                            Contacts = new[] { customerEmail }
                        });
                    }
                    return Redirect("/secure3d");
                }
                else
                {
                    paymentResultModel.Status = PaymentResultStatus.Failure;
                    paymentResultModel.Message = result.Error.ErrorMessage;
                    ViewData["PaymentResult"] = paymentResultModel;
                    return RedirectToAction("NormalPaymentResult", "Sale");
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
            NormalPaymentResultVm retVal = new NormalPaymentResultVm();
            if (data != null)
            {
                data.Locale = Locale.TR.ToString();
                var result = _paymentService.Confirm3DsPayment(data);
                if (result.Status == PaymentServiceMessages.ResponseSuccess)
                {
                    var manager = new PaymentManager(_paymentService, TransactionType.Secure3d);
                    var model = manager.CreateCompletionModel(result);
                    if (model == null)
                        return View();
                    retVal.Status = PaymentResultStatus.Success;
                    retVal.Message = "Ödemeniz başarılı bir şekilde gerçekleşmiştir.";
                    var paymentModelSuccess = _unitOfWork.TempSaleData.Get(data.ConversationId);
                    if (!string.IsNullOrEmpty(paymentModelSuccess.PromotionId))
                    {
                        _unitOfWork.EducationPromotionItem.Insert(new EducationPromotionItem
                        {
                            UserId = paymentModelSuccess.UserId,
                            EducationPromotionCodeId = Guid.Parse(paymentModelSuccess.PromotionId),
                            InvoiceId = paymentModelSuccess.InvoiceId,
                            CreatedDate = DateTime.Now
                        });
                    }
                    _unitOfWork.TempSaleData.Remove(data.ConversationId);
                    _unitOfWork.Sale.CompletePayment(model, paymentModelSuccess.InvoiceId, paymentModelSuccess.InvoiceDetailIds);
                }
                else
                {
                    retVal.Status = PaymentResultStatus.Failure;
                    retVal.Message = result.ErrorMessage;
                }
            }
            else
            {
                retVal.Status = PaymentResultStatus.Failure;
                retVal.Message = "Ödeme servisinden cevap alınamamıştır. Lütfen yönetici ile iletişime geçiniz.";
            }
            ViewData["PaymentResult"] = retVal;
            return RedirectToAction("NormalPaymentResult", "Sale");
        }

        [NonAction]
        public string FormatCardNumber(string cardNumber)
        {
            var splitted = cardNumber.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            return string.Join(null, splitted);
        }
        [NonAction]
        public decimal GetPriceSumForCartItems(List<_CartItem> itemIds, decimal discountAmount = 0)
        {
            var retVal = 0m;
            if (itemIds != null && itemIds.Count > 0)
            {
                var groupIds = itemIds.Select(x => x.GroupId).ToList();
                var totalPrice = _unitOfWork.EducationGroup.Get(x => groupIds.Contains(x.Id), null).Sum(x => x.NewPrice.GetValueOrDefault());
                var discount = discountAmount;
                retVal = totalPrice - discount;
            }

            return retVal;
        }

        public ResponseData CheckPromotionCode(string userId, string promotionCode, List<_CartItem> cartItems)
        {
            var promotion = _unitOfWork.EducationPromotionCode.GetPromotionbyPromotionCode(promotionCode);
            if (promotion == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Girdiğiniz koda ait kupon bulunamamıştır."
                };
            }
            decimal totalBasketAmount = GetPriceSumForCartItems(cartItems);
            var allEducations = _unitOfWork.Education.GetAllEducationsWithCategory();
            int userBasedItemCount = _unitOfWork.EducationPromotionCode.GetEducationPromotionItemCountByUserId(promotion.Id, userId);
            int promotionItemCount = _unitOfWork.EducationPromotionCode.GetEducationPromotionItemByPromotionCodeId(promotion.Id);

            #region Satın alınan eğitimler
            List<PurchasedEducationVm> purchasedEducations = _unitOfWork.Education.GetPurchasedEducationsByUserId(userId);
            #endregion

            if (userBasedItemCount + 1 > promotion.UserBasedUsageLimit || promotionItemCount + 1 > promotion.MaxUsageLimit)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Kupon kodu kullanım sınırı dolmuştur."
                };
            }

            if (totalBasketAmount < promotion.MinBasketAmount)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Sepet tutarınız minimum kupon kullanım tutarının altında."
                };
            }

            if (DateTime.Now.Date < promotion.StartDate.Date || DateTime.Now.Date > promotion.EndDate.Date)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Kupon kodu aktif değildir."
                };
            }

            foreach (var condition in promotion.EducationPromotionConditions)
            {
                if (condition.ConditionType == ConditionType.Category)
                {
                    var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                    var cartItemsEducationIds = cartItems.Select(x => x.EducationId).ToList();
                    var cartItemCategoryIds = allEducations.Where(x => cartItemsEducationIds.Contains(x.Id)).Select(x => x.CategoryId).ToList();
                    if (!cartItemCategoryIds.Any(x => ids.Contains(x)))
                    {
                        return new ResponseData
                        {
                            Success = false,
                            Message = "Kupon kodunun geçerli olabilmesi için kampanyada belirtilen kategorideki eğitimlerden birinin sepetinizde olması gerekiyor."
                        };
                    }
                }
                else if (condition.ConditionType == ConditionType.Education)
                {
                    var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                    if (!cartItems.Any(x => ids.Contains(x.EducationId)))
                    {
                        return new ResponseData
                        {
                            Success = false,
                            Message = "Kupon kodunun geçerli olabilmesi için kampanyada belirtilen eğitimlerden birinin sepetinizde olması gerekiyor."
                        };
                    }
                }
                else if (condition.ConditionType == ConditionType.User)
                {
                    var ids = JsonConvert.DeserializeObject<string[]>(condition.ConditionValue);
                    if (!ids.Contains(userId))
                    {
                        return new ResponseData
                        {
                            Success = false,
                            Message = "Geçerli şartlar sağlanmadığı için indirim aktif edilemiyor."
                        };
                    }
                }
                else if (condition.ConditionType == ConditionType.PurchasedEducation)
                {
                    var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                    if (!purchasedEducations.Any(x => ids.Contains(x.EducationId)))
                    {
                        return new ResponseData
                        {
                            Success = false,
                            Message = "Kupon kodunun geçerli olabilmesi için kampanyada belirtilen eğitimlerden birini almış olmanız gerekiyor."
                        };
                    }
                }
                else if (condition.ConditionType == ConditionType.PurchasedCategory)
                {
                    var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                    if (!purchasedEducations.Any(x => ids.Contains(x.CategoryId)))
                    {
                        return new ResponseData
                        {
                            Success = false,
                            Message = "Kupon kodunun geçerli olabilmesi için kampanyada belirtilen kategoriden bir eğitim almış olmanız gerekiyor."
                        };
                    }
                }
            }



            return new ResponseData
            {
                Success = true,
                Data = promotion
            };

        }

        public EducationPromotionCode CheckBasketBasedPromotion(string userId, List<_CartItem> cartItems)
        {
            decimal totalBasketAmount = GetPriceSumForCartItems(cartItems);
            var promotions = _unitOfWork.EducationPromotionCode.GetBasketBasedPromotions()
                .Where(x => x.StartDate.Date <= DateTime.Now.Date
                && x.EndDate.Date > DateTime.Now.Date
                && x.MinBasketAmount < totalBasketAmount).ToList();
            #region Satın alınan eğitimler
            List<PurchasedEducationVm> purchasedEducations = _unitOfWork.Education.GetPurchasedEducationsByUserId(userId);
            #endregion
            var applicatePromotionIds = new List<Guid>();
            var allEducations = _unitOfWork.Education.GetAllEducationsWithCategory();
            foreach (var promotion in promotions)
            {
                int userBasedItemCount = _unitOfWork.EducationPromotionCode.GetEducationPromotionItemCountByUserId(promotion.Id, userId);
                int promotionItemCount = _unitOfWork.EducationPromotionCode.GetEducationPromotionItemByPromotionCodeId(promotion.Id);

                if (userBasedItemCount + 1 <= promotion.UserBasedUsageLimit && promotionItemCount + 1 <= promotion.MaxUsageLimit)
                {
                    applicatePromotionIds.Add(promotion.Id);
                    foreach (var condition in promotion.EducationPromotionConditions)
                    {
                        if (condition.ConditionType == ConditionType.Category)
                        {
                            var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                            var cartItemsEducationIds = cartItems.Select(x => x.EducationId).ToList();
                            var cartItemCategoryIds = allEducations.Where(x => cartItemsEducationIds.Contains(x.Id)).Select(x => x.CategoryId).ToList();
                            if (!cartItemCategoryIds.Any(x => ids.Contains(x)))
                            {
                                applicatePromotionIds.Remove(promotion.Id);
                            }
                        }
                        else if (condition.ConditionType == ConditionType.Education)
                        {
                            var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                            if (!cartItems.Any(x => ids.Contains(x.EducationId)))
                            {
                                applicatePromotionIds.Remove(promotion.Id);
                            }
                        }
                        else if (condition.ConditionType == ConditionType.User)
                        {
                            var ids = JsonConvert.DeserializeObject<string[]>(condition.ConditionValue);
                            if (!ids.Contains(userId))
                            {
                                applicatePromotionIds.Remove(promotion.Id);
                            }
                        }
                        else if (condition.ConditionType == ConditionType.PurchasedEducation)
                        {
                            var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                            if (!purchasedEducations.Any(x => ids.Contains(x.EducationId)))
                            {
                                applicatePromotionIds.Remove(promotion.Id);
                            }
                        }
                        else if (condition.ConditionType == ConditionType.PurchasedCategory)
                        {
                            var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                            if (!purchasedEducations.Any(x => ids.Contains(x.CategoryId)))
                            {
                                applicatePromotionIds.Remove(promotion.Id);
                            }
                        }
                    }
                }
            }

            return promotions.Where(x => applicatePromotionIds.Contains(x.Id)).OrderByDescending(x => x.DiscountAmount).FirstOrDefault();


        }

        [Route("get-states/{cityId}")]
        public IActionResult GetStatesByCityId(int cityId)
        {
            try
            {
                List<State> states = _unitOfWork.State.GetStateByCityId(cityId);
                return Json(new ResponseData
                {
                    Success = true,
                    Data = states
                });

            }
            catch (Exception)
            {
                //Todo Log
                throw;
            }
        }

        public IActionResult AddCorporateAddress(AddCorporateAddressPostVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.CustomerId = userId;
                model.IsDefaultAddress = true;
                _unitOfWork.Address.AddCorporateAddress(model);
                return RedirectToAction("InvoiceInformation", "Sale");
            }
            catch (Exception)
            {
                //Todo Log
                throw;
            }

        }
        public IActionResult AddIndividualAddress(AddIndividualAddressPostVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.CustomerId = userId;
                model.IsDefaultAddress = true;
                _unitOfWork.Address.AddIndividualAddress(model);
                return RedirectToAction("InvoiceInformation", "Sale");
            }
            catch (Exception)
            {
                //Todo Log
                throw;
            }
        }
    }

    public class GetPromotionCodeData
    {
        public List<_CartItem> Items { get; set; }
        public string PromotionCode { get; set; }
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