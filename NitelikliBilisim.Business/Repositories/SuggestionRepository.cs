using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Suggestion;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class SuggestionRepository
    {
        private readonly NbDataContext _context;
        private readonly IElasticClient _elasticClient;
        private readonly SuggestionSystemOptions _options;

        public SuggestionRepository(NbDataContext context, IElasticClient elasticClient, IConfiguration configuration)
        {
            _context = context;
            _elasticClient = elasticClient;
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
        public List<SuggestedEducationVm> GetUserSuggestedEducations(string userId, int count)
        {

            var educationPoints = GetEducationSuggestionRate(userId);
            var selectedEducations = educationPoints.OrderByDescending(x => x.Point)
                         .Take(count)
                         .ToDictionary(pair => pair.EducationId, pair => pair.Point);

            var lastEducations = _context.Educations.OrderByDescending(x => x.CreatedDate).Where(x => x.IsActive).Take(10).ToList();
            int i = 0;
            int educationCount = _context.Educations.Count(x => x.IsActive);
            while (educationCount > count && selectedEducations.Count() < count)
            {
                if (!selectedEducations.ContainsKey(lastEducations[i].Id))
                {
                    selectedEducations.Add(lastEducations[i].Id, 0);
                }
                i++;
            }

            return FillSuggestedEducationList(selectedEducations);
        }

        /// <summary>
        /// Misafir kullanıcılara önerilen eğitimleri döner
        /// </summary>
        /// <returns></returns>
        public List<SuggestedEducationVm> GetGuestUserSuggestedEducations()
        {
            var educationsList = _context.Educations.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).Take(5)
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
            retVal.AddRange(criterionBased);
            retVal.AddRange(userActionBased);
            retVal = retVal.GroupBy(x => x.EducationId).Select(y => new EducationPoint { EducationId = y.Key, Point = Math.Round(y.Sum(x => x.Point),2),Education=educations.First(x=>x.Id == y.Key) }).ToList();
            return retVal;
        }



        #region 1 KRİTERLERE GÖRE EĞİTİM ÖNERİLERİ

        /// <summary>
        /// Kriterlere göre önerilen eğitimler
        /// </summary>
        /// <param name="isLoggedIn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// 
        //public List<SuggestedEducationVm> GetSuggestedEducationList(bool isLoggedIn, string userId)
        //{
        //    List<EducationPoint> educationAppropriateCriterionRate = new List<EducationPoint>();
        //    var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
        //    if (isLoggedIn)
        //    {
        //        var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
        //        if (customer != null)
        //        {
        //            //Öğrencinin en yakın eğitim günü. (Müşteri hafta sonu veya tatil günü sisteme giriş yaptığını varsayarak geçmiş en yakın gün baz alındı.)
        //            int nearestDay = 0;
        //            var educationDay = studentEducationInfo != null ? _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).First().Day : 0;
        //            nearestDay = educationDay;
        //            var allEducations = _context.Educations.Include(x => x.Category).Include(x => x.EducationSuggestionCriterions).Where(x => x.IsActive);
        //            /*Öğrencinin NBUY eğitimi varsa o kategorideki eğitimler yoksa tüm eğitimler.*/
        //            var educations = studentEducationInfo != null ? allEducations.Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value) : allEducations;
        //            #region Favori eklenen eğitimler
        //            List<string> userWishList = _context.Wishlist.Where(x => x.Id == customer.Id).Include(x => x.Education).Select(x => x.Education.Id.ToString().ToLower()).ToList();
        //            #endregion
        //            #region Satın alınan eğitimler
        //            List<string> userPurchasedEducations = new List<string>();
        //            var tickets = _context.Tickets
        //            .Where(x => x.OwnerId == customer.Id)
        //            .ToList();
        //            tickets.ForEach(x => userPurchasedEducations.Add(x.EducationId.ToString().ToLower()));
        //            #endregion


        //            #region Kriterlerin uygunluğunun kontrolü
        //            foreach (var education in educations)
        //            {
        //                int appropriateCriterion = 0;
        //                if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
        //                {
        //                    foreach (var criterion in education.EducationSuggestionCriterions)
        //                    {
        //                        #region Eğitim Günü Kriteri
        //                        if (studentEducationInfo != null && criterion.CriterionType == CriterionType.EducationDay)
        //                        {
        //                            if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
        //                                appropriateCriterion += _options.EducationDayCriterion;//Eğitim günü kriteri %50 etkilediği için 100 puan üzerinden 50 puan ekleniyor.
        //                            else if (nearestDay > criterion.MaxValue && nearestDay < criterion.MaxValue + 7)
        //                                appropriateCriterion += _options.EducationDayOneWeekAfter;//Eğitim günü kriteri iki hafta öncesine kadar 30 puan etkiliyor.
        //                            else if (nearestDay < criterion.MinValue && nearestDay >= criterion.MinValue - 14)
        //                                appropriateCriterion += _options.EducationDayOneWeekAfter;//Eğitim günü kriteri bir hafta sonrasına kadar 30 puan etkiliyor.
        //                        }
        //                        #endregion
        //                        #region Favorilere Eklenmiş Eğitimler Kriteri
        //                        if (criterion.CriterionType == CriterionType.WishListEducations)
        //                        {
        //                            List<string> wishListItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
        //                            appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, wishListItemIds, userWishList);
        //                        }
        //                        #endregion
        //                        #region Satın Alınmış Eğitimler Kriteri
        //                        if (criterion.CriterionType == CriterionType.PurchasedEducations)
        //                        {
        //                            List<string> criterionItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
        //                            appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, criterionItemIds, userPurchasedEducations);
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                educationAppropriateCriterionRate.Add(
        //                    new EducationPoint { EducationId = education.Id, Point = appropriateCriterion }
        //                    );
        //            }
        //            #endregion

        //            //Yukarıda seçilen eğitimler içerisinden en çok kritere uyan 5 eğitim seçiliyor.
        //            var selectedEducations = educationAppropriateCriterionRate.OrderByDescending(x => x.Point)
        //                 .Take(5)
        //                 .ToDictionary(pair => pair.EducationId, pair => pair.Point);

        //            //Eğer seçilmiş eğitimler 5 taneyi tamamlayamıyorsa son eklenen 5 eğitim ile doldurulacak.
        //            var lastEducations = _context.Educations.OrderByDescending(x => x.CreatedDate).Where(x => x.IsActive).Take(10).ToList();
        //            int i = 0;
        //            int educationCount = _context.Educations.Count(x => x.IsActive);
        //            while (educationCount > 5 && selectedEducations.Count < 5)
        //            {
        //                if (!selectedEducations.ContainsKey(lastEducations[i].Id))
        //                {
        //                    selectedEducations.Add(lastEducations[i].Id, 0);
        //                }
        //                i++;
        //            }

        //            return FillSuggestedEducationList(selectedEducations);
        //        }
        //        return new List<SuggestedEducationVm>();
        //    }
        //    else
        //    {
        //        //Üye olmayanlar veya NBUY eğitimi almamış olanlar için son eklenen 5 eğitim.
        //        var educationsList = _context.Educations.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).Take(5)
        //         .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
        //         {
        //             Education = x,
        //             EducationPreviewMedia = y
        //         })
        //         .Join(_context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
        //         {
        //             Education = x.Education,
        //             EducationPreviewMedia = x.EducationPreviewMedia,
        //             CategoryName = y.Name
        //         }).ToList();

        //        var data = educationsList.Select(x => new SuggestedEducationVm
        //        {
        //            Base = new EducationBaseVm
        //            {
        //                Id = x.Education.Id,
        //                Name = x.Education.Name,
        //                Description = x.Education.Description,
        //                CategoryName = x.CategoryName,
        //                Level = EnumSupport.GetDescription(x.Education.Level),
        //                PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
        //                HoursPerDayText = x.Education.HoursPerDay.ToString(),
        //                DaysText = x.Education.Days.ToString(),
        //                DaysNumeric = x.Education.Days,
        //                HoursPerDayNumeric = x.Education.HoursPerDay
        //            },
        //            Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
        //            AppropriateCriterionCount = 0
        //        }
        // ).ToList();

        //        return data;
        //    }
        //}

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
                int nearestDay = 0;
                var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).First();
                nearestDay = educationDay.Day;

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
                            if (criterion.CriterionType == CriterionType.EducationDay)
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
                                                new EducationPoint { EducationId = education.Id, Point = appropriateCriterion / 2 }
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

        public List<SuggestedEducationVm> FillSuggestedEducationList(Dictionary<Guid, double> educationAndAppropriateCriterion)
        {
            var educationsList = _context.Educations.Where(x => educationAndAppropriateCriterion.Keys.Contains(x.Id) && x.IsActive)
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
            var category = _context.EducationCategories.First(x => x.Id == categoryId);
            return category.BaseCategoryId ?? category.Id;
        }
        #endregion

        #endregion

        #region 2 KULLANICI DAVRANIŞLARINA GÖRE EĞİTİM ÖNERİLERİ
        #region Closed
        ///// <summary>
        ///// Parametre olarak verilen sessionId ile incelenmiş eğitim listesini döner
        ///// </summary>
        ///// <param name="sessionId"></param>
        ///// <returns>List Education</returns>
        //public List<Education> GetViewingEducationsBySessionId(string sessionId)
        //{
        //    List<TransactionLog> transactionLogs = new List<TransactionLog>();
        //    List<Guid> educationIds = new List<Guid>();

        //    var count = _elasticClient.Count<TransactionLog>(s =>
        //    s.Query(
        //        q =>
        //        q.Term(t => t.SessionId, sessionId) &&
        //        q.Term(t => t.ControllerName, "course") &&
        //        q.Term(t => t.ActionName, "details")));

        //    var result = _elasticClient.Search<TransactionLog>(s =>
        //    s.Size((int)count.Count)
        //    .Query(
        //        q =>
        //        q.Term(t => t.SessionId, sessionId) &&
        //        q.Term(t => t.ControllerName, "course") &&
        //        q.Term(t => t.ActionName, "details")));

        //    if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
        //    {
        //        foreach (var log in result.Documents)
        //        {
        //            if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
        //                educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
        //        }
        //    }
        //    return _context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        //}
        ///// <summary>
        ///// Parametre olarak verilen userId ile incelenmiş eğitim listesini döner
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns>List Education</returns>
        //public List<Education> GetViewingEducationsByUserId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return new List<Education>();


        //    var transactionLogs = new List<TransactionLog>();
        //    var educationIds = new List<Guid>();
        //    var count = _elasticClient.Count<TransactionLog>(s =>
        //   s.Query(
        //       q =>
        //       q.Term(t => t.UserId, userId) &&
        //       q.Term(t => t.ControllerName, "course") &&
        //       q.Term(t => t.ActionName, "details")));

        //    var result = _elasticClient.Search<TransactionLog>(s =>
        //    s.Size((int)count.Count)
        //    .Query(
        //       q =>
        //       q.Term(t => t.UserId, userId) &&
        //       q.Term(t => t.ControllerName, "course") &&
        //       q.Term(t => t.ActionName, "details")));

        //    if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
        //    {
        //        foreach (var log in result.Documents)
        //        {
        //            if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
        //                educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
        //        }
        //    }
        //    return _context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        //}
        ///// <summary>
        ///// User Id ile incelenmiş eğitimlerin kaç adet incelendiğini döner.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns>Dictionary<EğitimId,İncelenme sayısı></returns>
        //public Dictionary<Education, int> EducationDetailViewsCountByUserId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return new Dictionary<Education, int>();

        //    Dictionary<Guid, int> retVal = new Dictionary<Guid, int>();
        //    Dictionary<Education, int> viewingInformation = new Dictionary<Education, int>();
        //    var count = _elasticClient.Count<TransactionLog>(s =>
        //   s.Query(
        //       q =>
        //       q.Term(t => t.UserId, userId) &&
        //       q.Term(t => t.ControllerName, "course") &&
        //       q.Term(t => t.ActionName, "details")));

        //    var result = _elasticClient.Search<TransactionLog>(s =>
        //    s.Size((int)count.Count)
        //    .Query(
        //        q =>
        //        q.Term(t => t.UserId, userId) &&
        //        q.Term(t => t.ControllerName, "course") &&
        //        q.Term(t => t.ActionName, "details")));

        //    if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
        //    {
        //        foreach (TransactionLog log in result.Documents)
        //        {
        //            if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
        //            {
        //                Guid educationId = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
        //                if (retVal.ContainsKey(educationId))
        //                    retVal[educationId]++;
        //                else
        //                    retVal.Add(educationId, 1);
        //            }
        //        }
        //    }
        //    var educations = _context.Educations.Where(x => retVal.Keys.Contains(x.Id)).ToList();
        //    foreach (var val in retVal)
        //    {
        //        viewingInformation.Add(educations.First(x => x.Id == val.Key), val.Value);
        //    }

        //    return viewingInformation;

        //}

        ///// <summary>
        ///// UserId ile arama yapılmış kelimeleri listeler.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns>List string</returns>
        //public List<string> GetSearchedTextsByUserId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return new List<string>();

        //    List<string> texts = new List<string>();

        //    var searchedTextCount = _elasticClient.Count<TransactionLog>(s =>
        //    s.Query(
        //        q =>
        //        q.Term(t => t.UserId, userId) &&
        //        q.Term(t => t.ControllerName, "browser") &&
        //        q.Term(t => t.ActionName, "getcourses")));

        //    var result = _elasticClient.Search<TransactionLog>(s =>
        //    s.Size((int)searchedTextCount.Count)
        //    .Query(q =>
        //        q.Term(t => t.UserId, userId) &&
        //        q.Term(t => t.ControllerName, "browser") &&
        //        q.Term(t => t.ActionName, "getcourses")));

        //    if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
        //    {
        //        foreach (var log in result.Documents)
        //        {
        //            if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchText"))
        //                texts.Add(JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchText").ParameterValue));
        //            //Todo aranılan kelime ile bulunacak olan eğitimler listelenebilir (_unitOfWork.Education.GetInfiniteScrollSearchResults(categoryName, searchText, page, order, filter);)
        //        }
        //    }
        //    return texts.Distinct().ToList();

        //}
        #endregion
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
            #region Clear
            //if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            //{
            //    foreach (var log in result.Documents)
            //    {//Aranılarak incelenmiş eğitimler
            //        if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchKey"))
            //        {
            //            string key = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchKey").ParameterValue);
            //            int totalKeySearched = getAllSearching[key];

            //            Guid Id = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
            //            if (model.SearchedEducations.Any(x => x.Key == key))
            //            {
            //                var eDetail = new EducationDetail();
            //                eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, model.TotalEducationSearchCount);
            //                eDetail.Id = Id;
            //                SearchedEducationList ed = model.SearchedEducations.First(x => x.Key == key);
            //                ed.ViewedCount++;
            //                ed.EducationDetails.Add(eDetail);
            //            }
            //            else
            //            {
            //                var eDetail = new EducationDetail();
            //                eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, model.TotalEducationSearchCount);
            //                eDetail.Id = Id;
            //                var sE = new SearchedEducationList();
            //                sE.Key = key;
            //                sE.ViewedCount = 1;
            //                sE.EducationDetails.Add(eDetail);
            //                model.SearchedEducations.Add(sE);
            //            }
            //        }
            //        //Direkt incelenmiş eğitimler
            //        if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId") && !log.Parameters.Any(x => x.ParameterName == "searchKey"))
            //        {
            //            Guid Id = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
            //            if (model.ViewingEducations.Any(x => x.EducationId == Id))
            //            {
            //                ViewingEducation current = model.ViewingEducations.First(x => x.EducationId == Id);
            //                current.ViewingCount++;
            //                current.Point = CalculateViewedEducationPoint(current.ViewingCount, totalEducationViewCount);
            //            }
            //            else
            //            {
            //                model.ViewingEducations.Add(new ViewingEducation
            //                {
            //                    EducationId = Id,
            //                    ViewingCount = 1,
            //                    Point = CalculateViewedEducationPoint(1, totalEducationViewCount)
            //                });
            //            }
            //        }

            //    }
            //}
            #endregion
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
        private List<SearchedEducationList> GetSearchedEducations(ISearchResponse<TransactionLog> result, string userId)
        {
            List<SearchedEducationList> model = new List<SearchedEducationList>();
            Dictionary<string, int> getAllSearching = GetAllSearchedKeyAndSearchCount(userId);
            int totalEducationSearchCount = CountOfEducationsSearchedAndViewed(result, getAllSearching);
            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {//Aranılarak incelenmiş eğitimler
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        string key = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchKey").ParameterValue);
                        int totalKeySearched = getAllSearching[key];

                        Guid Id = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
                        if (model.Any(x => x.Key == key))
                        {
                            var eDetail = new EducationDetail();
                            eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, totalEducationSearchCount);
                            eDetail.Id = Id;
                            eDetail.Education = _context.Educations.FirstOrDefault(x => x.Id == Id);
                            SearchedEducationList ed = model.First(x => x.Key == key);
                            ed.EducationDetails.Add(eDetail);
                        }
                        else
                        {
                            var eDetail = new EducationDetail();
                            eDetail.Point = CalculateSearchedKeyPoint(totalKeySearched, totalEducationSearchCount);
                            eDetail.Id = Id;
                            eDetail.Education = _context.Educations.FirstOrDefault(x => x.Id == Id);
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
        private List<ViewingEducation> GetViewingEducations(ISearchResponse<TransactionLog> result)
        {
            List<ViewingEducation> model = new List<ViewingEducation>();
            int totalEducationViewCount = GetEducationViewTotalCount(result);
            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId") && !log.Parameters.Any(x => x.ParameterName == "searchKey"))
                    {
                        Guid Id = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
                        if (model.Any(x => x.EducationId == Id))
                        {
                            ViewingEducation current = model.First(x => x.EducationId == Id);
                            current.Education = _context.Educations.FirstOrDefault(x => x.Id == Id);
                            current.ViewingCount++;
                            current.Point = CalculateViewedEducationPoint(current.ViewingCount, totalEducationViewCount);
                        }
                        else
                        {
                            model.Add(new ViewingEducation
                            {
                                EducationId = Id,
                                Education =_context.Educations.FirstOrDefault(x=>x.Id == Id),
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
            var educations = _context.Educations.Where(x => x.IsActive).ToList();
            //İncelenmiş eğitimler ve puanları
            foreach (var education in viewingEducations)
            {
                viewingEducationPoints.Add(new EducationPoint
                {
                    EducationId = education.EducationId,
                    Point = education.Point
                });
            }
            //Aranılarak incelenmiş eğitimler ve puanları
            foreach (var key in searchedEducations)
            {
                foreach (var detail in key.EducationDetails)
                {
                    var education = searchedEducationPoints.FirstOrDefault(x => x.EducationId == detail.Id);

                    if (education == null)
                    {
                        searchedEducationPoints.Add(new EducationPoint
                        {
                            EducationId = detail.Id,
                            Point = detail.Point
                        });
                    }
                    else
                    {
                        if (detail.Point > education.Point)
                        {
                            searchedEducationPoints.First(x => x.EducationId == detail.Id).Point = detail.Point;
                        }
                    }
                }
            }

            retVal.AddRange(viewingEducationPoints);
            retVal.AddRange(searchedEducationPoints);
            retVal = retVal.GroupBy(x => x.EducationId).Select(y => new EducationPoint { EducationId = y.Key,Education = educations.First(x=>x.Id ==y.Key), Point = (y.Sum(x => x.Point)) / 2 }).ToList();

            return retVal;
        }

        /// <summary>
        /// Aranılarak inceleme sayısını döner
        /// </summary>
        /// <param name="result"></param>
        /// <param name="getAllSearching"></param>
        /// <returns></returns>
        private int CountOfEducationsSearchedAndViewed(ISearchResponse<TransactionLog> result, Dictionary<string, int> getAllSearching)
        {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
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
        private int GetEducationViewTotalCount(ISearchResponse<TransactionLog> result)
        {
            int total = 0;
            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
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
        private ISearchResponse<TransactionLog> GetViewedEducations(string userId)
        {
            var count = _elasticClient.Count<TransactionLog>(s =>
            s.Query(
                q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "course") &&
                q.Term(t => t.ActionName, "details")));

            var result = _elasticClient.Search<TransactionLog>(s =>
            s.Size((int)count.Count)
            .Query(
                q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "course") &&
                q.Term(t => t.ActionName, "details")));
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
            var searchedTextCount = _elasticClient.Count<TransactionLog>(s =>
            s.Query(
                q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "browser") &&
                q.Term(t => t.ActionName, "getcourses")));
            var searchedTexts = _elasticClient.Search<TransactionLog>(s =>
            s.Size((int)searchedTextCount.Count)
            .Query(q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "browser") &&
                q.Term(t => t.ActionName, "getcourses")));

            if (searchedTexts.IsValid && searchedTexts.Documents != null && searchedTexts.Documents.Count > 0)
            {
                foreach (var log in searchedTexts.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchText"))
                    {
                        string sText = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchText").ParameterValue);

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




