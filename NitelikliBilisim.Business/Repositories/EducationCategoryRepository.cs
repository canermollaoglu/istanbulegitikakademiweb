using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Text;
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

        public List<SearchedEducationCategoryVm> GetSearchedEducationCategories(string searchText)
        {
            searchText = searchText.FormatForTag();

            var tags = _context.Bridge_EducationTags
                .Join(_context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
                {
                    TagId = x.Id,
                    EducationId = x.Id2,
                    TagName = y.Name
                })
                .ToList();

            var educationIds = tags
                .Where(x => x.TagName.Contains(searchText))
                .Select(x => x.EducationId)
                .ToList();

            var educations = _context.Educations
                .Where(x => educationIds.Contains(x.Id) && x.IsActive)
                .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
                {
                    Education = x,
                    EducationPreviewMedia = y
                })
                .Join(_context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
                {
                    Education = x.Education,
                    EducationPreviewMedia = x.EducationPreviewMedia,
                    CategoryName = y.Name
                });

            var model = educations
                .GroupBy(x => x.Education.Category.Name)
                .Select(x => new SearchedEducationCategoryVm()
                {
                    name = x.Key,
                    count = x.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            return model;
        }

        public override Guid Insert(EducationCategory entity, bool isSaveLater = false)
        {
            if (_context.EducationCategories.Any(x => x.Name == entity.Name))
                return default;

            return base.Insert(entity, isSaveLater);
        }


    }
}
