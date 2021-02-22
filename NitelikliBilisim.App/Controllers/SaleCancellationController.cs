using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Main.Sales;
using NitelikliBilisim.Notificator.Services;
using System;
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
        private readonly IEmailSender _emailSender;
        public SaleCancellationController(UnitOfWork unitOfWork, IPaymentService paymentService,IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _emailSender = emailSender;
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
                await _emailSender.SendAsync(new EmailMessage
                {
                    Subject ="Nitelikli Bilişim Eğitim İptali",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından {invoice.CreatedDate} tarihinde oluşutrulmuş fatura için iptal talebi oluşturulmuştur.",
                    Contacts = emails
                });

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
            var user = _unitOfWork.Customer.GetCustomerDetail(ticket.OwnerId);

            if (refundRequest.Status == PaymentServiceMessages.ResponseSuccess)
            {
                _unitOfWork.Sale.RefundPayment(data.InvoiceDetailId,refundPrice);

                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                await _emailSender.SendAsync(new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Eğitim İadesi",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından satın alınan {group.GroupName} grubundaki {group.EducationName} eğitimi iade edilmiştir.",
                    Contacts = emails
                });

                TempData["Success"] = "İptal talebiniz alınmıştır.";
                return RedirectToAction("MyCourseDetail", "UserProfile", new { groupId = invoiceDetail.GroupId });
            }
            else
            {
                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                await _emailSender.SendAsync(new EmailMessage
                {
                    Subject = "Nitelikli Bilişim Eğitim İadesi",
                    Body = $"{user.Name} {user.Surname} ({user.Email}) kullanıcısı tarafından satın alınan {group.GroupName} grubundaki {group.EducationName} eğitimi iade edilememiştir.<br>Hata: {refundRequest.ErrorMessage}",
                    Contacts = emails
                });
                TempData["Error"] = "İptal işleminiz gerçekleşmedi ilgili birimlerimiz kısa sürede sizinle iletişime geçecektir.";
                return RedirectToAction("MyCourseDetail", "UserProfile", new { groupId = invoiceDetail.GroupId });
            }

        }
    }
}