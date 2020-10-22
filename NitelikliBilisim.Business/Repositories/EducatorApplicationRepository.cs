using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorApplicationRepository :BaseRepository<EducatorApplication,Guid>
    {
        private readonly NbDataContext _context;
        public EducatorApplicationRepository(NbDataContext context):base(context)
        {

        }

    }
}
