using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>, IPageableEntity<Education>
    {
        public EducationRepository(NbDataContext context) : base(context)
        {
        }

        public PagedEntity<Education> GetPagedEntity(int page = 0, Expression<Func<Education, bool>> filter = null, int shownRecords = 15)
        {
            IQueryable<Education> dbSet = base.Table;
            if (filter != null)
                dbSet = dbSet.Where(filter);

            return new PagedEntity<Education>
            {
                Data = dbSet.OrderBy(o => o.Name)
                    .Skip(page * shownRecords)
                    .Take(shownRecords)
                    .AsNoTracking()
                    .ToList(),
                Count = dbSet.Count()
            };
        }

        public ListGetVm GetPagedEducations(int page = 0, int shownRecords = 15)
        {
            var educations = Table
                .OrderBy(o => o.Name)
                .Skip(page * shownRecords)
                .Take(shownRecords);

            var mediaCount = educations.Join(Context.EducationMedias, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                MediaId = y.Id
            })
            .GroupBy(g => g.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();
            var partCount = educations.Join(Context.EducationParts, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                PartId = y.Id
            })
            .GroupBy(g => g.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();
            var gainCount = educations.Join(Context.EducationGains, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                GainId = y.Id
            })
            .GroupBy(x => x.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();
            var educatorCount = educations.Join(Context.Bridge_EducationEducators, l => l.Id, r => r.Id, (x, y) => new
            {
                EducationId = x.Id,
                EducatorId = y.Id2
            })
            .GroupBy(x => x.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();

            var fetchedCategories = educations.Join(Context.Bridge_EducationTags, l => l.Id, r => r.Id2, (x, y) => new
            {
                EducationId = x.Id,
                CategoryId = y.Id
            }).Join(Context.EducationTags, l => l.CategoryId, r => r.Id, (x, y) => new
            {
                EducationId = x.EducationId,
                Category = y
            }).ToList();
            var categories = fetchedCategories.GroupBy(g => g.EducationId)
            .Select(x => new
            {
                EducationId = x.Key,
                Data = x.ToList()
            });

            var educationCategories = new List<_EducationCategory>();
            foreach (var item in categories)
            {
                var concattedCategories = item.Data.Count != 0 ? "" : null;
                for (int i = 0; i < item.Data.Count; i++)
                {
                    if (i != item.Data.Count - 1)
                        concattedCategories += $"{item.Data[i].Category.Name}, ";
                    else
                        concattedCategories += item.Data[i].Category.Name;
                }

                educationCategories.Add(new _EducationCategory { EducationId = item.EducationId, ConcattedCategories = concattedCategories });
            }

            var educationDtos = new List<EducationDto>();
            //var mapper = new Mapper.Mapper<Education, EducationDto>();
            //foreach (var item in educations)
            //    educationDtos.Add(mapper.Map(item));

            foreach (var item in educations)
                educationDtos.Add(new EducationDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Days = item.Days,
                    HoursPerDay = item.HoursPerDay,
                    Level = EnumSupport.GetDescription(item.Level),
                    NewPrice = item.NewPrice,
                    IsActive = item.IsActive
                });

            return new ListGetVm
            {
                Educations = educationDtos,
                Medias = mediaCount,
                Parts = partCount,
                Gains = gainCount,
                Educators = educatorCount,
                EducationCategories = educationCategories
            };
        }

        public Guid? Insert(Education entity, List<Guid> tagIds, List<EducationMedia> medias, bool isSaveLater = false)
        {
            if (tagIds == null || tagIds.Count == 0)
                return null;
            if (Context.Educations.Any(x => x.Name == entity.Name))
                return null;

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    entity.IsActive = false;
                    var educationId = base.Insert(entity);

                    foreach (var item in medias)
                        item.EducationId = educationId;

                    Context.EducationMedias.AddRange(medias);
                    //_context.SaveChanges();

                    var bridge = new List<Bridge_EducationTag>();
                    foreach (var tagId in tagIds)
                        bridge.Add(new Bridge_EducationTag
                        {
                            Id = tagId,
                            Id2 = educationId
                        });
                    var tag = entity.Name.FormatForTag();
                    EducationTag autoTag = null;
                    if (!Context.EducationTags.Any(x => x.Name == tag))
                    {
                        autoTag = new EducationTag
                        {
                            Name = tag,
                            Description = $"{entity.Name} isimli eğitimin otomatik oluşturulmuş etiketi"
                        };
                        Context.EducationTags.Add(autoTag);
                    }
                    if (autoTag != null)
                        bridge.Add(new Bridge_EducationTag
                        {
                            Id = autoTag.Id,
                            Id2 = educationId
                        });
                    Context.Bridge_EducationTags.AddRange(bridge);

                    Context.SaveChanges();
                    transaction.Commit();
                    return educationId;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public List<EducationTag> GetTags(Guid educationId)
        {
            var data = Context.Bridge_EducationTags.Where(x => x.Id2 == educationId)
                .Join(Context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
                {
                    Category = y
                }).ToList();
            var categories = new List<EducationTag>();
            foreach (var item in data)
                categories.Add(new EducationTag
                {
                    Id = item.Category.Id,
                    Description = item.Category.Description,
                    Name = item.Category.Name
                });

            return categories;
        }

        public bool CheckEducationState(Guid educationId)
        {
            var mediaCount = Context.EducationMedias.Count(x => x.EducationId == educationId &&
            (x.MediaType == Core.Enums.EducationMediaType.Banner ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewPhoto ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewVideo));

            var partCount = Context.EducationParts.Count(x => x.EducationId == educationId);

            var gainCount = Context.EducationGains.Count(x => x.EducationId == educationId);

            var categoryCount = Context.Bridge_EducationTags.Count(x => x.Id2 == educationId);

            var education = Context.Educations.First(x => x.Id == educationId);

            education.IsActive = mediaCount >= 2 && partCount > 0 && gainCount > 0 && categoryCount > 0;

            Context.SaveChanges();

            return education.IsActive;
        }

        public int Update(Education entity, List<Guid> categoryIds, bool isSaveLater = false)
        {
            var currentCategories = Context.Bridge_EducationTags.Where(x => x.Id2 == entity.Id).ToList();

            Context.Bridge_EducationTags.RemoveRange(currentCategories);

            var newItems = new List<Bridge_EducationTag>();
            foreach (var categoryId in categoryIds)
                newItems.Add(new Bridge_EducationTag
                {
                    Id = categoryId,
                    Id2 = entity.Id
                });

            Context.Bridge_EducationTags.AddRange(newItems);

            return base.Update(entity, isSaveLater);
        }

        public List<EducationVm> GetInfiniteScrollSearchResults(string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var shownResults = 5;
            searchText = searchText.FormatForTag();

            var tags = Context.Bridge_EducationTags
                .Join(Context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
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

            var educations = Context.Educations.Include(x => x.Category)
                .Where(x => educationIds.Contains(x.Id) && x.IsActive);

            if (filter.categories != null)
            {
                var categoryIds = Context.EducationCategories.Where(x => filter.categories.Contains(x.Name)).Select(x => x.Id).ToList();
                educations = educations.Where(x => categoryIds.Contains(x.CategoryId));
            }

            if (filter.levels != null)
                educations = educations.Where(x => filter.levels.Contains(x.Level));

            switch (order)
            {
                case OrderCriteria.Latest:
                    educations = educations.OrderByDescending(x => x.CreatedDate);
                    break;
                case OrderCriteria.Popular:
                    educations = educations.OrderByDescending(x => x.Level); // TODO: Şimdilik popülerliğe göre sıralanamadığı için seviyesine göre sıralandı.
                    break;
            }

            var educationsList = educations
                .Join(Context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
                {
                    Education = x,
                    EducationPreviewMedia = y
                })
                .Join(Context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
                {
                    Education = x.Education,
                    EducationPreviewMedia = x.EducationPreviewMedia,
                    CategoryName = y.Name
                })
                .Skip(page * shownResults)
                .Take(shownResults)
                .ToList();

            var data = educationsList.Select(x => new EducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = x.Education.Id,
                    Name = x.Education.Name,
                    Description = x.Education.Description,
                    CategoryName = x.CategoryName,
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    PriceText = x.Education.NewPrice.Value.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } }
            }).ToList();

            return data;
        }

        public List<EducationVm> GetEducationsByCategory(string category, int page = 0, OrderCriteria order = OrderCriteria.Latest)
        {
            var shownResults = 3;

            var educations = Context.Educations.Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                var categoryId = Context.EducationCategories.FirstOrDefault(x => x.Name.ToLower() == category.ToLower())?.Id;

                if(categoryId != null)
                    educations = educations.Where(x => x.CategoryId == categoryId.Value);
            }

            switch (order)
            {
                case OrderCriteria.Latest:
                    educations = educations.OrderByDescending(x => x.CreatedDate);
                    break;
                case OrderCriteria.Popular:
                    educations = educations.OrderByDescending(x => x.Level); // TODO: Şimdilik popülerliğe göre sıralanamadığı için seviyesine göre sıralandı.
                    break;
            }

            var educationsList = educations
                .Join(Context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
                {
                    Education = x,
                    EducationPreviewMedia = y
                })
                .Join(Context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
                {
                    Education = x.Education,
                    EducationPreviewMedia = x.EducationPreviewMedia,
                    CategoryName = y.Name
                })
                .Skip(page * shownResults)
                .Take(shownResults)
                .ToList();

            var data = educationsList.Select(x => new EducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = x.Education.Id,
                    Name = x.Education.Name,
                    Description = x.Education.Description,
                    CategoryName = x.CategoryName,
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    PriceText = x.Education.NewPrice.Value.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } }
            }).ToList();

            return data;
        }

        public EducationVm GetEducation(Guid id)
        {
            var education = Context.Educations.First(x => x.Id == id);
            var model = new EducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = education.Id,
                    Name = education.Name,
                    CategoryName = Context.EducationCategories.First(x => x.Id == education.CategoryId).Name,
                    DaysNumeric = education.Days,
                    DaysText = education.Days.ToString(),
                    HoursPerDayNumeric = education.HoursPerDay,
                    HoursPerDayText = education.HoursPerDay.ToString(),
                    Description = education.Description,
                    Description2 = education.Description2,
                    PriceNumeric = education.NewPrice.GetValueOrDefault(0),
                    Level = EnumSupport.GetDescription(education.Level),
                    PriceText = education.NewPrice.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
                },
                Gains = Context.EducationGains.Where(x => x.EducationId == id)
                .Select(x => new EducationGainVm
                {
                    EducationId = id,
                    Gain = x.Gain
                }).ToList(),
                Medias = Context.EducationMedias.Where(x => x.EducationId == id)
                .Select(x => new EducationMediaVm
                {
                    EducationId = id,
                    FileUrl = x.FileUrl,
                    MediaType = x.MediaType
                }).ToList()
            };

            var parts = Context.EducationParts.Where(x => x.EducationId == id)
                .OrderBy(o => o.Order)
                .ToList();

            var partVms = new List<EducationPartVm>();
            foreach (var item in parts.Where(x => x.BasePartId == null))
                partVms.Add(new EducationPartVm
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    BasePartId = item.BasePartId,
                    Title = item.Title,
                    Duration = parts.Where(x => x.BasePartId == item.Id).Sum(x => x.Duration),
                    Order = item.Order,
                    SubParts = parts.Where(y => y.BasePartId == item.Id).Select(z => new EducationPartVm
                    {
                        Id = z.Id,
                        EducationId = z.EducationId,
                        BasePartId = z.BasePartId,
                        Title = z.Title,
                        Order = z.Order,
                        Duration = z.Duration,
                        SubParts = null
                    }).ToList()
                });

            model.Parts = partVms;

            int totalPartCount = 0;
            int totalDurationCount = 0;
            foreach (var item in partVms)
            {
                if (item.SubParts != null)
                {
                    totalPartCount += item.SubParts.Count;
                    totalDurationCount += item.SubParts.Sum(x => x.Duration);
                }
            }
            model.TotalDuration = totalDurationCount;
            model.TotalPartCount = totalPartCount;

            return model;
        }

        public FilterOptionsVm GetEducationFilterOptions(string searchText)
        {
            searchText = searchText.FormatForTag();

            var tags = Context.Bridge_EducationTags
                .Join(Context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
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

            var educations = Context.Educations
                .Where(x => educationIds.Contains(x.Id) && x.IsActive)
                .Join(Context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
                {
                    Education = x,
                    EducationPreviewMedia = y
                })
                .Join(Context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
                {
                    Education = x.Education,
                    EducationPreviewMedia = x.EducationPreviewMedia,
                    CategoryName = y.Name
                });

            var filterOptions = new FilterOptionsVm();

            filterOptions.categories = educations
                .GroupBy(x => x.Education.Category.Name)
                .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
                .ToList()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            filterOptions.levels = educations
                .GroupBy(x => x.Education.Level)
                .Select(x => new KeyValuePair<EducationLevel, int>(x.Key, x.Count()))
                .ToList()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key.ToString(), x => x.Value);

            // TODO: locations ve stars

            return filterOptions;
        }

        public bool IsUnique(string name)
        {
            return !Context.Educations.Any(x => x.Name == name);
        }
    }
}
