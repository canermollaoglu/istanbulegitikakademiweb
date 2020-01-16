using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Data;
using NitelikliBilisim.Enums;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>, IPageableEntity<Education>
    {
        public EducationRepository(NbDataContext context) : base(context)
        {
        }

        public PagedEntity<Education> GetPagedEntity(int page = 0, Expression<Func<Education, bool>> filter = null, int shownRecords = 15)
        {
            IQueryable<Education> dbSet = base._table;
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
            var educations = _table
                .OrderBy(o => o.Name)
                .Skip(page * shownRecords)
                .Take(shownRecords);

            var mediaCount = educations.Join(_context.EducationMedias, l => l.Id, r => r.EducationId, (x, y) => new
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
            var partCount = educations.Join(_context.EducationParts, l => l.Id, r => r.EducationId, (x, y) => new
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
            var gainCount = educations.Join(_context.EducationGains, l => l.Id, r => r.EducationId, (x, y) => new
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
            var educatorCount = educations.Join(_context.Bridge_EducationEducators, l => l.Id, r => r.Id, (x, y) => new
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

            var categories = educations.Join(_context.Bridge_EducationTags, l => l.Id, r => r.Id2, (x, y) => new
            {
                EducationId = x.Id,
                CategoryId = y.Id
            }).Join(_context.EducationTags, l => l.CategoryId, r => r.Id, (x, y) => new
            {
                EducationId = x.EducationId,
                Category = y
            })
            .GroupBy(g => g.EducationId)
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
            if (_context.Educations.Any(x => x.Name == entity.Name))
                return null;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    entity.IsActive = false;
                    var educationId = base.Insert(entity);

                    foreach (var item in medias)
                        item.EducationId = educationId;

                    _context.EducationMedias.AddRange(medias);
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
                    if (!_context.EducationTags.Any(x => x.Name == tag))
                    {
                        autoTag = new EducationTag
                        {
                            Name = tag,
                            Description = $"{entity.Name} isimli eğitimin otomatik oluşturulmuş etiketi"
                        };
                        _context.EducationTags.Add(autoTag);
                    }
                    if (autoTag != null)
                        bridge.Add(new Bridge_EducationTag
                        {
                            Id = autoTag.Id,
                            Id2 = educationId
                        });
                    _context.Bridge_EducationTags.AddRange(bridge);

                    _context.SaveChanges();
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
            var data = _context.Bridge_EducationTags.Where(x => x.Id2 == educationId)
                .Join(_context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
                {
                    Category = y
                }).ToList();
            var categories = new List<EducationTag>();
            foreach (var item in data)
                categories.Add(new EducationTag
                {
                    Id = item.Category.Id,
                    BaseTagId = item.Category.BaseTagId,
                    Description = item.Category.Description,
                    Name = item.Category.Name
                });

            return categories;
        }

        public bool CheckEducationState(Guid educationId)
        {
            var mediaCount = _context.EducationMedias.Count(x => x.EducationId == educationId &&
            (x.MediaType == Core.Enums.EducationMediaType.Banner ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewPhoto ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewVideo));

            var partCount = _context.EducationParts.Count(x => x.EducationId == educationId);

            var gainCount = _context.EducationGains.Count(x => x.EducationId == educationId);

            var categoryCount = _context.Bridge_EducationTags.Count(x => x.Id2 == educationId);

            var education = _context.Educations.First(x => x.Id == educationId);

            education.IsActive = mediaCount >= 2 && partCount > 0 && gainCount > 0 && categoryCount > 0;

            _context.SaveChanges();

            return education.IsActive;
        }

        public int Update(Education entity, List<Guid> categoryIds, bool isSaveLater = false)
        {
            var currentCategories = _context.Bridge_EducationTags.Where(x => x.Id2 == entity.Id).ToList();

            _context.Bridge_EducationTags.RemoveRange(currentCategories);

            var newItems = new List<Bridge_EducationTag>();
            foreach (var categoryId in categoryIds)
                newItems.Add(new Bridge_EducationTag
                {
                    Id = categoryId,
                    Id2 = entity.Id
                });

            _context.Bridge_EducationTags.AddRange(newItems);

            return base.Update(entity, isSaveLater);
        }

        public List<EducationVm> GetInfiniteScrollSearchResults(string searchText, int page = 0)
        {
            var shownResults = 5;
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
                })
                .OrderByDescending(o => o.Education.CreatedDate)
                .Skip(page * shownResults)
                .Take(shownResults)
                .ToList();

            var data = educations.Select(x => new EducationVm
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
            var education = _context.Educations.FirstOrDefault(x => x.Id == id);
            var model = new EducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = education.Id,
                    Name = education.Name,
                    CategoryName = _context.EducationCategories.FirstOrDefault(x => x.Id == education.CategoryId).Name,
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
                Gains = _context.EducationGains.Where(x => x.EducationId == id)
                .Select(x => new EducationGainVm
                {
                    EducationId = id,
                    Gain = x.Gain
                }).ToList(),
                Medias = _context.EducationMedias.Where(x => x.EducationId == id)
                .Select(x => new EducationMediaVm
                {
                    EducationId = id,
                    FileUrl = x.FileUrl,
                    MediaType = x.MediaType
                }).ToList()
            };

            var parts = _context.EducationParts.Where(x => x.EducationId == id)
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
        public bool IsUnique(string name)
        {
            return !_context.Educations.Any(x => x.Name == name);
        }
    }
}
