using Nest;
using NitelikliBilisim.Core.Entities.campaign;
using NitelikliBilisim.Core.ESOptions;
using NitelikliBilisim.Core.ESOptions.ESEntities;
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
        private readonly IElasticClient _elasticClient;
        public CampaignRepository(NbDataContext context, IElasticClient elasticClient) : base(context)
        {
            _context = context;
            _elasticClient = elasticClient;
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

            var count = _elasticClient.Count<CampaignLog>(s =>
             s.Query(
                 q =>
                 q.Term(t => t.CampaignName, campaignName)));

            var result = _elasticClient.Search<CampaignLog>(s =>
            s.Size((int)count.Count)
            .Query(
                q =>
                 q.Term(t => t.CampaignName, campaignName)));

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
               var dl= result.Documents.GroupBy(x => x.RefererUrl).Select(x => new CampaignDetailLog
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
