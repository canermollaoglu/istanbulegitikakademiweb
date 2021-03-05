using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Main.Sales;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class SaleCancellationController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IMessageService _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public SaleCancellationController(IWebHostEnvironment env,UserManager<ApplicationUser> userManager,UnitOfWork unitOfWork, IPaymentService paymentService, IMessageService emailSender)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _emailSender = emailSender;
            _userManager = userManager;
            _env = env;
        }

        [Route("iptal-formu/{invoiceDetailId?}")]
        public IActionResult CancellationForm(Guid? invoiceDetailId)
        {
            if (!invoiceDetailId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }

        [HttpPost, Route("iptal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(CancellationFormPostVm data)
        {
            var invoice = _unitOfWork.Invoice.GetByIdWithOnlinePaymentInfos(data.InvoiceId);
            var conversationId = Guid.NewGuid().ToString();
            var cancelRequest = _paymentService.CreateCancelRequest(conversationId, invoice.OnlinePaymentInfo.PaymentId, "", RefundReason.BUYER_REQUEST, data.UserDescription);
            var user = _unitOfWork.Customer.GetCustomerDetail(invoice.CustomerId);
            if (cancelRequest.Status == PaymentServiceMessages.ResponseSuccess)
            {
                _unitOfWork.Sale.CancelPayment(data.InvoiceId);

                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                var emailMessage = new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Eğitim İptali",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından {invoice.CreatedDate} tarihinde oluşutrulmuş fatura için iptal talebi oluşturulmuştur.",
                    Contacts = emails
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(emailMessage));

                return Json(new ResponseData
                {
                    Success = true,
                    Message = "Başarıyla iptal edildi."
                });
            }

            return Json(new ResponseData
            {
                Success = false,
                Message = "Bir sorun oluştu."
            });
        }

        [HttpPost, Route("iade")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(RefundVm data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var invoiceDetail = _unitOfWork.InvoiceDetail.GetByIdWithOnlinePaymentDetailInfo(data.InvoiceDetailId);
            if (invoiceDetail.Invoice.CustomerId != userId)
            {
                TempData["Error"] = "İşlem gerçekleşmedi! Lütfen yöneticiye danışınız.";
                return RedirectToAction("MyCourseDetail", "UserProfile",new { groupId =invoiceDetail.GroupId});
            }

            decimal refundPrice = 0;
            var conversationId = Guid.NewGuid().ToString();
            var group = _unitOfWork.EducationGroup.GetByIdWithInvoiceInfo(invoiceDetail.GroupId,userId);
            var ticket = _unitOfWork.Ticket.GetByInvoiceDetailId(invoiceDetail.Id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (ticket!=null && ticket.IsUsed && DateTime.Now.Date>group.StartDate.Date)
            {//Kısmi İptal (Kalan gün)
                var education = ticket.Education;
                decimal dailyPrice = group.PaidPrice/ education.Days;
                var gStudent = _unitOfWork.EducationGroup.GetEducationGroupByTicketId(ticket.Id);
                var daysLeft = gStudent.GroupLessonDays.Count(x => x.DateOfLesson.Date > DateTime.Now.Date);
                refundPrice = daysLeft * dailyPrice;
            }
            else//Tamamı iptal ediliyor.
            {
                refundPrice = invoiceDetail.OnlinePaymentDetailInfo.PaidPrice;
            }
            var refundRequest = _paymentService.CreateRefundRequest(conversationId, invoiceDetail.OnlinePaymentDetailInfo.TransactionId, refundPrice, "", RefundReason.BUYER_REQUEST, data.UserDescription);
            

            if (refundRequest.Status == PaymentServiceMessages.ResponseSuccess)
            {
                _unitOfWork.Sale.RefundPayment(data.InvoiceDetailId,refundPrice);

                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                var emailMessage = new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Eğitim İadesi",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından satın alınan {group.GroupName} grubundaki {group.EducationName} eğitimi iade edilmiştir.",
                    Contacts = emails
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(emailMessage));
                #region Customer Email
                var builder = string.Empty;
                var templatePath = Path.Combine(_env.WebRootPath, "mail-templates", "mail-template.html");
                using (StreamReader r = System.IO.File.OpenText(templatePath))
                {
                    builder = r.ReadToEnd();
                }
                builder = builder.Replace("[##subject##]", "Eğitiminiz İptal Edilmiştir!");
                builder = builder.Replace("[##content##]", $"Sayın {user.Name} {user.Surname}, eğitim eğitim iptali işleminiz başarılı bir şekilde gerçekleşmiştir.");
                builder = builder.Replace("[##content2##]", "Eğitim ödemenizden kalan tutar hesabınıza iade edilmiştir.");
                if (user.Email != null)
                {
                    var refundEmailMessage = new EmailMessage
                    {
                        Subject = "Eğitim iptal işleminiz tamamlanmıştır. | Nitelikli Bilişim",
                        Body = builder,
                        Contacts = new[] { user.Email }
                    };
                    await _emailSender.SendAsync(JsonConvert.SerializeObject(refundEmailMessage));
                }
                #endregion

                TempData["Success"] =  $"{group.EducationName} Eğitimi için iptal işleminiz tamamlanmıştır.";
                return RedirectToAction("MyCourses", "UserProfile");
            }
            else
            {
                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                var message = new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Eğitim İadesi",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından satın alınan {group.GroupName} grubundaki {group.EducationName} eğitimi iade edilememiştir.<br>Hata: {refundRequest.ErrorMessage}",
                    Contacts = emails
                };
                await _emailSender.SendAsync(JsonConvert.SerializeObject(message));
                TempData["Error"] = "İptal işleminiz gerçekleşmedi ilgili birimlerimiz kısa sürede sizinle iletişime geçecektir.";
                return RedirectToAction("MyCourseDetail", "UserProfile", new { groupId = invoiceDetail.GroupId });
            }

        }
    }
}