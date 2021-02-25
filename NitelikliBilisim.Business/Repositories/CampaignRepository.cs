using NitelikliBilisim.Core.Entities.campaign;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class CampaignRepository : BaseRepository<Campaign, Guid>
    {
        private readonly NbDataContext _context;
        public CampaignRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
