using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.blog
{
    [Table("Bridge_BlogTags")]
    public class Bridge_BlogPostTag:BaseEntity2<Guid,Guid>
    {
        [ForeignKey("Id")]
        public virtual BlogPost BlogPost { get; set; }
        [ForeignKey("Id2")]
        public virtual BlogTag BlogTag { get; set; }
    }
}
