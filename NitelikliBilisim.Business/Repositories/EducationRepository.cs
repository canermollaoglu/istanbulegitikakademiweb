using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>, IPageableEntity<Education>
    {
        private readonly IConfiguration _configuration;

        public EducationRepository(NbDataContext context, IConfiguration configuration) : base(context)
        {
            _configuration = configuration;
        }

        public int TotalEducationCount()
        {
            return Context.Educations.Count(x => x.IsActive);
        }
        public bool CheckEducationBySeoUrl(string seoUrl)
        {
            return Context.Educations.Any(x => x.SeoUrl == seoUrl);
        }

        public IQueryable<EducationListVm> GetListQueryable()
        {
            return from e in Context.Educations
                   join c in Context.EducationCategories on e.CategoryId equals c.Id
                   select new EducationListVm
                   {
                       Id = e.Id,
                       Name = e.Name,
                       Description = e.Description,
                       CategoryName = c.Name,
                       Level = (int)e.Level,
                       Days = e.Days,
                       HoursPerDay = e.HoursPerDay,
                       isActive = e.IsActive
                   };
        }

        public bool CheckIsCanComment(string userId, Guid educationId)
        {
            var group = (from bridge in Context.Bridge_GroupStudents
                         join eGroup in Context.EducationGroups.Include(x => x.GroupLessonDays) on bridge.Id equals eGroup.Id
                         where bridge.Id2 == userId && eGroup.EducationId == educationId
                         select eGroup).FirstOrDefault();

            if (group == null)
                return false;
            if (group.GroupLessonDays == null || group.GroupLessonDays.Count == 0)
                return false;

            var lastDate = group.GroupLessonDays.OrderBy(x => x.DateOfLesson).Last().DateOfLesson;
            if (DateTime.Now.Date > lastDate)
            {
                return true;
            }
            return false;

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

        public List<EducationDto> GetPagedEducations(int page = 0, int shownRecords = 15)
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

            foreach (var item in educations)
                educationDtos.Add(new EducationDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Days = item.Days,
                    HoursPerDay = item.HoursPerDay,
                    Level = EnumHelpers.GetDescription(item.Level),
                    IsActive = item.IsActive,
                    MediaCount = mediaCount.Where(x => x.EducationId == item.Id).Sum(x => x.Count),
                    PartCount = partCount.Where(x => x.EducationId == item.Id).Sum(x => x.Count),
                    GainCount = gainCount.Where(x => x.EducationId == item.Id).Sum(x => x.Count),
                    EducatorCount = educatorCount.Where(x => x.EducationId == item.Id).Sum(x => x.Count),
                    EducationCategories = educationCategories.Count(x => x.EducationId == item.Id) > 0 ?
                        string.Join(", ", educationCategories.Where(x => x.EducationId == item.Id).Select(x => x.ConcattedCategories)) : "-"
                });

            return educationDtos;
        }

        public CoursesPagePagedListVm GetCoursesPageEducations(int? hostCity, int page, OrderCriteria order)
        {
            var model = new CoursesPagePagedListVm();
            var educations = Context.Educations.Include(x => x.Category);
            var rawData = (from education in educations
                           join eImage in Context.EducationMedias on education.Id equals eImage.EducationId
                           where
                           eImage.MediaType == EducationMediaType.PreviewPhoto && education.IsActive
                           select new CoursesPageEducationsVm
                           {
                               Id = education.Id,
                               Seo=education.SeoUrl,
                               CSeo = education.Category.SeoUrl,
                               CreatedDate = education.CreatedDate,
                               Name = education.Name,
                               ImagePath = eImage.FileUrl,
                               EducationDays = education.Days,
                               EducationHours = education.HoursPerDay * education.Days,
                               Description = education.Description,
                               CategoryId = education.Category.BaseCategoryId,
                               Level = education.Level
                           }).AsQueryable();

            if (hostCity.HasValue)
            {
                var city = (HostCity)hostCity;
                var educationIds = Context.EducationGroups.Include(x => x.Host).Where(x => x.Host.City == city && x.StartDate>DateTime.Now.Date).Select(x => x.EducationId).ToList();
                rawData = rawData.Where(x => educationIds.Contains(x.Id));
            }

            switch (order)
            {
                case OrderCriteria.Latest:
                    rawData = rawData.OrderByDescending(x => x.CreatedDate);
                    break;
                case OrderCriteria.Popular:
                    rawData = rawData.OrderByDescending(x => x.Level);
                    break;
                default:
                    rawData = rawData.OrderByDescending(x => x.CreatedDate);
                    break;
            }
            model.TotalCount = rawData.Count();
            model.TotalPageCount = (int)Math.Ceiling(rawData.Count() / (double)6);
            model.PageIndex = page;
            rawData = rawData.Skip((page-1) * 6).Take(6);
            model.Educations = rawData.ToList();
            return model;
        }

        public Guid? Insert(Education entity, string[] tags, List<EducationMedia> medias, bool isSaveLater = false)
        {
            if (tags == null || tags.Length == 0)
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
                    var dbTags = Context.EducationTags.ToList();
                    var bridge = new List<Bridge_EducationTag>();
                    foreach (var tagName in tags)
                    {
                        if (!dbTags.Any(x => x.Name == tagName))
                        {

                            var educationTag = new EducationTag { Name = tagName };
                            var model = Context.EducationTags.Add(educationTag);
                            Context.SaveChanges();
                            bridge.Add(new Bridge_EducationTag
                            {
                                Id = educationTag.Id,
                                Id2 = educationId
                            });
                        }
                        else
                        {
                            bridge.Add(new Bridge_EducationTag
                            {
                                Id = dbTags.First(x => x.Name == tagName).Id,
                                Id2 = educationId
                            });
                        }
                    }
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
            var hasBanner = Context.EducationMedias.Count(x => x.EducationId == educationId && x.MediaType == EducationMediaType.Banner) > 0;
            var hasPreview = Context.EducationMedias.Count(x => x.EducationId == educationId &&
                (x.MediaType == EducationMediaType.PreviewPhoto ||
                x.MediaType == EducationMediaType.PreviewVideo)) > 0;

            var partCount = Context.EducationParts.Count(x => x.EducationId == educationId);

            var gainCount = Context.EducationGains.Count(x => x.EducationId == educationId);

            var categoryCount = Context.Bridge_EducationTags.Count(x => x.Id2 == educationId);

            var education = Context.Educations.First(x => x.Id == educationId);

            education.IsActive = hasBanner && hasPreview && partCount > 0 && gainCount > 0 && categoryCount > 0;

            Context.SaveChanges();

            return education.IsActive;
        }

        public int Update(Education entity, string[] tags, bool isSaveLater = false)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var currentTags = Context.Bridge_EducationTags.Where(x => x.Id2 == entity.Id).ToList();
                    Context.Bridge_EducationTags.RemoveRange(currentTags);
                    Context.SaveChanges();
                    var educationId = base.Update(entity);
                    Context.SaveChanges();
                    var newItems = new List<Bridge_EducationTag>();
                    var dbTags = Context.EducationTags.ToList();
                    foreach (var tagName in tags)
                    {
                        if (!dbTags.Any(x => x.Name == tagName))
                        {

                            var educationTag = new EducationTag { Name = tagName };
                            var model = Context.EducationTags.Add(educationTag);
                            Context.SaveChanges();
                            newItems.Add(new Bridge_EducationTag
                            {
                                Id = educationTag.Id,
                                Id2 = entity.Id
                            });
                        }
                        else
                        {
                            newItems.Add(new Bridge_EducationTag
                            {
                                Id = dbTags.First(x => x.Name == tagName).Id,
                                Id2 = entity.Id
                            });
                        }
                    }
                    Context.Bridge_EducationTags.AddRange(newItems);
                    Context.SaveChanges();
                    transaction.Commit();
                    return educationId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

            }
        }

        public List<EducationVm> GetInfiniteScrollSearchResults(string categoryName, string searchText = "", int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var shownResults = 4;
            var educationIds = new List<Guid>();

            if (!string.IsNullOrEmpty(searchText))
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

                educationIds = tags
                    .Where(x => x.TagName.Contains(searchText))
                    .Select(x => x.EducationId)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(categoryName))
            {
                var educationCategories = Context.EducationCategories.Select(x => new
                {
                    Id = x.Id,
                    BaseCategoryId = x.BaseCategoryId,
                    Name = x.Name,
                });

                var baseCategoryId = educationCategories.FirstOrDefault(x => x.Name.ToLower() == categoryName.ToLower())?.Id;

                if (baseCategoryId != null)
                {
                    var educationsByCategory = Context.Educations.Join(educationCategories, e => e.CategoryId, c => c.Id, (e, c) => new
                    {
                        EducationId = e.Id,
                        CategoryId = c.Id
                    });

                    educationIds.AddRange(educationsByCategory.Where(x => x.CategoryId == baseCategoryId).Select(x => x.EducationId));

                    var subCategories = educationCategories.Where(x => x.BaseCategoryId == baseCategoryId).Select(x => x.Id).ToList();

                    foreach (var item in subCategories)
                        educationIds.AddRange(educationsByCategory.Where(x => x.CategoryId == item).Select(x => x.EducationId));
                }
            }
            else
                educationIds = Context.Educations.Select(x => x.Id).ToList();


            var educations = Context.Educations.Include(x => x.Category)
                .Where(x => educationIds.Contains(x.Id) && x.IsActive);

            //var educationGroupRepository = new EducationGroupRepository(Context);


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
                    Level = EnumHelpers.GetDescription(x.Education.Level),
                    //PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay,
                    //StartDateText = educationGroupRepository.GetFirstAvailableGroup(x.Education.Id)?.StartDate
                    //    .ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("tr-TR")) ?? "Açılan grup yok"
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } }
            }).ToList();

            return data;
        }

        public IQueryable<Education> GetAllEducationsWithCategory()
        {
            return Context.Educations.Include(x => x.Category);
        }

        public List<EducationVm> GetEducationsByCategory(string category, int page = 0, OrderCriteria order = OrderCriteria.Latest)
        {
            var shownResults = 3;

            var educations = Context.Educations.Where(x => x.IsActive);

            var preList = new List<Education>();

            if (!string.IsNullOrEmpty(category))
            {
                var baseCategoryId = Context.EducationCategories.FirstOrDefault(x => x.Name.ToLower() == category.ToLower())?.Id;

                if (baseCategoryId != null)
                {
                    preList.AddRange(educations.Where(x => x.CategoryId == baseCategoryId));

                    var subCategories = Context.EducationCategories.Where(x => x.BaseCategoryId == baseCategoryId).Select(x => x.Id).ToList();

                    foreach (var item in subCategories)
                        preList.AddRange(educations.Where(x => x.CategoryId == item));
                }
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

            var educationGroupRepository = new EducationGroupRepository(Context, _configuration);

            var query = string.IsNullOrEmpty(category) ? educations : preList.AsQueryable();

            var educationsList = query
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
                    Level = EnumHelpers.GetDescription(x.Education.Level),
                    //PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay,
                    StartDateText = educationGroupRepository.GetFirstAvailableGroups(x.Education.Id).FirstOrDefault()?.StartDate
                        .ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("tr-TR")) ?? "Açılan grup yok"
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } }
            }).ToList();

            return data;
        }

        public EducationVm GetEducation(string seoUrl)
        {
            var education = Context.Educations.First(x => x.SeoUrl == seoUrl);

            var comments = (from comment in Context.EducationComments
                            join commenter in Context.Users on comment.CommentatorId equals commenter.Id
                            join student in Context.Customers on commenter.Id equals student.Id
                            where comment.EducationId == education.Id && comment.ApprovalStatus == CommentApprovalStatus.Approved
                            orderby comment.ApprovalDate descending
                            select new CommentDetailVm
                            {
                                Point = comment.Points,
                                Commenter = $"{commenter.Name} {commenter.Surname}",
                                Date = comment.CreatedDate.ToString("dd MMMM yyyy"),
                                CommenterJob = student.Job,
                                Content = comment.Content
                            }).ToList();


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
                    IsWishListItem = false,
                    IsCanComment = false,
                    Comments = comments,
                    Point = 0,
                    PointText = "0.0",
                    CommentCount = comments.Count(),
                    //PriceNumeric = education.NewPrice.GetValueOrDefault(0),
                    Level = EnumHelpers.GetDescription(education.Level),
                    //PriceText = education.NewPrice.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
                },
                Gains = Context.EducationGains.Where(x => x.EducationId == education.Id)
                .Select(x => new EducationGainVm
                {
                    EducationId = education.Id,
                    Gain = x.Gain
                }).ToList(),
                Medias = Context.EducationMedias.Where(x => x.EducationId == education.Id)
                .Select(x => new EducationMediaVm
                {
                    EducationId = education.Id,
                    FileUrl = x.FileUrl,
                    MediaType = x.MediaType
                }).ToList()
            };
            if (comments.Count > 0)
            {
                model.Base.Point = (comments.Sum(x => x.Point) / (double)comments.Count());
                model.Base.PointText = (comments.Sum(x => x.Point) / comments.Count()).ToString("0.0");
            }

            var parts = Context.EducationParts.Where(x => x.EducationId == education.Id)
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

        public FilterOptionsVm GetEducationFilterOptions(string categoryName, string searchText, FiltersVm filter = null)
        {
            var educationIds = new List<Guid>();

            if (!string.IsNullOrEmpty(searchText))
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

                educationIds = tags
                    .Where(x => x.TagName.Contains(searchText))
                    .Select(x => x.EducationId)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(categoryName))
            {
                var educationCategories = Context.EducationCategories.Select(x => new
                {
                    Id = x.Id,
                    BaseCategoryId = x.BaseCategoryId,
                    Name = x.Name,
                });

                var baseCategoryId = educationCategories.FirstOrDefault(x => x.Name.ToLower() == categoryName.ToLower())?.Id;

                if (baseCategoryId != null)
                {
                    var educationsByCategory = Context.Educations.Join(educationCategories, e => e.CategoryId, c => c.Id, (e, c) => new
                    {
                        EducationId = e.Id,
                        CategoryId = c.Id
                    });

                    educationIds.AddRange(educationsByCategory.Where(x => x.CategoryId == baseCategoryId).Select(x => x.EducationId));

                    var subCategories = educationCategories.Where(x => x.BaseCategoryId == baseCategoryId).Select(x => x.Id).ToList();

                    foreach (var item in subCategories)
                        educationIds.AddRange(educationsByCategory.Where(x => x.CategoryId == item).Select(x => x.EducationId));
                }
            }
            else
                educationIds = Context.Educations.Select(x => x.Id).ToList();

            if (filter?.categories?.Length > 0)
                educationIds = Context.Educations.Where(x => filter.categories.Contains(x.Category.Name)).Select(x => x.Id).ToList();

            var educations = Context.Educations
                .Where(x => educationIds.Contains(x.Id) && x.IsActive)
                .Select(x => new
                {
                    Level = x.Level,
                    CategoryName = x.Name,
                    CategoryId = x.CategoryId
                })
                .Join(Context.EducationCategories, l => l.CategoryId, r => r.Id, (x, y) => new
                {
                    Level = x.Level,
                    CategoryName = y.Name,
                    BaseCategoryName = y.BaseCategoryId.HasValue ? y.BaseCategory.Name : ""
                });

            var filterOptions = new FilterOptionsVm
            {
                categories = educations.AsEnumerable()
                    .GroupBy(x => x.CategoryName)
                    .Select(x => new CategoryOptionVm()
                    {
                        BaseCategoryName = x.First().BaseCategoryName,
                        CategoryName = x.Key,
                        Count = x.Count()
                    })
                    .ToList()
                    .OrderBy(x => x.BaseCategoryName)
                    .ThenByDescending(x => x.Count)
                    .ToList()
            };

            var baseCategories = filterOptions.categories.GroupBy(x => x.BaseCategoryName)
                .Select(x => new CategoryOptionVm()
                {
                    BaseCategoryName = "",
                    CategoryName = x.Key,
                    Count = x.Sum(y => y.Count)
                }).ToList();

            filterOptions.categories.InsertRange(0, baseCategories);

            filterOptions.levels = educations
                .GroupBy(x => x.Level)
                .Select(x => new KeyValuePair<EducationLevel, int>(x.Key, x.Count()))
                .ToList()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key.ToString(), x => x.Value);

            // TODO: locations ve stars
            return filterOptions;
        }


        /// <summary>
        /// Kullanıcı tarafından satın alınmış eğitimlere ve bu eğitimlere ait kategorilere ait Id leri döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Dictionary<CategoryId,EducationId></returns>
        public List<PurchasedEducationVm> GetPurchasedEducationsByUserId(string userId)
        {
            return (from ticket in Context.Tickets
                    join education in Context.Educations on ticket.EducationId equals education.Id
                    join category in Context.EducationCategories on education.CategoryId equals category.Id
                    where ticket.OwnerId == userId
                    select new PurchasedEducationVm
                    {
                        EducationId = education.Id,
                        CategoryId = category.Id
                    }).ToList();
        }


        public bool IsUnique(string name)
        {
            return !Context.Educations.Any(x => x.Name == name);
        }
    }
}
