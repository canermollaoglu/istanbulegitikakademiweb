using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostRepository : BaseRepository<EducationHost, Guid>
    {
        public EducationHostRepository(NbDataContext context) : base(context)
        {

        }
    }
}
