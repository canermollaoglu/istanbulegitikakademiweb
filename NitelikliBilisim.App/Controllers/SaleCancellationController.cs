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

        [Route("iptal-formu/{ticketId?}")]
        public IActionResult CancellationForm(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }

        [HttpPost, Route("iptal")]
        public IActionResult Cancel(CancellationFormPostVm data)
        {
            var conversationId = Guid.NewGuid().ToString();
            var cancelRequest = _paymentService.CreateCancelRequest(conversationId, "", "", RefundReason.BUYER_REQUEST, data.UserDescription);

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
            var refundRequest = _paymentService.CreateRefundRequest(conversationId, "", 0m, "", RefundReason.BUYER_REQUEST, data.UserDescription);

            if(refundRequest.Status == "success")
            {
                _unitOfWork.Sale.RefundPayment(data.TicketId);

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