using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels;
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
        public SuggestionRepository(NbDataContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        #region 1 KRİTERLERE GÖRE EĞİTİM ÖNERİLERİ

        /// <summary>
        /// Kriterlere göre önerilen eğitimler
        /// </summary>
        /// <param name="isLoggedIn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SuggestedEducationVm> GetSuggestedEducationList(bool isLoggedIn, string userId)
        {
            //Eğitim Id ve uygun kriterlerin sayısı. Eğitimler uygun kriter sayısına göre listelenecek.
            Dictionary<Guid, int> educationAndAppropriateCriterion = new Dictionary<Guid, int>();
            //Kullanıcının NBUY eğitimi alıp almadığını kontrol ediyoruz.
            var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
            //Kullanıcı kayıtlı ise ve NBUY öğrencisi ise 
            if (isLoggedIn && studentEducationInfo != null)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
                //Müşterinin en yakın eğitim günü. (Müşteri hafta sonu veya tatil günü sisteme giriş yaptığını varsayarak geçmiş en yakın gün baz alındı.)
                int nearestDay = 0;
                var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).First();
                nearestDay = educationDay.Day;

                /*Müşterinin NBUY eğitimi aldığı kategoriye göre eğitim listesi.*/
                var educations = _context.Educations.Include(c => c.Category).Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value).Include(x => x.EducationSuggestionCriterions).Where(x => x.IsActive);
                #region Favori eklenen eğitimler
                List<string> userWishList = _context.Wishlist.Where(x => x.Id == customer.Id).Include(x => x.Education).Select(x => x.Education.Id.ToString()).ToList();
                #endregion
                #region Satın alınan eğitimler
                List<string> userPurchasedEducations = new List<string>();
                var tickets = _context.Tickets
                .Where(x => x.OwnerId == customer.Id)
                .ToList();
                tickets.ForEach(x => userPurchasedEducations.Add(x.EducationId.ToString()));
                #endregion


                #region Kriterlerin uygunluğunun kontrolü
                foreach (var education in educations)
                {
                    int appropriateCriterion = 1;
                    #region Kriterlere göre eğitim önerileri
                    if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                    {
                        foreach (var criterion in education.EducationSuggestionCriterions)
                        {
                            #region Eğitim Günü Kriteri
                            if (criterion.CriterionType == CriterionType.EducationDay)
                            {
                                if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                                    appropriateCriterion++;
                            }
                            #endregion
                            #region Favorilere Eklenmiş Eğitimler Kriteri
                            if (criterion.CriterionType == CriterionType.WishListEducations)
                            {
                                List<string> wishListItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                appropriateCriterion = appropriateCriterion + SameElementCount(wishListItemIds, userWishList);
                            }
                            #endregion
                            #region Satın Alınmış Eğitimler Kriteri
                            if (criterion.CriterionType == CriterionType.PurchasedEducations)
                            {
                                List<string> criterionItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                appropriateCriterion = appropriateCriterion + SameElementCount(userPurchasedEducations, criterionItemIds);
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region Kullanıcı davranışlarına göre eğitim önerileri
                    //Bu alanda en çok incelenen eğitime veya en çok aranan eğitime +1 uygunluk puanı verilebilir.
                    #endregion
                    educationAndAppropriateCriterion.Add(education.Id, appropriateCriterion);
                }
                #endregion

                //Yukarıda seçilen eğitimler içerisinden en çok kritere uyan 5 eğitim seçiliyor.
                var selectedEducations = educationAndAppropriateCriterion.OrderByDescending(entry => entry.Value)
                     .Take(5)
                     .ToDictionary(pair => pair.Key, pair => pair.Value);

                //Eğer seçilmiş eğitimler 5 taneyi tamamlayamıyorsa son eklenen 5 eğitim ile doldurulacak.
                var lastEducations = _context.Educations.OrderByDescending(x => x.CreatedDate).Where(x => x.IsActive).Take(10).ToList();
                int i = 0;
                int educationCount = _context.Educations.Count(x => x.IsActive);
                while (educationCount > 5 && selectedEducations.Count < 5)
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
        }

        #region Suggested Educations Helper Method
        public int SameElementCount(List<string> first, List<string> second)
        {
            int count = 0;
            for (int i = 0; i < first.Count; i++)
                if (second.Contains(first[i].ToUpper()))
                    count++;
            return count;
        }

        public List<SuggestedEducationVm> FillSuggestedEducationList(Dictionary<Guid, int> educationAndAppropriateCriterion)
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

        /// <summary>
        /// Parametre olarak verilen sessionId ile incelenmiş eğitim listesini döner
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>List Education</returns>
        public List<Education> GetViewingEducationsBySessionId(string sessionId)
        {
            List<TransactionLog> transactionLogs = new List<TransactionLog>();
            List<Guid> educationIds = new List<Guid>();

            var count = _elasticClient.Count<TransactionLog>(s =>
            s.Query(
                q =>
                q.Term(t => t.SessionId, sessionId) &&
                q.Term(t => t.ControllerName, "course") &&
                q.Term(t => t.ActionName, "details")));

            var result = _elasticClient.Search<TransactionLog>(s =>
            s.Size((int)count.Count)
            .Query(
                q =>
                q.Term(t => t.SessionId, sessionId) &&
                q.Term(t => t.ControllerName, "course") &&
                q.Term(t => t.ActionName, "details")));

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
                        educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
                }
            }
            return _context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        }
        /// <summary>
        /// Parametre olarak verilen userId ile incelenmiş eğitim listesini döner
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List Education</returns>
        public List<Education> GetViewingEducationsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Education>();


            var transactionLogs = new List<TransactionLog>();
            var educationIds = new List<Guid>();
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

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
                        educationIds.Add(JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue));
                }
            }
            return _context.Educations.Where(x => educationIds.Contains(x.Id)).ToList();
        }
        /// <summary>
        /// User Id ile incelenmiş eğitimlerin kaç adet incelendiğini döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Dictionary<EğitimId,İncelenme sayısı></returns>
        public Dictionary<Education, int> EducationDetailViewsCountByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new Dictionary<Education, int>();

            Dictionary<Guid, int> retVal = new Dictionary<Guid, int>();
            Dictionary<Education, int> viewingInformation = new Dictionary<Education, int>();
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
                q.Term(t => t.UserId,userId) &&
                q.Term(t => t.ControllerName,"course") &&
                q.Term(t => t.ActionName,"details")));

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (TransactionLog log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "courseId"))
                    {
                        Guid educationId = JsonConvert.DeserializeObject<Guid>(log.Parameters.First(x => x.ParameterName == "courseId").ParameterValue);
                        if (retVal.ContainsKey(educationId))
                            retVal[educationId]++;
                        else
                            retVal.Add(educationId, 1);
                    }
                }
            }
            var educations = _context.Educations.Where(x => retVal.Keys.Contains(x.Id)).ToList();
            foreach (var val in retVal)
            {
                viewingInformation.Add(educations.First(x => x.Id == val.Key), val.Value);
            }

            return viewingInformation;

        }

        /// <summary>
        /// UserId ile arama yapılmış kelimeleri listeler.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List string</returns>
        public List<string> GetSearchedTextsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<string>();

            List<string> texts = new List<string>();

            var searchedTextCount = _elasticClient.Count<TransactionLog>(s =>
            s.Query(
                q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "browser") &&
                q.Term(t => t.ActionName, "getcourses")));

            var result = _elasticClient.Search<TransactionLog>(s =>
            s.Size((int)searchedTextCount.Count)
            .Query(q =>
                q.Term(t => t.UserId, userId) &&
                q.Term(t => t.ControllerName, "browser") &&
                q.Term(t => t.ActionName, "getcourses")));

            if (result.IsValid && result.Documents != null && result.Documents.Count > 0)
            {
                foreach (var log in result.Documents)
                {
                    if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "searchText"))
                        texts.Add(JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "searchText").ParameterValue));
                    //Todo aranılan kelime ile bulunacak olan eğitimler listelenebilir (_unitOfWork.Education.GetInfiniteScrollSearchResults(categoryName, searchText, page, order, filter);)
                }
            }
            return texts;

        }

        #endregion

    }
}
