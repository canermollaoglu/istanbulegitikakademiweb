using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationTagRepository : BaseRepository<EducationTag, Guid>
    {
        public EducationTagRepository(NbDataContext context) : base(context)
        {
        }

        public override Guid Insert(EducationTag entity, bool isSaveLater = false)
        {
            if (Context.EducationTags.Any(x => x.Name == entity.Name))
                return default;

            return base.Insert(entity, isSaveLater);
        }
    }
}
