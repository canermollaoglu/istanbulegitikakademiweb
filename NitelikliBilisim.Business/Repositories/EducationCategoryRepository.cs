using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using NitelikliBilisim.Core.ViewModels.Main.Home;
using NitelikliBilisim.Data;
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

        public EducationCategory GetCategoryBySeoUrl(string catSeoUrl)
        {
            var category = _context.EducationCategories.Include(x=>x.BaseCategory).FirstOrDefault(x => x.SeoUrl == catSeoUrl);
            return category;
        }

        public List<CoursesPageEducationCategoryVm> GetCoursesPageCategories()
        {
            List<CoursesPageEducationCategoryVm> model = new List<CoursesPageEducationCategoryVm>();
            var baseCategories = _context.EducationCategories.Where(x => !x.BaseCategoryId.HasValue).OrderBy(x=>x.Order).ToList();

                foreach (var baseCategory in baseCategories)
            {
                var educationCount = (from education in Context.Educations
                                      join eGroup in Context.EducationGroups on education.Id equals eGroup.EducationId
                                      join cat in Context.EducationCategories on education.CategoryId equals cat.Id
                                      join bCat in Context.EducationCategories on cat.BaseCategoryId equals bCat.Id
                                      where bCat.Id == baseCategory.Id && eGroup.StartDate.Date > DateTime.Now.Date && education.IsActive && eGroup.IsGroupOpenForAssignment
                                      select education).Count();
                var category = new CoursesPageEducationCategoryVm();
                category.Id = baseCategory.Id;
                category.SeoUrl = baseCategory.SeoUrl;
                category.Name = baseCategory.Name;category.EducationCount = educationCount;
                model.Add(category);
            }
            return model;
        }

        public List<EducationCategory> GetSubCategories()
        {
            return _context.EducationCategories.Where(x => x.BaseCategoryId.HasValue).ToList();
        }

        public override Guid Insert(EducationCategory entity, bool isSaveLater = false)
        {
            if (Context.EducationCategories.Any(x => x.Name == entity.Name))
                return default;

            return base.Insert(entity, isSaveLater);
        }

        public List<HomePageCategoryVm> GetNBUYEducationCategories()
        {
            List<HomePageCategoryVm> model = new();
            var dictionary = new Dictionary<Guid, int>();
            var baseCategories = _context.EducationCategories.Where(x => x.BaseCategoryId == null && x.CategoryType == CategoryType.NBUY).ToList();

            foreach (var baseCategory in baseCategories)
            {
                dictionary.Add(baseCategory.Id, 0);
                var subCategories = _context.EducationCategories.Where(x => x.BaseCategoryId == baseCategory.Id).Select(x => x.Id).ToList();

                var categoryEducations = _context.EducationCategories.Where(x => subCategories.Contains(x.Id))
                    .Join(_context.Educations, r => r.Id, l => l.CategoryId, (x, y) => new
                    {
                        Category = x,
                        Education = y
                    }).Where(x=>x.Education.IsActive)
                    .ToList()
                    .GroupBy(x => x.Category)
                    .Select(x => new
                    {
                        Name = x.Key.Name,
                        Count = x.Count()
                    }).ToList();

                foreach (var item in categoryEducations)
                    dictionary[baseCategory.Id] += item.Count;
            }

            foreach (var baseCategory in dictionary)
            {
                var c = baseCategories.First(x => x.Id == baseCategory.Key);
                model.Add(new HomePageCategoryVm
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconColor = c.IconColor,
                    IconUrl = c.IconUrl,
                    EducationCount = baseCategory.Value,
                    SeoUrl =c.SeoUrl
                });
            }

            return model;
        }

        public IQueryable<EducationCategory> GetBaseCategoryListQueryable()
        {
            return Context.EducationCategories.Where(x=>x.BaseCategoryId == null);
        }
        public IQueryable<EducationCategory> GetCategoriesByBaseCategoryId(Guid baseCategoryId)
        {
            return Context.EducationCategories.Where(x => x.BaseCategoryId == baseCategoryId);

        }

        public Dictionary<Guid,string> GetEducationCategoryDictionary()
        {
            var categories = _context.EducationCategories.Where(x => x.BaseCategoryId != null).ToDictionary(x => x.Id, x => x.Name);
            return categories;
        }

        //public List<PopularCategoryVm> GetPopularCategories()
        //{
        //    return _context.EducationCategories.Where(x => x.BaseCategoryId != null
        //    && !string.IsNullOrEmpty(x.BackgroundImageUrl)
        //    && !string.IsNullOrEmpty(x.IconUrl)).Select(x=> new PopularCategoryVm { 
        //    Id = x.Id,
        //    Name = x.Name,
        //    Description = x.Description2,
        //    IconUrl = x.IconUrl,
        //    BackgroundImageUrl = x.BackgroundImageUrl,
        //    SeoUrl = x.SeoUrl
        //    }).ToList();
        //}
    }

    public class _EducationCountByCategory
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
