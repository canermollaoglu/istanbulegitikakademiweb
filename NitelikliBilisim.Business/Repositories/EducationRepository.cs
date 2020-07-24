using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.ComplexTypes;
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
        private readonly IElasticClient _elasticClient;
        public EducationRepository(NbDataContext context, IElasticClient elasticClient) : base(context)
        {
            _elasticClient = elasticClient;
        }


        public List<SuggestedEducationVm> GetSuggestedEducationList(bool isLoggedIn, string userId)
        {
            //Eğitim Id ve uygun kriterlerin sayısı. Eğitimler uygun kriter sayısına göre listelenecek.
            Dictionary<Guid, int> educationAndAppropriateCriterion = new Dictionary<Guid, int>();
            //Kullanıcının NBUY eğitimi alıp almadığını kontrol ediyoruz.
            var studentEducationInfo = Context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
            //Kullanıcı kayıtlı ise ve NBUY öğrencisi ise 
            if (isLoggedIn && studentEducationInfo != null)
            {
                var customer = Context.Customers.FirstOrDefault(x => x.Id == userId);
                //Müşterinin en yakın eğitim günü. (Müşteri hafta sonu veya tatil günü sisteme giriş yaptığını varsayarak geçmiş en yakın gün baz alındı.)
                int nearestDay = 0;
                var educationDay = Context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).First();
                nearestDay = educationDay.Day;

                /*Müşterinin NBUY eğitimi aldığı kategoriye göre eğitim listesi.*/
                var educations = Context.Educations.Include(c => c.Category).Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value).Include(x => x.EducationSuggestionCriterions);

                #region Kriterlerin uygunluğunun kontrolü
                foreach (var education in educations)
                {
                    int appropriateCriterion = 1;
                    if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                    {
                        foreach (var criterion in education.EducationSuggestionCriterions)
                        {
                            #region Eğitim Günü Kriteri
                            if (criterion.CriterionType == Core.Enums.educations.CriterionType.EducationDay)
                            {
                                if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                                    appropriateCriterion++;
                            }
                            #endregion
                            #region Yeni Kriter
                            //Todo eklenen kriterler buraya girilecek.
                            #endregion
                        }
                    }
                    educationAndAppropriateCriterion.Add(education.Id, appropriateCriterion);
                }
                #endregion

                //Yukarıda seçilen eğitimler içerisinden en çok kritere uyan 5 eğitim seçiliyor.
                var selectedEducations = educationAndAppropriateCriterion.OrderByDescending(entry => entry.Value)
                     .Take(5)
                     .ToDictionary(pair => pair.Key, pair => pair.Value);

                //Eğer seçilmiş eğitimler 5 taneyi tamamlayamıyorsa son eklenen 5 eğitim ile doldurulacak.
                var lastEducations = Context.Educations.OrderByDescending(x => x.CreatedDate).Take(10).ToList();
                int i = 0;
                int educationCount = Context.Educations.Count(x => x.IsActive);
                while (educationCount > 5 && selectedEducations.Count <= 5)
                {
                    if (!selectedEducations.ContainsKey(lastEducations[i].Id))
                    {
                        selectedEducations.Add(lastEducations[i].Id, 0);
                    }
                    i++;
                }


                return FillSuggestedEducationList(selectedEducations);
            }
            else
            {
                //Üye olmayanlar veya NBUY eğitimi almamış olanlar için son eklenen 5 eğitim.
                var educationsList = Context.Educations.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).Take(5)
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
                 }).ToList();

                var data = educationsList.Select(x => new SuggestedEducationVm
                {
                    Base = new EducationBaseVm
                    {
                        Id = x.Education.Id,
                        Name = x.Education.Name,
                        Description = x.Education.Description,
                        CategoryName = x.CategoryName,
                        Level = EnumSupport.GetDescription(x.Education.Level),
                        PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                        HoursPerDayText = x.Education.HoursPerDay.ToString(),
                        DaysText = x.Education.Days.ToString(),
                        DaysNumeric = x.Education.Days,
                        HoursPerDayNumeric = x.Education.HoursPerDay
                    },
                    Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
                    AppropriateCriterionCount = 0
                }
         ).ToList();

                return data;
            }
        }

        #region Suggested Educations Helper Method
        public List<SuggestedEducationVm> FillSuggestedEducationList(Dictionary<Guid, int> educationAndAppropriateCriterion)
        {
            var educationsList = Context.Educations.Where(x => educationAndAppropriateCriterion.Keys.Contains(x.Id) && x.IsActive)
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
               }).ToList();

            var data = educationsList.Select(x => new SuggestedEducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = x.Education.Id,
                    Name = x.Education.Name,
                    Description = x.Education.Description,
                    CategoryName = x.CategoryName,
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
                AppropriateCriterionCount = educationAndAppropriateCriterion.FirstOrDefault(y => y.Key == x.Education.Id).Value
            }
          ).OrderByDescending(x => x.AppropriateCriterionCount).ToList();

            return data;
        }
        public Guid GetBaseCategoryId(Guid categoryId)
        {
            var category = Context.EducationCategories.First(x => x.Id == categoryId);
            return category.BaseCategoryId ?? category.Id;
        }
        #endregion

        #region ElasticSearch Request
        public List<Education> GetViewingEducationsByCurrentSessionId(string sessionId)
        {
            List<TransactionLog> transactionLogs = new List<TransactionLog>();
            List<Guid> educationIds = new List<Guid>();
            var result = _elasticClient.Search<TransactionLog>(s =>
            s.Query(q => q.Match(m => m.Field(f => f.SessionId).Query(sessionId))
            && q.Match(m => m.Field(f => f.ControllerName).Query("Course"))
            && q.Match(m => m.Field(f => f.ActionName).Query("Details"))
            ));

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
                        educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
                }
            }
            return Context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        }
        public List<Education> GetViewingEducationsByCurrentUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Education>();


            var transactionLogs = new List<TransactionLog>();
            var educationIds = new List<Guid>();
            var result = _elasticClient.Search<TransactionLog>(s =>
            s.Query(q => q.Match(m => m.Field(f => f.UserId).Query(userId))
            && q.Match(m => m.Field(f => f.ControllerName).Query("Course"))
            && q.Match(m => m.Field(f => f.ActionName).Query("Details"))
            ));
            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
                        educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
                }
            }
            return Context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        }
        #endregion


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
                       Level = EnumSupport.GetDescription(e.Level),
                       Days = e.Days,
                       HoursPerDay = e.HoursPerDay,
                       isActive = e.IsActive
                   };
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
                    Level = EnumSupport.GetDescription(item.Level),
                    NewPrice = item.NewPrice,
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

        public int Update(Education entity, List<Guid> tagIds, bool isSaveLater = false)
        {
            var currentCategories = Context.Bridge_EducationTags.Where(x => x.Id2 == entity.Id).ToList();

            Context.Bridge_EducationTags.RemoveRange(currentCategories);

            var newItems = new List<Bridge_EducationTag>();
            foreach (var tagId in tagIds)
                newItems.Add(new Bridge_EducationTag
                {
                    Id = tagId,
                    Id2 = entity.Id
                });

            Context.Bridge_EducationTags.AddRange(newItems);
            Context.SaveChanges();

            return base.Update(entity, isSaveLater);
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
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
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

            var educationGroupRepository = new EducationGroupRepository(Context);

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
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
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

        public bool IsUnique(string name)
        {
            return !Context.Educations.Any(x => x.Name == name);
        }
    }
}
