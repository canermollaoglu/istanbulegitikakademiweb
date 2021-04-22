using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.MongoOptions.Entities;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Core.ViewModels.Main.Wizard;
using NitelikliBilisim.Core.ViewModels.Suggestion;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NitelikliBilisim.Core.Enums.suggestion;

namespace NitelikliBilisim.Business.Repositories
{
    public class SuggestionRepository
    {
        private readonly NbDataContext _context;
        private readonly SuggestionSystemOptions _options;
        private readonly TransactionLogRepository _transactionLogRepository;
        private readonly SuggestedEducationsRepository _suggestedEducations;
        private readonly IConfiguration _configuration;

        public SuggestionRepository(NbDataContext context, SuggestedEducationsRepository suggestedEducations, TransactionLogRepository transactionLogRepository, IConfiguration configuration)
        {
            _context = context;
            _transactionLogRepository = transactionLogRepository;
            _suggestedEducations = suggestedEducations;
            _configuration = configuration;
            _options = configuration.GetSection("EducationSuggestionSystemOptions").Get<SuggestionSystemOptions>(); ;
        }

        /// <summary>
        /// Öneri sisteminde eğitimlere verilen nihai puanları döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<EducationPoint> GetEducationSuggestionRate(string userId)
        {
            var result = GetViewedEducations(userId);

            var searched = GetSearchedEducations(result, userId);
            var viewed = GetViewingEducations(result);

            var criterionBased = GetCriteriaBasedSuggestions(userId);

            var userActionBased = CalculateUserActionBasedSuggestionTotalPoint(searched, viewed);

            List<EducationPoint> totalSuggestionPoint = CalculateTotalSuggestionPoint(criterionBased, userActionBased);

            return totalSuggestionPoint;
        }

        /// <summary>
        /// Login olmuş kullanıcılara önerilen eğitimleri döner
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count">kaç adet dönmesi gerektiği</param>
        /// <returns></returns>
        public void GetUserSuggestedEducations(string userId, int count)
        {

            var educationPoints = GetEducationSuggestionRate(userId);
            var selectedEducations = educationPoints.OrderByDescending(x => x.Point)
                         .Take(count)
                         .ToDictionary(pair => pair.SeoUrl, pair => pair.Point);

            var lastEducations = _context.Educations.OrderByDescending(x => x.CreatedDate).Where(x => x.IsActive).Take(10).ToList();
            int i = 0;
            int educationCount = _context.Educations.Count(x => x.IsActive);
            while (educationCount > count && selectedEducations.Count() < count)
            {
                if (!selectedEducations.ContainsKey(lastEducations[i].SeoUrl))
                {
                    selectedEducations.Add(lastEducations[i].SeoUrl, 0);
                }
                i++;
            }
            _suggestedEducations.Create(new MSuggestedEducation { UserId = userId, Educations = selectedEducations, });

        }
        public List<SuggestedEducationVm> GetUserSuggestedEducations(string userId)
        {
            var educations = _suggestedEducations.GetByUserId(userId);
            if (educations == null)
            {
                return GetGuestUserSuggestedEducations();
            }
            else
            {
                return FillSuggestedEducationList(educations.Educations);
            }
        }


