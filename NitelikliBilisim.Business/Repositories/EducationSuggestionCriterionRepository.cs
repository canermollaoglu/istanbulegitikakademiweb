using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationSuggestionCriterionRepository : BaseRepository<EducationSuggestionCriterion, Guid>
    {
        private readonly NbDataContext _context;
        public EducationSuggestionCriterionRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<EducationSuggestionCriterionGetListVm> GetByEducationId(Guid id)
        {
            List<EducationSuggestionCriterion> model = new List<EducationSuggestionCriterion>();
            model = _context.EducationSuggestionCriterions.OrderByDescending(x => x.CreatedDate).Where(x => x.EducationId == id).ToList();
            List<string> educationIds = new List<string>();
            foreach (var criterion in model)
            {
                if (criterion.CharValue!=null)
                {
                    educationIds.AddRange(JsonConvert.DeserializeObject<List<string>>(criterion.CharValue));
                }
            }
            var educations = _context.Educations.Where(x => educationIds.Contains(x.Id.ToString())).ToList();
            List<EducationSuggestionCriterionGetListVm> retVal = new List<EducationSuggestionCriterionGetListVm>();
            foreach (var criterion in model)
            {
                EducationSuggestionCriterionGetListVm vm = new EducationSuggestionCriterionGetListVm();
                vm.CriterionType = criterion.CriterionType;
                vm.CriterionTypeName = EnumHelpers.GetDescription(criterion.CriterionType);
                vm.Id = criterion.Id;
                vm.MinValue = criterion.MinValue;
                vm.MaxValue = criterion.MaxValue;
                if (!string.IsNullOrEmpty(criterion.CharValue) && vm.CriterionType == CriterionType.WishListEducations || vm.CriterionType == CriterionType.PurchasedEducations)
                {
                    List<Guid> educationId = JsonConvert.DeserializeObject<List<Guid>>(criterion.CharValue);
                    vm.CharValue = string.Join(",", educations.Where(x => educationId.Contains(x.Id)).Select(x => x.Name).ToList());
                }
                retVal.Add(vm);
            }
            return retVal;
        }

        public IQueryable<EducationSuggestionCriterionListVm> GetQueryableWithDayCriterion()
        {
            return from education in _context.Educations.Include(x => x.Category)
                       join criterion in _context.EducationSuggestionCriterions on
                       new { EducationId = education.Id, CriterionType = CriterionType.EducationDay } equals
                       new { EducationId = criterion.EducationId, CriterionType = criterion.CriterionType } into ps
                       from criterion in ps.DefaultIfEmpty()
                       select new EducationSuggestionCriterionListVm
                       {
                           Id=education.Id,
                           CategoryName = education.Category.Name,
                           EducationName = education.Name,
                           MinValue = criterion != null ? criterion.MinValue : null,
                           MaxValue = criterion != null ? criterion.MaxValue : null,
                           IsActive = education.IsActive,
                       };
        }
    }
}
