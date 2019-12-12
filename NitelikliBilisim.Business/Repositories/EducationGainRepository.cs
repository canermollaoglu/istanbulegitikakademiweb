using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGainRepository : BaseRepository<EducationGain, Guid>
    {
        public EducationGainRepository(NbDataContext context) : base(context)
        {
        }
    }
}
