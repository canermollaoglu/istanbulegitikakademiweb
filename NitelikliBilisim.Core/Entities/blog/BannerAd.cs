using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.blog
{
    [Table("BannerAds")]
    public class BannerAd : BaseEntity<Guid>
    {
        public BannerAd()
        {
            Id = Guid.NewGuid();
        }
        public string Code { get; set; }
        public string IconUrl { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string RelatedApplicationUrl { get; set; }

    }
}
