using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPartRepository : BaseRepository<EducationPart, Guid>
    {
        public EducationPartRepository(NbDataContext context) : base(context)
        {
        }

        public bool HasSubParts(Guid partId)
        {
            return _context.EducationParts.Any(x => x.BasePartId == partId);
        }

        public bool IsBasePart(Guid partId)
        {
            var part = _context.EducationParts.Find(partId);
            return part.BasePartId == null;
        }
    }
}
