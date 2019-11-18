using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducatorSocialMedias")]
    public class EducatorSocialMedia : BaseEntity<int>
    {
        public string EducatorId { get; set; }
        [ForeignKey("EducatorId")]
        public virtual Educator Educator { get; set; }
        public EducatorSocialMediaType SocialMediaType { get; set; }
    }
}
