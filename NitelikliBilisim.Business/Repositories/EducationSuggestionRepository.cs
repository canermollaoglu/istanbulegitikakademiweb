using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class Query_1
    {
        public SubQuery_1 Education { get; set; }
    }
    public class SubQuery_1
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public byte Days { get; set; }
        public byte HoursPerDay { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public decimal? Price { get; set; }
        public EducationLevel Level { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PreviewPhotoUrl { get; set; }
    }
    public class EducationSuggestionRepository : BaseRepository<Suggestion, Guid>
    {
        public EducationSuggestionRepository(NbDataContext context) : base(context)
        {

        }

        public GetSuggestionsVm GetSuggestionsVm()
        {
            var data = Context.Suggestions
                .Include(x => x.Category)
                .Select(x => new _Suggestion
                {
                    Id = x.Id,
                    CategoryName = x.Category.Name,
                    Min = x.RangeMin,
                    Max = x.RangeMax
                }).OrderBy(x => x.CategoryName).ThenBy(x => x.Min).ToList();

            var model = new GetSuggestionsVm
            {
                Suggestions = data
            };
            return model;
        }

        public Guid Insert(Suggestion entity, List<Guid> suggestableEducations, bool isSaveLater = false)
        {
            if (suggestableEducations.Count == 0)
                return default;

            var serialized = JsonConvert.SerializeObject(suggestableEducations);
            entity.SuggestableEducations = serialized;

            return base.Insert(entity, isSaveLater);
        }

        public List<EducationVm> SuggestEducationsForHomeIndex(bool isLoggedIn, string userId)
        {
            var query = Context.Educations.Where(x => x.IsActive)
                .Join(Context.EducationCategories, l => l.CategoryId, r => r.Id, (education, category) => new
                {
                    Education = new
                    {
                        Id = education.Id,
                        Name = education.Name,
                        CategoryName = category.Name,
                        Days = education.Days,
                        HoursPerDay = education.HoursPerDay,
                        Description = education.Description,
                        Description2 = education.Description2,
                        Price = education.NewPrice,
                        Level = education.Level,
                        CreatedDate = education.CreatedDate
                    }
                })
                .Join(Context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Education.Id, r => r.EducationId, (education, media) => new Query_1
                {
                    Education = new SubQuery_1
                    {
                        Id = education.Education.Id,
                        Name = education.Education.Name,
                        CategoryName = education.Education.Name,
                        Days = education.Education.Days,
                        HoursPerDay = education.Education.HoursPerDay,
                        Description = education.Education.Description,
                        Description2 = education.Education.Description2,
                        Price = education.Education.Price,
                        Level = education.Education.Level,
                        CreatedDate = education.Education.CreatedDate,
                        PreviewPhotoUrl = media.FileUrl
                    }
                })
                .Select(x => x.Education);

            List<EducationVm> model;

            if (isLoggedIn)
            {
                var customer = Context.Customers.FirstOrDefault(x => x.Id == userId);
                var isNbuy = false;
                if (customer != null)
                    isNbuy = customer.IsNbuyStudent;

                model = isNbuy ? CustomSuggestions(query, userId) : DefaultSuggestions(query);
            }
            else
                model = DefaultSuggestions(query);

            return model;
        }

        private List<EducationVm> DefaultSuggestions(IQueryable<SubQuery_1> query)
        {
            var model = new List<EducationVm>();
            var data = query
                        .OrderByDescending(o => o.CreatedDate)
                        .Take(5)
                        .ToList();

            foreach (var item in data)
            {
                model.Add(new EducationVm
                {
                    Base = new EducationBaseVm
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CategoryName = item.CategoryName,
                        DaysNumeric = item.Days,
                        HoursPerDayNumeric = item.HoursPerDay,
                        DaysText = item.Days.ToString(),
                        HoursPerDayText = item.HoursPerDay.ToString(),
                        Level = EnumSupport.GetDescription(item.Level),
                        PriceNumeric = item.Price.GetValueOrDefault(0),
                        PriceText = item.Price.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                        Description = item.Description
                    },
                    Medias = new List<EducationMediaVm>
                            {
                            new EducationMediaVm { EducationId = item.Id, FileUrl = item.PreviewPhotoUrl, MediaType = EducationMediaType.PreviewPhoto }
                            }
                });
            }
            return model;
        }
        private List<EducationVm> CustomSuggestions(IQueryable<SubQuery_1> query, string userId)
        {
            var studentEducationInfo = Context.StudentEducationInfos.First(x => x.CustomerId == userId);
            var daysPassed = DateTime.Now.Subtract(studentEducationInfo.StartedAt).Days;

            if (daysPassed > 100)
                daysPassed = 100;

            var suggestedEducation = Context.Suggestions.FirstOrDefault(x => x.CategoryId == studentEducationInfo.CategoryId && (daysPassed > x.RangeMin && daysPassed <= x.RangeMax));
            if (suggestedEducation == null)
                suggestedEducation = Context.Suggestions.FirstOrDefault(x => x.CategoryId == studentEducationInfo.CategoryId);

            if (suggestedEducation == null)
                return DefaultSuggestions(query);

            var educationIds = JsonConvert.DeserializeObject<List<Guid>>(suggestedEducation.SuggestableEducations);
            var data = query.ToList();
            var count = 1;
            var model = new List<EducationVm>();
            foreach (var item in data)
            {
                if (educationIds.Contains(item.Id))
                {
                    model.Add(new EducationVm
                    {
                        Base = new EducationBaseVm
                        {
                            Id = item.Id,
                            Name = item.Name,
                            CategoryName = item.CategoryName,
                            DaysNumeric = item.Days,
                            HoursPerDayNumeric = item.HoursPerDay,
                            DaysText = item.Days.ToString(),
                            HoursPerDayText = item.HoursPerDay.ToString(),
                            Level = EnumSupport.GetDescription(item.Level),
                            PriceNumeric = item.Price.GetValueOrDefault(0),
                            PriceText = item.Price.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                            Description = item.Description
                        },
                        Medias = new List<EducationMediaVm>
                                {
                            new EducationMediaVm { EducationId = item.Id, FileUrl = item.PreviewPhotoUrl, MediaType = EducationMediaType.PreviewPhoto }
                                }
                    });
                    count++;
                }

                if (count >= 5)
                    break;
            }

            return model;
        }
    }
}
