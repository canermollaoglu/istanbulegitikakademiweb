using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("Bridge_EducatorCertificates")]
    public class Bridge_EducatorCertificate : BaseEntity2<string,int>
    {
        [ForeignKey("Id")]
        public virtual Educator Educator { get; set; }
        [ForeignKey("Id2")]
        public virtual EducatorCertificate EducatorCertificate { get; set; }
    }
}
