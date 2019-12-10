using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationMediaItemRepository : BaseRepository<EducationMedia, Guid>
    {
        public EducationMediaItemRepository(NbDataContext context) : base(context)
        {

        }
    }
}
