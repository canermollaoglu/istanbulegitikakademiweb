using Nest;
using System;

namespace NitelikliBilisim.Core.ESOptions.ESEntities
{
    public class CampaignLog
    {
        [Keyword]
        public Guid Id { get; set; }
        [Keyword]
        public string RefererUrl { get; set; }
        [Keyword]
        public string CampaignName { get; set; }
        [Keyword]
        public string IpAddress { get; set; }
    }
}
