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

            if (cancelRequest.Status == "success")
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
        public IActionResult Refund(CancellationFormPostVm data)
        {
            var conversationId = Guid.NewGuid().ToString();
            var invoiceDetail = _unitOfWork.InvoiceDetail.GetByIdWithOnlinePaymentDetailInfo(data.InvoiceDetailId.Value);
            if (invoiceDetail.IsUsedAsTicket)
            {
                var ticket = _unitOfWork.Ticket.GetByInvoiceDetailId(invoiceDetail.Id);
                //TODO saatlik ücret hesaplaması yapılarak kalan günler için iade edilecek tutar bulunacak.
            }
            
            var refundRequest = _paymentService.CreateRefundRequest(conversationId, invoiceDetail.OnlinePaymentDetailInfo.TransactionId, invoiceDetail.OnlinePaymentDetailInfo.Price, "", RefundReason.BUYER_REQUEST, data.UserDescription);

            if(refundRequest.Status == "success")
            {
                _unitOfWork.Sale.RefundPayment(data.InvoiceDetailId.Value);

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