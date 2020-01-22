using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGroupRepository : BaseRepository<EducationGroup, Guid>
    {
        public EducationGroupRepository(NbDataContext context) : base(context)
        {
        }
    }
}
