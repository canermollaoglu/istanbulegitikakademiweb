using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;
using NitelikliBilisim.Data;
using NitelikliBilisim.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var query = _context.Educations.Where(x => x.IsActive)
                .Join(_context.EducationCategories, l => l.CategoryId, r => r.Id, (education, category) => new
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
                .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Education.Id, r => r.EducationId, (education, media) => new
                {
                    Education = new
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

            var model = new List<EducationVm>();

            if (!isLoggedIn)
            {
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
            }
            else
            {
                var isNbuy = _context.Customers.FirstOrDefault(x => x.Id == userId).IsNbuyStudent;

                if (isNbuy)
                {
                    var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
                    var suggestedEducation = _context.Suggestions.FirstOrDefault(x => x.CategoryId == studentEducationInfo.CategoryId);
                    var educationIds = JsonConvert.DeserializeObject<List<Guid>>(suggestedEducation.SuggestableEducations);

                    var count = 1;
                    foreach (var item in query.ToList())
                    {
                        if (educationIds.Contains(item.Id))
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
                        if (count >= 5)
                            break;
                        count++;
                    }
                }
                else
                {
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
                }
            }

            return model;
        }
    }
}
