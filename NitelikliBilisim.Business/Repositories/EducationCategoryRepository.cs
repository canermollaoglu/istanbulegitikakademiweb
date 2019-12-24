using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationCategoryRepository : BaseRepository<EducationCategory, Guid>
    {
        public EducationCategoryRepository(NbDataContext context) : base(context)
        {
        }
    }
}
