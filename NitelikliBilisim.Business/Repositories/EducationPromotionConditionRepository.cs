using NitelikliBilisim.Core.Entities.promotion;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPromotionConditionRepository :BaseRepository<EducationPromotionCondition,Guid>
    {
        private readonly NbDataContext _context;
        public EducationPromotionConditionRepository(NbDataContext context):base(context)
        {
            _context = context;
        }
    }
}
