using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.MongoOptions.Entities;
using NitelikliBilisim.Core.ViewModels.Suggestion;
using NitelikliBilisim.Data;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NitelikliBilisim.RefreshSuggestedEducations
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TransactionLogRepository _transactionLog;
        private readonly SuggestedEducationsRepository _suggestedEducations;
        private readonly SuggestionSystemOptions _options;
        private readonly IEmailSender _sender;
        private readonly bool _isSendEmail;
        private readonly string[] _adminEmails;

        public IServiceProvider Services { get; }
        public Worker(IServiceProvider services,IEmailSender sender, ILogger<Worker> logger, SuggestedEducationsRepository suggestedEducationsRepository, TransactionLogRepository transactionLogRepository, IConfiguration configuration)
        {
            Services = services;
            _logger = logger;
            _sender = sender;
            _transactionLog = transactionLogRepository;
            _suggestedEducations = suggestedEducationsRepository;
            _options = configuration.GetSection("EducationSuggestionSystemOptions").Get<SuggestionSystemOptions>();
            _isSendEmail = configuration.GetValue<bool>("GeneralSettings:IsSendEmail");
            _adminEmails = configuration.GetValue<string>("GeneralSettings:AdminEmails").Split(";");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = Services.CreateScope())
            {

                try
                {
                    var _context = scope.ServiceProvider.GetRequiredService<NbDataContext>();
                    var allEducations = _context.Educations.Include(c => c.Category).Include(x => x.EducationSuggestionCriterions).Where(x => x.IsActive).ToList();
                    var customerIds = _context.Customers.Select(x => x.Id).ToList();

                    foreach (var userId in customerIds)
                    {

                        var result = _transactionLog.GetList(x => x.UserId == userId && x.ControllerName == "Course" && x.ActionName == "Details");
                        var groupStudents = _context.Bridge_GroupStudents.Include(x => x.Group).Where(x=>x.Id2 == userId).ToList();
                        var educations = allEducations.Where(x => !groupStudents.Any(y => y.Group.EducationId == x.Id)).ToList();

                        #region Arama kelimesi, arama sayýsý ve bu arama ile incelenmiþ eðitimlerin id ve puanlarý //
                        List<SearchedEducationList> model = new List<SearchedEducationList>();
                        var searchedTexts = _transactionLog.GetList(x => x.UserId == userId && x.ControllerName == "Course" && x.ActionName == "GetCourses");
                        Dictionary<string, int> getAllSearching = GetAllSearchedKeyAndSearchCount(userId, searchedTexts);
                        int totalEducationSearchCount = CountOfEducationsSearchedAndViewed(result, getAllSearching);
                        if (result != null && result.Count > 0)
                        {
                            foreach (var log in result)
                            {//Aranýlarak incelenmiþ eðitimler
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
                        #endregion
                        var searched = model;
                        #region  Ýncelenmiþ eðitimleri, incelenme sayýsýný ve puanlarý
                        List<ViewingEducation> ve = new List<ViewingEducation>();
                        int totalEducationViewCount = GetEducationViewTotalCount(result);
                        if (result != null && result.Count > 0)
                        {
                            foreach (var log in result)
                            {
                                if (log.Parameters != null && log.Parameters.Any(x => x.ParameterName == "seoUrl") && !log.Parameters.Any(x => x.ParameterName == "searchKey"))
                                {
                                    string seoUrl = JsonConvert.DeserializeObject<string>(log.Parameters.First(x => x.ParameterName == "seoUrl").ParameterValue);
                                    if (ve.Any(x => x.SeoUrl == seoUrl))
                                    {
                                        ViewingEducation current = ve.First(x => x.SeoUrl == seoUrl);
                                        current.Education = _context.Educations.FirstOrDefault(x => x.SeoUrl == seoUrl);
                                        current.ViewingCount++;
                                        current.Point = CalculateViewedEducationPoint(current.ViewingCount, totalEducationViewCount);
                                    }
                                    else
                                    {
                                        ve.Add(new ViewingEducation
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
                        #endregion
                        var viewed = ve;
                        #region Kriterlere göre önerilern eðitimler
                        List<EducationPoint> educationAppropriateCriterionRate = new List<EducationPoint>();
                        var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
                        //Kullanýcý kayýtlý ise ve NBUY öðrencisi ise 
                        if (studentEducationInfo != null)
                        {
                            var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
                            //Müþterinin en yakýn eðitim günü. (Müþteri hafta sonu veya tatil günü sisteme giriþ yaptýðýný varsayarak geçmiþ en yakýn gün baz alýndý.)

                            var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).FirstOrDefault();
                            int nearestDay = educationDay != null ? educationDay.Day : 0;

                            /*Müþterinin NBUY eðitimi aldýðý kategoriye göre eðitim listesi.*/
                            //Todo burada yalnýzca nbuy kategorisi mi geçerli olacak yoksa tüm kategorilerdeki eðitimler dikkate alýnacak mý konusunun netleþtirilmesi gerekli.
                            //.Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value);
                            #region Favori eklenen eðitimler
                            List<string> userWishList = _context.Wishlist.Where(x => x.Id == customer.Id).Include(x => x.Education).Select(x => x.Education.Id.ToString().ToLower()).ToList();
                            #endregion
                            #region Satýn alýnan eðitimler
                            List<string> userPurchasedEducations = new List<string>();
                            var tickets = _context.Tickets
                            .Where(x => x.OwnerId == customer.Id)
                            .ToList();
                            tickets.ForEach(x => userPurchasedEducations.Add(x.EducationId.ToString().ToLower()));
                            #endregion

                            #region Kriterlerin uygunluðunun kontrolü
                            foreach (var education in educations)
                            {
                                int appropriateCriterion = 0;
                                #region Kriterlere göre eðitim önerileri
                                if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                                {
                                    foreach (var criterion in education.EducationSuggestionCriterions)
                                    {
                                        #region Eðitim Günü Kriteri
                                        if (criterion.CriterionType == CriterionType.EducationDay && education.Category.BaseCategoryId == studentEducationInfo.CategoryId)
                                        {
                                            if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                                                appropriateCriterion += _options.EducationDayCriterion;//Eðitim günü kriteri %50 etkilediði için 100 puan üzerinden 50 puan ekleniyor.
                                            else if (nearestDay > criterion.MaxValue && nearestDay < criterion.MaxValue + 7)
                                                appropriateCriterion += _options.EducationDayTwoWeeksBefore;//Eðitim günü kriteri iki hafta öncesine kadar 30 puan etkiliyor.
                                            else if (nearestDay < criterion.MinValue && nearestDay >= criterion.MinValue - 14)
                                                appropriateCriterion += _options.EducationDayOneWeekAfter;//Eðitim günü kriteri bir hafta sonrasýna kadar 30 puan etkiliyor.
                                        }
                                        #endregion
                                        #region Favorilere Eklenmiþ Eðitimler Kriteri
                                        if (criterion.CriterionType == CriterionType.WishListEducations)
                                        {
                                            List<string> wishListItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                            appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, wishListItemIds, userWishList);
                                        }
                                        #endregion
                                        #region Satýn Alýnmýþ Eðitimler Kriteri
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
                        #endregion
                        var criterionBased = educationAppropriateCriterionRate;

                        var userActionBased = CalculateUserActionBasedSuggestionTotalPoint(searched, viewed, educations);

                        var educationPoints = CalculateTotalSuggestionPoint(criterionBased, userActionBased, educations);

                        var selectedEducations = educationPoints.OrderByDescending(x => x.Point)
                                     .Take(4)
                                     .ToDictionary(pair => pair.SeoUrl, pair => pair.Point);
                        var lastEducations = educations.OrderByDescending(x => x.CreatedDate).Where(x => x.IsActive).Take(10).ToList();
                        int i = 0;
                        int educationCount = educations.Count();
                        while (educationCount > 4 && selectedEducations.Count() < 4)
                        {
                            if (!selectedEducations.ContainsKey(lastEducations[i].SeoUrl))
                            {
                                selectedEducations.Add(lastEducations[i].SeoUrl, 0);
                            }
                            i++;
                        }
                        var oldRow = _suggestedEducations.GetByUserId(userId);
                        if (oldRow == null)
                        {
                            _suggestedEducations.Create(new MSuggestedEducation { UserId = userId, Educations = selectedEducations, });
                        }
                        else
                        {
                            oldRow.Educations = selectedEducations;
                            _suggestedEducations.Update(oldRow.Id.ToString(), oldRow);
                        }

                    }

                    if (_isSendEmail)
                    {
                        await _sender.SendAsync(new EmailMessage
                        {
                            Body = "Eðitim önerileri güncelleme servisi baþarýyla tamamlandý.",
                            Subject = "Nitelikli Biliþim Eðitim Öneri Servisi - Baþarýlý",
                            Contacts = _adminEmails
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Hata: " + ex.Message, DateTimeOffset.Now);
                    if (_isSendEmail)
                    {
                        await _sender.SendAsync(new EmailMessage
                        {
                            Body = "Eðitim önerileri güncelleme servisi hata aldý.<br/> Hata:" + ex.Message,
                            Subject = "Nitelikli Biliþim Eðitim Öneri Servisi - Hata",
                            Contacts = _adminEmails
                        });
                    }
                }
            }
           await Task.CompletedTask;

        }
        /// <summary>
        /// Kriter bazlý ve Kullanýcý davranýþlarý bazlý eðitim puanlarýný alarak nihai puanlarý hesaplar.
        /// </summary>
        /// <param name="criterionBased"></param>
        /// <param name="userActionBased"></param>
        /// <returns></returns>
        private List<EducationPoint> CalculateTotalSuggestionPoint(List<EducationPoint> criterionBased, List<EducationPoint> userActionBased, List<Education> educations)
        {
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

        /// <summary>
        /// Bu method eðitimde belirtilen Favori eðitim ve Satýn alýnan eðitim kriterlerindeki eðitimleri 
        /// ve kullanýcýnýn satýn aldýðý ve favoriye eklediði eðitimleri alarak 
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
        /// <summary>
        /// Kullanýcý davranýþlarýna göre eðitim puanlarýný döner
        /// </summary>
        /// <param name="searchedEducations">Aranýlarak incelenen eðitimler</param>
        /// <param name="viewingEducations">Direkt incelenen eðitimler</param>
        /// <returns></returns>
        private List<EducationPoint> CalculateUserActionBasedSuggestionTotalPoint(List<SearchedEducationList> searchedEducations, List<ViewingEducation> viewingEducations, List<Education> educations)
        {
            List<EducationPoint> retVal = new List<EducationPoint>();
            List<EducationPoint> viewingEducationPoints = new List<EducationPoint>();
            List<EducationPoint> searchedEducationPoints = new List<EducationPoint>();
            List<EducationPoint> totalPoints = new List<EducationPoint>();

            //Ýncelenmiþ eðitimler ve puanlarý
            foreach (var education in viewingEducations)
            {
                viewingEducationPoints.Add(new EducationPoint
                {
                    SeoUrl = education.SeoUrl,
                    Point = education.Point
                });
            }
            //Aranýlarak incelenmiþ eðitimler ve puanlarý
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
        /// userId bazýnda arama yapýlan kelimeleri ve arama sayýlarýný döner.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Dictionary<string, int> GetAllSearchedKeyAndSearchCount(string userId, List<TransactionLog> searchedTexts)
        {
            Dictionary<string, int> allSearchings = new Dictionary<string, int>();

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

        /// <summary>
        /// Aranýlarak inceleme sayýsýný döner
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
                {//Aranýlarak incelenmiþ eðitimler
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



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
