using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Core.Entities.campaign;
using NitelikliBilisim.Core.ViewModels.areas.admin.campaign;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CampaignRepository : BaseRepository<Campaign, Guid>
    {
        private readonly NbDataContext _context;
        private readonly CampaignLogRepository _campaignLogRepository;
        public CampaignRepository(NbDataContext context,CampaignLogRepository campaignLogRepository) : base(context)
        {
            _context = context;
            _campaignLogRepository = campaignLogRepository;
        }

        public CampaignDetailsVm GetCampaignDetails(Guid id)
        {
            var campaign = _context.Campaigns.First(x => x.Id == id);
            var retVal = new CampaignDetailsVm
            {
                Id = campaign.Id,
                CampaignName = campaign.CampaignName,
                CampaignUrl = campaign.CampaignUrl,
                Description = campaign.Description,
                CreatedDate = campaign.CreatedDate.ToShortDateString(),
                Details = GetCampaignDetailLogs(campaign.CampaignName)
            };
            return retVal;
        }

        private List<CampaignDetailLog> GetCampaignDetailLogs(string campaignName)
        {
            List<CampaignDetailLog> detailLogs = new();

            var campaignItems = _campaignLogRepository.GetList(x => x.CampaignName == campaignName);

            if (campaignItems!=null&& campaignItems.Count>0)
            {
               var dl= campaignItems.GroupBy(x => x.RefererUrl).Select(x => new CampaignDetailLog
                {
                    Source = x.Key,
                    Count = x.Count()
                });
                detailLogs.AddRange(dl);
            }
            return detailLogs;
        }



    }
}
