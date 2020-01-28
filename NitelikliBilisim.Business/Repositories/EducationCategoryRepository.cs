﻿using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationCategoryRepository : BaseRepository<EducationCategory, Guid>
    {
        private readonly NbDataContext _context;
        public EducationCategoryRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<EducationCategory> GetDeepestCategories(CategoryType? categoryType = null)
        {
            var query = Context.EducationCategories
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
            if (Context.EducationCategories.Any(x => x.Name == entity.Name))
                return default;

            return base.Insert(entity, isSaveLater);
        }

        public Dictionary<string, int> GetEducationCountForCategories()
        {
            var dictionary = new Dictionary<string, int>();
            var baseCategories = _context.EducationCategories.Where(x => x.BaseCategoryId == null).ToList();

            foreach (var baseCategory in baseCategories)
            {
                dictionary.Add(baseCategory.Name, 0);
                var subCategories = _context.EducationCategories.Where(x => x.BaseCategoryId == baseCategory.Id).Select(x => x.Id).ToList();

                var categoryEducations = _context.EducationCategories.Where(x => subCategories.Contains(x.Id))
                    .Join(_context.Educations, r => r.Id, l => l.CategoryId, (x, y) => new
                    {
                        Category = x,
                        Education = y
                    }).ToList()
                    .GroupBy(x => x.Category)
                    .Select(x => new
                    {
                        Name = x.Key.Name,
                        Count = x.Count()
                    }).ToList();

                foreach (var item in categoryEducations)
                    dictionary[baseCategory.Name] += item.Count;
            }

            return dictionary;
        }
    }

    public class _EducationCountByCategory
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
