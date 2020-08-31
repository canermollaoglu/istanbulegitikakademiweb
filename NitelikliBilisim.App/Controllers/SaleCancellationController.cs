using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Main.Sales;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class SaleCancellationController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        public SaleCancellationController(UnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        [Route("iptal-formu/{invoiceDetailId?}")]
        public IActionResult CancellationForm(Guid? invoiceDetailId)
        {
            if (!invoiceDetailId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }

        [HttpPost, Route("iptal")]
        public IActionResult Cancel(CancellationFormPostVm data)
        {
            var invoice = _unitOfWork.Invoice.GetByIdWithOnlinePaymentInfos(data.InvoiceId);
            var conversationId = Guid.NewGuid().ToString();
            var cancelRequest = _paymentService.CreateCancelRequest(conversationId, invoice.OnlinePaymentInfo.PaymentId, "", RefundReason.BUYER_REQUEST, data.UserDescription);

            if (cancelRequest.Status == PaymentServiceMessages.ResponseSuccess)
            {
                _unitOfWork.Sale.CancelPayment(data.InvoiceId);

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
        public IActionResult Refund(RefundVm data)
        {
            decimal refundPrice = 0;
            var conversationId = Guid.NewGuid().ToString();
            var invoiceDetail = _unitOfWork.InvoiceDetail.GetByIdWithOnlinePaymentDetailInfo(data.InvoiceDetailId);
            var ticket = _unitOfWork.Ticket.GetByInvoiceDetailId(invoiceDetail.Id);
            if (ticket!=null && ticket.IsUsed)
            {//Kısmi İptal (Kalan gün)
                var education = ticket.Education;
                decimal dailyPrice = education.NewPrice.Value / education.Days;
                var gStudent = _unitOfWork.EducationGroup.GetEducationGroupByTicketId(ticket.Id);
                var daysLeft = gStudent.GroupLessonDays.Count(x => x.DateOfLesson.Date > DateTime.Now.Date);
                refundPrice = daysLeft * dailyPrice;
            }
            else//Tamamı iptal ediliyor.
            {
                refundPrice = invoiceDetail.OnlinePaymentDetailInfo.Price;
            }
            var refundRequest = _paymentService.CreateRefundRequest(conversationId, invoiceDetail.OnlinePaymentDetailInfo.TransactionId, refundPrice, "", RefundReason.BUYER_REQUEST, data.UserDescription);

            if (refundRequest.Status == PaymentServiceMessages.ResponseSuccess)
            {
                _unitOfWork.Sale.RefundPayment(data.InvoiceDetailId);

                return Json(new ResponseData
                {
                    Success = true,
                    Message = "İade talebiniz alınmıştır."
                });
            }

            return Json(new ResponseData
            {
                Success = false,
                Message = "Bir sorun oluştu."
            });
        }
    }
}