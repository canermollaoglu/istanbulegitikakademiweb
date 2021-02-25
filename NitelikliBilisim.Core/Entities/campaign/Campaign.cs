using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.campaign
{
    [Table("Campaigns")]
    public class Campaign : BaseEntity<Guid>
    {
        public Campaign()
        {
            Id = Guid.NewGuid();
        }
        [Required, MaxLength(128)]
        public string CampaignName { get; set; }
        public string CampaignUrl { get; set; }
        public string Description { get; set; }

    }
}
