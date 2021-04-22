using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("PopularTopics")]
    public class PopularTopic : BaseEntity<Guid>
    {
        public PopularTopic()
        {
            Id = Guid.NewGuid();
        }

        public string ShortTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string BackgroundUrl { get; set; }
        public string TargetUrl { get; set; }
    }
}
