using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostRepository : BaseRepository<EducationHost, Guid>
    {
        public EducationHostRepository(NbDataContext context) : base(context)
        {

        }
    }
}
