using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPartRepository : BaseRepository<EducationPart, Guid>
    {
        public EducationPartRepository(NbDataContext context) : base(context)
        {
        }
    }
}
