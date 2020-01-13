using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationCategoryRepository : BaseRepository<EducationCategory, Guid>
    {
        public EducationCategoryRepository(NbDataContext context) : base(context)
        {
        }

        public List<EducationCategory> GetDeepestCategories(CategoryType? categoryType = null)
        {
            var query = _context.EducationCategories
                .Where(x => x.BaseCategoryId != null);
            if (categoryType != null)
                query = query.Where(x => x.CategoryType == categoryType);

            var categories = query.OrderBy(o => o.Name).ToList();

            var deepestCategories = new List<EducationCategory>();
            foreach (var category in categories)
            {
                if (query.Any(x => x.BaseCategoryId == category.Id))
                    continue;

                deepestCategories.Add(category);
            }

            return deepestCategories;
        }
        public override Guid Insert(EducationCategory entity, bool isSaveLater = false)
        {
            if (_context.EducationCategories.Any(x => x.Name == entity.Name))
                return default;

            return base.Insert(entity, isSaveLater);
        }


    }
}
