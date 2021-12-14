using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class CorporateMembershipApplicationRepository : BaseRepository<CorporateMembershipApplication, Guid>
    {
        private readonly NbDataContext _context;
        public CorporateMembershipApplicationRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


    }
}
