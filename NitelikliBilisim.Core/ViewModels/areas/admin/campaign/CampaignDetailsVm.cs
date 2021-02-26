using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.campaign
{
    public class CampaignDetailsVm
    {
        public Guid Id { get; set; }
        public string CampaignName { get; set; }
        public string CampaignUrl { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public List<CampaignDetailLog> Details { get; set; }
    }
}
