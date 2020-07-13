using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationSuggestionCriterionRepository: BaseRepository<EducationSuggestionCriterion,Guid>
    {
        private readonly NbDataContext _context;
        public EducationSuggestionCriterionRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public List<EducationSuggestionCriterionGetListVm> GetByEducationId(Guid id)
        {
            return _context.EducationSuggestionCriterions.OrderByDescending(x=>x.CreatedDate).Where(x => x.EducationId == id).Select(x=> new EducationSuggestionCriterionGetListVm
            {
                CriterionTypeName = EnumSupport.GetDescription(x.CriterionType),
                Id = x.Id,
                MinValue = x.MinValue,
                MaxValue = x.MaxValue
            }).ToList();
        }

    }
}
