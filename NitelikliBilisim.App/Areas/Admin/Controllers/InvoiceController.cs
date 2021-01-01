using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.invoice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly UnitOfWork _unitOfWork; 
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public InvoiceController(IStorageService storage ,IWebHostEnvironment hostingEnvironment,UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "pdf");
            _storage = storage;
        }

        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminInvoices");
            return View();
        }

        [Route("admin/invoice/upload-invoice-pdf")]
        [HttpPost]
        public async Task<IActionResult> UploadInvoicePfd(UploadInvoicePdfVm data)
        {
            if (!ModelState.IsValid || data.InvoiceId == Guid.Empty)
            {
                var errors = ModelStateUtil.GetErrors(ModelState).ToList();
                if (data.InvoiceId == Guid.Empty)
                {
                    errors.Add("Lütfen sayfayı yenileyip tekrar deneyiniz.");
                }
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }
            try
            {
                var invoice = _unitOfWork.Invoice.GetByIdWithCustomer(data.InvoiceId);

                 var mediaFileName = $"{StringHelpers.FormatForTag(invoice.Customer.User.Name+invoice.Customer.User.Surname+"-"+invoice.OnlinePaymentInfo.PaymentId)}";
                var mediaPath = await _storage.UploadFile(data.PostedFile.OpenReadStream(), $"{mediaFileName}.{Path.GetExtension(data.PostedFile.FileName.ToLower())}", "invoices");

                invoice.InvoicePdfUrl = mediaPath;
                _unitOfWork.Invoice.Update(invoice);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Dosya yüklenirken hata oluştu" }
                });
            }
        }


        [Route("admin/invoice/view")]
        public IActionResult ViewInvoice(Guid invoiceId)
        {
            try
            {
                string fullPath = string.Empty;
                var invoice = _unitOfWork.Invoice.GetById(invoiceId);
                fullPath = _storage.BlobUrl + invoice.InvoicePdfUrl;
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = fullPath
                });
            }
            catch (Exception)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Pdf Yüklenedi!" }
                }); ;
            }
            
        }

    }
}
