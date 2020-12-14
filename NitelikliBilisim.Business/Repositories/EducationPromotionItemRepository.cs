using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPromotionItemRepository: BaseRepository<EducationPromotionItem, Guid>
    {
        private readonly NbDataContext _context;
        public EducationPromotionItemRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
