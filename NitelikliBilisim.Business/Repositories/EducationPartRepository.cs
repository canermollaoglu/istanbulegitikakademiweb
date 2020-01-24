using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPartRepository : BaseRepository<EducationPart, Guid>
    {
        public EducationPartRepository(NbDataContext context) : base(context)
        {
        }

        public bool HasSubParts(Guid partId)
        {
            return Context.EducationParts.Any(x => x.BasePartId == partId);
        }

        public bool IsBasePart(Guid partId)
        {
            var part = Context.EducationParts.Find(partId);
            return part.BasePartId == null;
        }
    }
}
