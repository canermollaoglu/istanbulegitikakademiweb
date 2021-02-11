using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BannerAdsRepository : BaseRepository<BannerAd, Guid>
    {
        NbDataContext _context;
        public BannerAdsRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public BannerAd GetBannerByCode(string code)
        {
            return _context.BannerAds.FirstOrDefault(x => x.Code == code);
        }
    }
}
