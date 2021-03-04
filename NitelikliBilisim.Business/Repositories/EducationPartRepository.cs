using Microsoft.EntityFrameworkCore;
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

        public byte GetPartOrder(Guid? basePartId,Guid educationId)
        {
            var parts = Context.EducationParts.Include(x => x.BasePart).ToList();
            if (!basePartId.HasValue)
            {
               var lastPart= parts.Where(x => x.BasePart == null && x.EducationId == educationId).OrderBy(x => x.Order).LastOrDefault();
                return (byte)(lastPart != null ? (byte)(lastPart.Order+1) : 1);
            }
            else
            {
                var childParts = parts.Where(x => x.BasePartId == basePartId).OrderBy(x => x.Order).LastOrDefault();
                return (byte)(childParts != null ? (byte)(childParts.Order+1) : 1);
            }

        }
    }
}
