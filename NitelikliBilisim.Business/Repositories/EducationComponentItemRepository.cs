using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_component;
using NitelikliBilisim.Core.ViewModels.Main;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationComponentItemRepository :BaseRepository<EducationComponentItem,Guid>
    {
        private readonly NbDataContext _context;
        public EducationComponentItemRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<EducationComponentItemListVm> GetQueryableList()
        {
            var data = from componentItem in _context.EducationComponentItems
                join education in _context.Educations on componentItem.EducationId equals education.Id
                join category in _context.EducationCategories on education.CategoryId equals category.Id
                join baseCategory in _context.EducationCategories on category.BaseCategoryId equals baseCategory.Id
                select new EducationComponentItemListVm()
                {
                    Id = componentItem.Id,
                    EducationName = education.Name,
                    Category = category.Name,
                    BaseCategory = baseCategory.Name,
                    Order = componentItem.Order,
                    SuggestionType = componentItem.SuggestionType,
                    ComponentType = componentItem.ComponentType
                };
            return data;
        }
    }
}
