using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGainRepository : BaseRepository<EducationGain, Guid>
    {
        public EducationGainRepository(NbDataContext context) : base(context)
        {
        }
    }
}
