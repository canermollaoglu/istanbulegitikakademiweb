using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationSuggestionRepository : BaseRepository<Suggestion, Guid>
    {
        public EducationSuggestionRepository(NbDataContext context) : base(context)
        {

        }

        public GetSuggestionsVm GetSuggestionsVm()
        {
            var data = _context.Suggestions
                .Include(x => x.Category)
                .Select(x => new _Suggestion
                {
                    Id = x.Id,
                    CategoryName = x.Category.Name,
                    Min = x.RangeMin,
                    Max = x.RangeMax
                }).ToList();

            var model = new GetSuggestionsVm
            {
                Suggestions = data
            };
            return model;
        }
    }
}
