using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("FeaturedComments")]
    public class FeaturedComment : BaseEntity<Guid>
    {
        public FeaturedComment()
        {
            Id = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Yorum içeriği alanı zorunludur.")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Meslek alanı zorunludur.")]
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public string PreviewImageFileUrl { get; set; }

    }
}
