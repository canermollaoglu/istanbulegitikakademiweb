using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationMediaItemRepository : BaseRepository<EducationMedia, Guid>
    {
        public EducationMediaItemRepository(NbDataContext context) : base(context)
        {
        }
    }
}
