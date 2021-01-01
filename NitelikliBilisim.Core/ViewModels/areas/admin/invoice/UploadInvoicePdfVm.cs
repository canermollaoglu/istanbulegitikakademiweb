using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.invoice
{
    public class UploadInvoicePdfVm
    {
        [Required(ErrorMessage = "Lütfen sayfayı yenileyerek tekrar deneyiniz.")]
        public Guid InvoiceId { get; set; }
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public IFormFile PostedFile { get; set; }
    }
    
}