        /// <summary>
        /// Misafir kullanıcılara önerilen eğitimleri döner
        /// </summary>
        /// <returns></returns>
        public List<SuggestedEducationVm> GetGuestUserSuggestedEducations()
        {
            var educationsList = _context.Educations.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).Take(4)
                 .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.Card), l => l.Id, r => r.EducationId, (x, y) => new
                 {
                     Education = x,
                     EducationPreviewMedia = y
                 })
                 .Join(_context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
                 {
                     Education = x.Education,
                     EducationPreviewMedia = x.EducationPreviewMedia,
                     CategoryName = y.Name,
                     CategorySeoUrl = y.SeoUrl
                 }).ToList();
            var hostId = Guid.Parse(_configuration.GetSection("SiteGeneralOptions").GetSection("PriceLocationId").Value);
            var data = educationsList.Select(x => new SuggestedEducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = x.Education.Id,
                    Name = x.Education.Name,
                    Description = x.Education.Description,
                    CategoryName = x.CategoryName,
                    CategorySeoUrl = x.CategorySeoUrl,
                    Level = EnumHelpers.GetDescription(x.Education.Level),
                    Price = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault(y => y.HostId == hostId && y.EducationId == x.Education.Id).NewPrice.GetValueOrDefault().ToString(CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay,
                    SeoUrl = x.Education.SeoUrl
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
                AppropriateCriterionCount = 0
            }
     ).ToList();

            return data;
        }


        /// <summary>
        /// Kriter bazlı ve Kullanıcı davranışları bazlı eğitim puanlarını alarak nihai puanları hesaplar.
        /// </summary>
        /// <param name="criterionBased"></param>
        /// <param name="userActionBased"></param>
        /// <returns></returns>
        private List<EducationPoint> CalculateTotalSuggestionPoint(List<EducationPoint> criterionBased, List<EducationPoint> userActionBased)
        {
            var educations = _context.Educations.Where(x => x.IsActive).ToList();
            List<EducationPoint> retVal = new List<EducationPoint>();
            List<EducationPoint> totalPoints = new List<EducationPoint>();

            totalPoints.AddRange(criterionBased);
            totalPoints.AddRange(userActionBased);

            foreach (var info in totalPoints)
            {
                if (!educations.Any(x => x.SeoUrl == info.SeoUrl))
                    continue;

                if (retVal.Any(x => x.SeoUrl == info.SeoUrl))
                {
                    retVal.First(x => x.SeoUrl == info.SeoUrl).Point += info.Point;
                }
                else
                {
                    retVal.Add(new EducationPoint()
                    {
                        SeoUrl = info.SeoUrl,
                        Point = info.Point
                    });
                }

            }


            //retVal = retVal.GroupBy(x => x.SeoUrl).Select(y => new EducationPoint { SeoUrl = y.Key, Point = Math.Round(y.Sum(x => x.Point),2),Education=educations.FirstOrDefault(x=>x.SeoUrl == y.Key) }).ToList();
            return retVal;
        }

        public List<EducationOfTheWeekVm> GetEducationsOfTheWeek(int week, string userId)
        {
            var eInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
            var categoryId = Guid.Empty;
            if (eInfo != null)
            {
                categoryId = eInfo.CategoryId;
            }
            var nearestDay = week * 7;
            var educations = _context.Educations.Include(c => c.Category).Include(x => x.EducationSuggestionCriterions).Where(x => x.IsActive);
            var hostId = Guid.Parse(_configuration.GetSection("SiteGeneralOptions").GetSection("PriceLocationId").Value);
            var activeGroups = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now && x.HostId == hostId).Select(x => x.EducationId);
            var thisWeekEducations = new Dictionary<string, double>();

            var weeklySuggestedEducations = new List<WeeklySuggestedEducationsVm>();

            foreach (var education in educations)
            {
                var relatedNbuyCategories = !string.IsNullOrEmpty(education.RelatedNBUYCategories) ? JsonConvert.DeserializeObject<List<Guid>>(education.RelatedNBUYCategories) : new List<Guid>();

                if (activeGroups.Contains(education.Id) && relatedNbuyCategories.Contains(education.Category.BaseCategoryId.Value) && education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                {
                    foreach (var criterion in education.EducationSuggestionCriterions)
                    {
                        if (criterion.CriterionType == CriterionType.EducationDay)
                        {
                            if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                            {
                                weeklySuggestedEducations.Add(new WeeklySuggestedEducationsVm
                                {
                                    SeoUrl = education.SeoUrl,
                                    IsCurrentCategory = education.Category.BaseCategoryId == categoryId,
                                    StartDay = criterion.MinValue.Value,
                                    EndDay = criterion.MaxValue.Value,
                                    Point = education.Category.BaseCategoryId == categoryId?4:2,
                                    WeekType = WeekType.CurrentWeek
                                });
                                //thisWeekEducations.Add(education.SeoUrl, education.Category.BaseCategoryId == categoryId ? 6 : 3);//İçinde bulunulan hafta en öncelikli olduğu için sıra 3 
                            }
                            else if (criterion.MaxValue <= nearestDay - 7 && criterion.MinValue > nearestDay - 14)
                            {
                                weeklySuggestedEducations.Add(new WeeklySuggestedEducationsVm
                                {
                                    SeoUrl = education.SeoUrl,
                                    IsCurrentCategory = education.Category.BaseCategoryId == categoryId,
                                    StartDay = criterion.MinValue.Value,
                                    EndDay = criterion.MaxValue.Value,
                                    Point = education.Category.BaseCategoryId == categoryId ? 3 : 1,
                                    WeekType = WeekType.NextWeek
                                });
                            }
                            //thisWeekEducations.Add(education.SeoUrl, education.Category.BaseCategoryId == categoryId ? 2 : 1);//sonraki hafta için sıra 1 
                            else if (criterion.MaxValue <= nearestDay + 7 && criterion.MinValue > nearestDay)
                            {
                                weeklySuggestedEducations.Add(new WeeklySuggestedEducationsVm
                                {
                                    SeoUrl = education.SeoUrl,
                                    IsCurrentCategory = education.Category.BaseCategoryId == categoryId,
                                    StartDay = criterion.MinValue.Value,
                                    EndDay = criterion.MaxValue.Value,
                                    Point = education.Category.BaseCategoryId == categoryId ? 3 : 1,
                                    WeekType = WeekType.LastWeek
                                });
                            }
                            //thisWeekEducations.Add(education.SeoUrl, education.Category.BaseCategoryId == categoryId ? 4 : 2);//onceki hafta için sıra 2
                        }
                    }
                }
            }
            
            var currentWeekEducations = weeklySuggestedEducations.Where(x => x.WeekType == WeekType.CurrentWeek).ToList();
            var lastWeekEducations = weeklySuggestedEducations.Where(x => x.WeekType == WeekType.LastWeek).ToList();
            var nextWeekEducations = weeklySuggestedEducations.Where(x => x.WeekType == WeekType.NextWeek).ToList();
            var currentWeekEducationCount = currentWeekEducations.Count();
            if (currentWeekEducationCount>0)
            {
                if (currentWeekEducationCount>1)
                {
                    while (thisWeekEducations.Count <= 2)
                    {
                        var ed = currentWeekEducations.ElementAt(new Random().Next(0, currentWeekEducationCount));
                        if (!thisWeekEducations.ContainsKey(ed.SeoUrl))
                        {
                            thisWeekEducations.Add(ed.SeoUrl, ed.Point);
                        }
                    }
                }
                else
                {
                        thisWeekEducations.Add(currentWeekEducations[0].SeoUrl,currentWeekEducations[0].Point);
                }
            }

            if (lastWeekEducations.Any())
            {
                var maxDay = lastWeekEducations.OrderByDescending(x => x.EndDay).First();
                thisWeekEducations.Add(maxDay.SeoUrl,maxDay.Point);
            }
            if (nextWeekEducations.Any())
            {
                var maxDay = nextWeekEducations.OrderBy(x => x.StartDay).First();
                thisWeekEducations.Add(maxDay.SeoUrl, maxDay.Point);
            }





            var retVal = (from education in _context.Educations
                          join category in _context.EducationCategories on education.CategoryId equals category.Id
                          join eImage in _context.EducationMedias on new {  education.Id, MediaType = EducationMediaType.List } equals new { Id = eImage.EducationId, eImage.MediaType }
                          join cardImage in _context.EducationMedias on new { education.Id, MediaType = EducationMediaType.Card } equals new { Id = cardImage.EducationId, MediaType = cardImage.MediaType }
                          where thisWeekEducations.Keys.Contains(education.SeoUrl)
                          select new EducationOfTheWeekVm
                          {
                              Id = education.Id,
                              Name = education.Name,
                              CategoryName = category.Name,
                              Description = education.Description,
                              CategorySeoUrl = category.SeoUrl,
                              Day = education.Days,
                              Hour = education.Days * education.HoursPerDay,
                              SeoUrl = education.SeoUrl,
                              Image = eImage.FileUrl,
                              CardImage = cardImage.FileUrl
                          }).ToList();

            foreach (var education in retVal)
            {
                education.Price = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now).OrderBy(x => x.CreatedDate).First(y => y.HostId == hostId && y.EducationId == education.Id).NewPrice.GetValueOrDefault().ToString(CultureInfo.CreateSpecificCulture("tr-TR"));
                education.AppropriateCriterionCount = thisWeekEducations.FirstOrDefault(y => y.Key == education.SeoUrl).Value;
            }
            return retVal.OrderByDescending(x => x.AppropriateCriterionCount).ToList();
        }

        public List<WizardFirstStepData> GetWizardFirstStepData()
        {
            return _context.EducationCategories.Where(x => x.BaseCategoryId == null && x.CategoryType == CategoryType.NBUY).OrderBy(x => x.Order).Select(x =>
                 new WizardFirstStepData
                 {
                     Id = x.Id,
                     Name = x.Name,
                     IconUrl = x.IconUrl,
                     WizardClass = x.WizardClass
                 }).ToList();
        }

        public List<WizardSecondStepData> GetWizardSecondStepData(List<Guid> relatedCategories)
        {
            var retval = new List<WizardSecondStepData>();
            var categories = _context.EducationCategories.Where(x => relatedCategories.Contains(x.Id)).ToList();
            var subCategories = _context.EducationCategories.Where(x => x.BaseCategoryId != null && relatedCategories.Contains(x.BaseCategoryId.Value)).ToList();
            foreach (var baseCategory in categories)
            {
                var data = new WizardSecondStepData
                {
                    Id = baseCategory.Id,
                    Name = baseCategory.Name
                };
                data.SubCategories = subCategories.Where(x => x.BaseCategoryId == baseCategory.Id).Select(x => new WizardSecondStepSubData
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                retval.Add(data);
            }
            return retval;
        }

        public List<WizardSuggestedEducationVm> GetWizardSuggestedEducations(List<WizardLastStepPostVm> lastdata)
        {
            var hostId = Guid.Parse(_configuration.GetSection("SiteGeneralOptions").GetSection("PriceLocationId").Value);

            var educations = _context.Educations.Where(x => x.IsActive).ToList();
            var suggestedEducations = new List<Education>();

            foreach (var data in lastdata)
            {
                var eList = educations.Where(x => x.CategoryId == data.Id && (int)x.Level == data.Level);
                suggestedEducations.AddRange(eList);
            }
            var activeGroups = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now && x.HostId == hostId).Select(x => x.EducationId);

            var retVal = (from education in suggestedEducations
                          join category in _context.EducationCategories on education.CategoryId equals category.Id
                          join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                          where
                          eImage.MediaType == EducationMediaType.List && education.IsActive && activeGroups.Contains(education.Id)
                          select new WizardSuggestedEducationVm
                          {
                              Id = education.Id,
                              SeoUrl = education.SeoUrl,
                              CatSeoUrl = category.SeoUrl,
                              Name = education.Name,
                              Day = education.Days,
                              Hours = education.Days * education.HoursPerDay,
                              ImageUrl = eImage.FileUrl,
                              Description = education.Description,
                              Price = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault(y => y.HostId == hostId && y.EducationId == education.Id).NewPrice.GetValueOrDefault().ToString(CultureInfo.CreateSpecificCulture("tr-TR"))
                          }).ToList();

            return retVal;
        }



        #region 1 KRİTERLERE GÖRE EĞİTİM ÖNERİLERİ

        public List<EducationPoint> GetCriteriaBasedSuggestions(string userId)
        {
            List<EducationPoint> educationAppropriateCriterionRate = new List<EducationPoint>();
            //Kullanıcının NBUY eğitimi alıp almadığını kontrol ediyoruz.
            var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
            //Kullanıcı kayıtlı ise ve NBUY öğrencisi ise 
            if (studentEducationInfo != null)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
                //Müşterinin en yakın eğitim günü. (Müşteri hafta sonu veya tatil günü sisteme giriş yaptığını varsayarak geçmiş en yakın gün baz alındı.)

                var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).FirstOrDefault();
                int nearestDay = educationDay != null ? educationDay.Day : 0;

                /*Müşterinin NBUY eğitimi aldığı kategoriye göre eğitim listesi.*/
                //Todo burada yalnızca nbuy kategorisi mi geçerli olacak yoksa tüm kategorilerdeki eğitimler dikkate alınacak mı konusunun netleştirilmesi gerekli.
                var educations = _context.Educations.Include(c => c.Category).Include(x => x.EducationSuggestionCriterions).Where(x => x.IsActive);//.Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value);
                #region Favori eklenen eğitimler
                List<string> userWishList = _context.Wishlist.Where(x => x.Id == customer.Id).Include(x => x.Education).Select(x => x.Education.Id.ToString().ToLower()).ToList();
                #endregion
                #region Satın alınan eğitimler
                List<string> userPurchasedEducations = new List<string>();
                var tickets = _context.Tickets
                .Where(x => x.OwnerId == customer.Id)
                .ToList();
                tickets.ForEach(x => userPurchasedEducations.Add(x.EducationId.ToString().ToLower()));
                #endregion

                #region Kriterlerin uygunluğunun kontrolü
                foreach (var education in educations)
                {
                    int appropriateCriterion = 0;
                    #region Kriterlere göre eğitim önerileri
                    if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                    {
                        foreach (var criterion in education.EducationSuggestionCriterions)
                        {
                            #region Eğitim Günü Kriteri
                            if (criterion.CriterionType == CriterionType.EducationDay && education.Category.BaseCategoryId == studentEducationInfo.CategoryId)
                            {
                                if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                                    appropriateCriterion += _options.EducationDayCriterion;//Eğitim günü kriteri %50 etkilediği için 100 puan üzerinden 50 puan ekleniyor.
                                else if (nearestDay > criterion.MaxValue && nearestDay < criterion.MaxValue + 7)
                                    appropriateCriterion += _options.EducationDayTwoWeeksBefore;//Eğitim günü kriteri iki hafta öncesine kadar 30 puan etkiliyor.
                                else if (nearestDay < criterion.MinValue && nearestDay >= criterion.MinValue - 14)
                                    appropriateCriterion += _options.EducationDayOneWeekAfter;//Eğitim günü kriteri bir hafta sonrasına kadar 30 puan etkiliyor.
                            }
                            #endregion
                            #region Favorilere Eklenmiş Eğitimler Kriteri
                            if (criterion.CriterionType == CriterionType.WishListEducations)
                            {
                                List<string> wishListItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, wishListItemIds, userWishList);
                            }
                            #endregion
                            #region Satın Alınmış Eğitimler Kriteri
                            if (criterion.CriterionType == CriterionType.PurchasedEducations)
                            {
                                List<string> criterionItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, criterionItemIds, userPurchasedEducations);
                            }
                            #endregion
                        }
                    }
                    #endregion
                    if (appropriateCriterion > 0)
                    {
                        educationAppropriateCriterionRate.Add(
                                                new EducationPoint { SeoUrl = education.SeoUrl, Point = appropriateCriterion / 2 }
                                                );
                    }
                }
                #endregion
            }
            return educationAppropriateCriterionRate;
        }

        #region Suggested Educations Helper Method
        /// <summary>
        /// Bu method eğitimde belirtilen Favori eğitim ve Satın alınan eğitim kriterlerindeki eğitimleri 
        /// ve kullanıcının satın aldığı ve favoriye eklediği eğitimleri alarak 
        /// bir puan dönderir.
        /// </summary>
        /// <param name="criterionType"></param>
        /// <param name="criterionEducationList"></param>
        /// <param name="studentEducationList"></param>
        /// <returns></returns>
        public double TotalSameElementPoint(CriterionType criterionType, List<string> criterionEducationList, List<string> studentEducationList)
        {
            //Her bir kriter uyumu için eklenecek puan
            double criterionPoint = criterionType == CriterionType.WishListEducations ? _options.WishlistEducationsCriterion / criterionEducationList.Count : _options.PurchasedEducationsCriterion / criterionEducationList.Count;
            //Toplam puan
            double totalPoint = 0;
            for (int i = 0; i < criterionEducationList.Count; i++)
                if (studentEducationList.Contains(criterionEducationList[i].ToLower()))
                    totalPoint += criterionPoint;
            return totalPoint;
        }

        public List<SuggestedEducationVm> FillSuggestedEducationList(Dictionary<string, double> educationAndAppropriateCriterion)
        {
            var educationsList = _context.Educations.Where(x => educationAndAppropriateCriterion.Keys.Contains(x.SeoUrl) && x.IsActive)
               .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.Card), l => l.Id, r => r.EducationId, (x, y) => new
               {
                   Education = x,
                   EducationPreviewMedia = y
               })
               .Join(_context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
               {
                   Education = x.Education,
                   EducationPreviewMedia = x.EducationPreviewMedia,
                   CategoryName = y.Name,
                   CategorySeoUrl = y.SeoUrl
               }).ToList();

            var hostId = Guid.Parse(_configuration.GetSection("SiteGeneralOptions").GetSection("PriceLocationId").Value);

            var retVal = new List<SuggestedEducationVm>();
            //  var data = educationsList.Select(x => new SuggestedEducationVm
            //  {
            //      Base = new EducationBaseVm
            //      {
            //          Id = x.Education.Id,
            //          Name = x.Education.Name,
            //          Description = x.Education.Description,
            //          CategoryName = x.CategoryName,
            //          CategorySeoUrl = x.CategorySeoUrl,
            //          Level = EnumHelpers.GetDescription(x.Education.Level),
            //          //Price = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault(y => y.HostId == hostId && y.EducationId == x.Education.Id).NewPrice.GetValueOrDefault().ToString(CultureInfo.CreateSpecificCulture("tr-TR")),
            //          HoursPerDayText = x.Education.HoursPerDay.ToString(),
            //          DaysText = x.Education.Days.ToString(),
            //          DaysNumeric = x.Education.Days,
            //          HoursPerDayNumeric = x.Education.HoursPerDay,
            //          SeoUrl = x.Education.SeoUrl
            //      },
            //      Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
            //      AppropriateCriterionCount = educationAndAppropriateCriterion.FirstOrDefault(y => y.Key == x.Education.SeoUrl).Value
            //  }
            //).OrderByDescending(x => x.AppropriateCriterionCount).ToList();

            foreach (var education in educationsList)
            {
                var priceGroup = _context.EducationGroups.Where(x => x.StartDate > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault(y => y.HostId == hostId && y.EducationId == education.Education.Id);
                if (priceGroup != null)
                {
                    retVal.Add(new SuggestedEducationVm
                    {
                        Base = new EducationBaseVm
                        {
                            Id = education.Education.Id,
                            Name = education.Education.Name,
                            Description = education.Education.Description,
                            CategoryName = education.CategoryName,
                            CategorySeoUrl = education.CategorySeoUrl,
                            Level = EnumHelpers.GetDescription(education.Education.Level),
                            Price = priceGroup.NewPrice.GetValueOrDefault().ToString(CultureInfo.CreateSpecificCulture("tr-TR")),
                            HoursPerDayText = education.Education.HoursPerDay.ToString(),
                            DaysText = education.Education.Days.ToString(),
                            DaysNumeric = education.Education.Days,
                            HoursPerDayNumeric = education.Education.HoursPerDay,
                            SeoUrl = education.Education.SeoUrl
                        },
                        Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = education.Education.Id, FileUrl = education.EducationPreviewMedia.FileUrl } },
                        AppropriateCriterionCount = educationAndAppropriateCriterion.FirstOrDefault(y => y.Key == education.Education.SeoUrl).Value
                    });
                }
            }

            return retVal.OrderByDescending(x => x.AppropriateCriterionCount).ToList();
        }
        public Guid GetBaseCategoryId(Guid categoryId)
        {
            var category = _context.EducationCategories.First(x => x.Id == categoryId);
            return category.BaseCategoryId ?? category.Id;
        }
        #endregion

        #endregion

        #region 2 KULLANICI DAVRANIŞLARINA GÖRE EĞİTİM ÖNERİLERİ

        public EducationDetailLog GetEducationDetailLogs(string userId)
        {

            EducationDetailLog model = new EducationDetailLog();
            model.ViewingEducations = new List<ViewingEducation>();
            model.SearchedEducations = new List<SearchedEducationList>();
            model.EducationTotalPoint = new List<EducationPoint>();
            Dictionary<string, int> getAllSearching = GetAllSearchedKeyAndSearchCount(userId);

            #region İncelenmiş Eğitimler  | Aranılarak incelenmiş ve direkt incelenmiş olarak ikiye ayrılıyor.

            var result = GetViewedEducations(userId);
            int totalEducationViewCount = GetEducationViewTotalCount(result);
            model.TotalEducationSearchCount = CountOfEducationsSearchedAndViewed(result, getAllSearching);

            model.ViewingEducations = GetViewingEducations(result);
            model.SearchedEducations = GetSearchedEducations(result, userId);
            #endregion

            model.TotalEducationViewCount = totalEducationViewCount;
            foreach (var item in model.SearchedEducations)
            {
                if (getAllSearching.ContainsKey(item.Key))
                {
                    item.SearchedCount = getAllSearching[item.Key];
                }
            }


            model.EducationTotalPoint = CalculateUserActionBasedSuggestionTotalPoint(model.SearchedEducations, model.ViewingEducations);

            return model;
        }

        /// <summary>
        /// Arama kelimesi, arama sayısı ve bu arama ile incelenmiş eğitimlerin id ve puanlarını döner
        /// </summary>
        /// <param name="result"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<SearchedEducationList> GetSearchedEducations(List<TransactionLog> result, string userId)
        {
            List<SearchedEducationList> model = new List<SearchedEducationList>();
            Dictionary<string, int> getAllSearching = GetAllSearchedKeyAndSearchCount(userId);
            int totalEducationSearchCount = CountOfEducationsSearchedAndViewed(result, getAllSearching);
            if (result != null && result.Count > 0)
            {
                foreach (var log in result)
                {//Aranılarak incelenmiş eğitimler
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        string key = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchKey").ParameterValue);
                        int totalKeySearched = getAllSearching[key];

                        string seoUrl = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "seoUrl").ParameterValue);
                        if (model.Any(x => x.Key == key))
                        {
                            var eDetail = new EducationDetail();
                            eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, totalEducationSearchCount);
                            eDetail.SeoUrl = seoUrl;
                            eDetail.Education = _context.Educations.FirstOrDefault(x => x.SeoUrl == seoUrl);
                            SearchedEducationList ed = model.First(x => x.Key == key);
                            ed.EducationDetails.Add(eDetail);
                        }
                        else
                        {
                            var eDetail = new EducationDetail();
                            eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, totalEducationSearchCount);
                            eDetail.SeoUrl = seoUrl;
                            eDetail.Education = _context.Educations.FirstOrDefault(x => x.SeoUrl == seoUrl);
                            var sE = new SearchedEducationList();
                            sE.Key = key;
                            sE.EducationDetails.Add(eDetail);
                            model.Add(sE);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// İncelenmiş eğitimleri, incelenme sayısını ve puanlarını döner.
        /// </summary>
        /// <param name="result">Elastic search üzerinden dönen veriler.</param>
        /// <returns></returns>
        private List<ViewingEducation> GetViewingEducations(List<TransactionLog> result)
        {
            List<ViewingEducation> model = new List<ViewingEducation>();
            int totalEducationViewCount = GetEducationViewTotalCount(result);
            if (result != null && result.Count > 0)
            {
                foreach (var log in result)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "seoUrl") && !log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        string seoUrl = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "seoUrl").ParameterValue);
                        if (model.Any(x => x.SeoUrl == seoUrl))
                        {
                            ViewingEducation current = model.First(x => x.SeoUrl == seoUrl);
                            current.Education = _context.Educations.FirstOrDefault(x => x.SeoUrl == seoUrl);
                            current.ViewingCount++;
                            current.Point = CalculateViewedEducationPoint(current.ViewingCount, totalEducationViewCount);
                        }
                        else
                        {
                            model.Add(new ViewingEducation
                            {
                                SeoUrl = seoUrl,
                                Education = _context.Educations.FirstOrDefault(x => x.SeoUrl == seoUrl),
                                ViewingCount = 1,
                                Point = CalculateViewedEducationPoint(1, totalEducationViewCount)
                            });
                        }
                    }
                }
            }
            return model;
        }


        /// <summary>
        /// Kullanıcı davranışlarına göre eğitim puanlarını döner
        /// </summary>
        /// <param name="searchedEducations">Aranılarak incelenen eğitimler</param>
        /// <param name="viewingEducations">Direkt incelenen eğitimler</param>
        /// <returns></returns>
        private List<EducationPoint> CalculateUserActionBasedSuggestionTotalPoint(List<SearchedEducationList> searchedEducations, List<ViewingEducation> viewingEducations)
        {
            List<EducationPoint> retVal = new List<EducationPoint>();
            List<EducationPoint> viewingEducationPoints = new List<EducationPoint>();
            List<EducationPoint> searchedEducationPoints = new List<EducationPoint>();
            List<EducationPoint> totalPoints = new List<EducationPoint>();

            var educations = _context.Educations.Where(x => x.IsActive).ToList();
            //İncelenmiş eğitimler ve puanları
            foreach (var education in viewingEducations)
            {
                viewingEducationPoints.Add(new EducationPoint
                {
                    SeoUrl = education.SeoUrl,
                    Point = education.Point
                });
            }
            //Aranılarak incelenmiş eğitimler ve puanları
            foreach (var key in searchedEducations)
            {
                foreach (var detail in key.EducationDetails)
                {
                    var education = searchedEducationPoints.FirstOrDefault(x => x.SeoUrl == detail.SeoUrl);

                    if (education == null)
                    {
                        searchedEducationPoints.Add(new EducationPoint
                        {
                            SeoUrl = detail.SeoUrl,
                            Point = detail.Point
                        });
                    }
                    else
                    {
                        if (detail.Point > education.Point)
                        {
                            searchedEducationPoints.First(x => x.SeoUrl == detail.SeoUrl).Point = detail.Point;
                        }
                    }
                }
            }


            totalPoints.AddRange(viewingEducationPoints);
            totalPoints.AddRange(searchedEducationPoints);

            foreach (var info in totalPoints)
            {
                if (!educations.Any(x => x.SeoUrl == info.SeoUrl))
                    continue;

                if (retVal.Any(x => x.SeoUrl == info.SeoUrl))
                {
                    retVal.First(x => x.SeoUrl == info.SeoUrl).Point += info.Point / 2;
                }
                else
                {
                    retVal.Add(new EducationPoint()
                    {
                        SeoUrl = info.SeoUrl,
                        Point = info.Point / 2
                    });
                }

            }

            //retVal = totalPoints.GroupBy(x => x.SeoUrl).Select(y => new EducationPoint { SeoUrl = y.Key,Education = educations.FirstOrDefault(x=>x.SeoUrl ==y.Key), Point = (y.Sum(x => x.Point)) / 2 }).ToList();

            return retVal;
        }

        /// <summary>
        /// Aranılarak inceleme sayısını döner
        /// </summary>
        /// <param name="result"></param>
        /// <param name="getAllSearching"></param>
        /// <returns></returns>
        private int CountOfEducationsSearchedAndViewed(List<TransactionLog> result, Dictionary<string, int> getAllSearching)
        {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            if (result != null && result.Count > 0)
            {
                foreach (var log in result)
                {//Aranılarak incelenmiş eğitimler
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        string key = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchKey").ParameterValue);
                        if (!counter.ContainsKey(key))
                        {
                            counter.Add(key, getAllSearching[key]);
                        }
                    }
                }
            }
            return counter.Sum(x => x.Value);
        }
        private int GetEducationViewTotalCount(List<TransactionLog> result)
        {
            int total = 0;
            if (result != null && result.Count > 0)
            {
                foreach (var log in result)
                {
                    if (log.Parameters != null && !log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        total++;
                    }
                }
            }
            return total;
        }

        private double CalculateSearchedKeyPoint(int totalKeySearched, int totalSearchedCount)
        {

            return Convert.ToDouble(totalKeySearched) / Convert.ToDouble(totalSearchedCount) * _options.SearchedEducation;
        }
        private double CalculateViewedEducationPoint(int totalEducationView, int totalAllEducationView)
        {
            return Convert.ToDouble(totalEducationView) / Convert.ToDouble(totalAllEducationView) * _options.ViewedEducation;
        }


        /// <summary>
        /// UserId ile elasticsearch üzerinde yapılan sorgu ile tüm incelenmiş eğitimleri döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<TransactionLog> GetViewedEducations(string userId)
        {
            var result = _transactionLogRepository.GetList(x => x.UserId == userId && x.ControllerName == "Course" && x.ActionName == "Details");
            return result;
        }

        #endregion

        /// <summary>
        /// userId bazında arama yapılan kelimeleri ve arama sayılarını döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Dictionary<string, int> GetAllSearchedKeyAndSearchCount(string userId)
        {
            Dictionary<string, int> allSearchings = new Dictionary<string, int>();

            var searchedTexts = _transactionLogRepository.GetList(x => x.UserId == userId && x.ControllerName == "Course" && x.ActionName == "GetCourses");

            if (searchedTexts != null && searchedTexts.Count > 0)
            {
                foreach (var log in searchedTexts)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        string sText = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchKey").ParameterValue);

                        if (allSearchings.ContainsKey(sText))
                            allSearchings[sText]++;
                        else
                            allSearchings.Add(sText, 1);
                    }
                }
            }

            return allSearchings;
        }
    }
}




