using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator_certificate
{
    public class EducatorCertificateAddVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Sertifika adı alanı boş geçilemez.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Sertifika açıklaması alanı boş geçilemez.")]
        public string Description { get; set; }
        public _PostedFile CertificateImage { get; set; }
    }
    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }


}
