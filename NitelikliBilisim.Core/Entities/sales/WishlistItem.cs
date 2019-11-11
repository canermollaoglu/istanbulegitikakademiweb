using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Wishlist")]
    public class WishlistItem : BaseEntity2<string, Guid>
    {
        [ForeignKey("Id")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("Id2")]
        public virtual Education Education { get; set; }
    }
}
