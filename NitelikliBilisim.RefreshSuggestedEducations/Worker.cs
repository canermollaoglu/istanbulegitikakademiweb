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

                        #region Arama kelimesi, arama say�s� ve bu arama ile incelenmi� e�itimlerin id ve puanlar� //
                        List<SearchedEducationList> model = new List<SearchedEducationList>();
                        var searchedTexts = _transactionLog.GetList(x => x.UserId == userId && x.ControllerName == "Course" && x.ActionName == "GetCourses");
                        Dictionary<string, int> getAllSearching = GetAllSearchedKeyAndSearchCount(userId, searchedTexts);
                        int totalEducationSearchCount = CountOfEducationsSearchedAndViewed(result, getAllSearching);
                        if (result != null && result.Count > 0)
                        {
                            foreach (var log in result)
                            {//Aran�larak incelenmi� e�itimler
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
                        #region  �ncelenmi� e�itimleri, incelenme say�s�n� ve puanlar�
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
                        #region Kriterlere g�re �nerilern e�itimler
                        List<EducationPoint> educationAppropriateCriterionRate = new List<EducationPoint>();
                        var studentEducationInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
                        //Kullan�c� kay�tl� ise ve NBUY ��rencisi ise 
                        if (studentEducationInfo != null)
                        {
                            var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
                            //M��terinin en yak�n e�itim g�n�. (M��teri hafta sonu veya tatil g�n� sisteme giri� yapt���n� varsayarak ge�mi� en yak�n g�n baz al�nd�.)

                            var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).FirstOrDefault();
                            int nearestDay = educationDay != null ? educationDay.Day : 0;

                            /*M��terinin NBUY e�itimi ald��� kategoriye g�re e�itim listesi.*/
                            //Todo burada yaln�zca nbuy kategorisi mi ge�erli olacak yoksa t�m kategorilerdeki e�itimler dikkate al�nacak m� konusunun netle�tirilmesi gerekli.
                            //.Where(x => x.Category.BaseCategoryId == studentEducationInfo.CategoryId.Value || x.Category.Id == studentEducationInfo.CategoryId.Value);
                            #region Favori eklenen e�itimler
                            List<string> userWishList = _context.Wishlist.Where(x => x.Id == customer.Id).Include(x => x.Education).Select(x => x.Education.Id.ToString().ToLower()).ToList();
                            #endregion
                            #region Sat�n al�nan e�itimler
                            List<string> userPurchasedEducations = new List<string>();
                            var tickets = _context.Tickets
                            .Where(x => x.OwnerId == customer.Id)
                            .ToList();
                            tickets.ForEach(x => userPurchasedEducations.Add(x.EducationId.ToString().ToLower()));
                            #endregion

                            #region Kriterlerin uygunlu�unun kontrol�
                            foreach (var education in educations)
                            {
                                int appropriateCriterion = 0;
                                #region Kriterlere g�re e�itim �nerileri
                                if (education.EducationSuggestionCriterions != null && education.EducationSuggestionCriterions.Count > 0)
                                {
                                    foreach (var criterion in education.EducationSuggestionCriterions)
                                    {
                                        #region E�itim G�n� Kriteri
                                        if (criterion.CriterionType == CriterionType.EducationDay && education.Category.BaseCategoryId == studentEducationInfo.CategoryId)
                                        {
                                            if (nearestDay <= criterion.MaxValue && nearestDay >= criterion.MinValue)
                                                appropriateCriterion += _options.EducationDayCriterion;//E�itim g�n� kriteri %50 etkiledi�i i�in 100 puan �zerinden 50 puan ekleniyor.
                                            else if (nearestDay > criterion.MaxValue && nearestDay < criterion.MaxValue + 7)
                                                appropriateCriterion += _options.EducationDayTwoWeeksBefore;//E�itim g�n� kriteri iki hafta �ncesine kadar 30 puan etkiliyor.
                                            else if (nearestDay < criterion.MinValue && nearestDay >= criterion.MinValue - 14)
                                                appropriateCriterion += _options.EducationDayOneWeekAfter;//E�itim g�n� kriteri bir hafta sonras�na kadar 30 puan etkiliyor.
                                        }
                                        #endregion
                                        #region Favorilere Eklenmi� E�itimler Kriteri
                                        if (criterion.CriterionType == CriterionType.WishListEducations)
                                        {
                                            List<string> wishListItemIds = JsonConvert.DeserializeObject<string[]>(criterion.CharValue).ToList();
                                            appropriateCriterion = appropriateCriterion + (int)TotalSameElementPoint(criterion.CriterionType, wishListItemIds, userWishList);
                                        }
                                        #endregion
                                        #region Sat�n Al�nm�� E�itimler Kriteri
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
                            Body = "E�itim �nerileri g�ncelleme servisi ba�ar�yla tamamland�.",
                            Subject = "Nitelikli Bili�im E�itim �neri Servisi - Ba�ar�l�",
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
                            Body = "E�itim �nerileri g�ncelleme servisi hata ald�.<br/> Hata:" + ex.Message,
                            Subject = "Nitelikli Bili�im E�itim �neri Servisi - Hata",
                            Contacts = _adminEmails
                        });
                    }
                }
            }
           await Task.CompletedTask;

        }
        /// <summary>
        /// Kriter bazl� ve Kullan�c� davran��lar� bazl� e�itim puanlar�n� alarak nihai puanlar� hesaplar.
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
        /// Bu method e�itimde belirtilen Favori e�itim ve Sat�n al�nan e�itim kriterlerindeki e�itimleri 
        /// ve kullan�c�n�n sat�n ald��� ve favoriye ekledi�i e�itimleri alarak 
        /// bir puan d�nderir.
        /// </summary>
        /// <param name="criterionType"></param>
        /// <param name="criterionEducationList"></param>
        /// <param name="studentEducationList"></param>
        /// <returns></returns>
        public double TotalSameElementPoint(CriterionType criterionType, List<string> criterionEducationList, List<string> studentEducationList)
        {
            //Her bir kriter uyumu i�in eklenecek puan
            double criterionPoint = criterionType == CriterionType.WishListEducations ? _options.WishlistEducationsCriterion / criterionEducationList.Count : _options.PurchasedEducationsCriterion / criterionEducationList.Count;
            //Toplam puan
            double totalPoint = 0;
            for (int i = 0; i < criterionEducationList.Count; i++)
                if (studentEducationList.Contains(criterionEducationList[i].ToLower()))
                    totalPoint += criterionPoint;
            return totalPoint;
        }
        /// <summary>
        /// Kullan�c� davran��lar�na g�re e�itim puanlar�n� d�ner
        /// </summary>
        /// <param name="searchedEducations">Aran�larak incelenen e�itimler</param>
        /// <param name="viewingEducations">Direkt incelenen e�itimler</param>
        /// <returns></returns>
        private List<EducationPoint> CalculateUserActionBasedSuggestionTotalPoint(List<SearchedEducationList> searchedEducations, List<ViewingEducation> viewingEducations, List<Education> educations)
        {
            List<EducationPoint> retVal = new List<EducationPoint>();
            List<EducationPoint> viewingEducationPoints = new List<EducationPoint>();
            List<EducationPoint> searchedEducationPoints = new List<EducationPoint>();
            List<EducationPoint> totalPoints = new List<EducationPoint>();

            //�ncelenmi� e�itimler ve puanlar�
            foreach (var education in viewingEducations)
            {
                viewingEducationPoints.Add(new EducationPoint
                {
                    SeoUrl = education.SeoUrl,
                    Point = education.Point
                });
            }
            //Aran�larak incelenmi� e�itimler ve puanlar�
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
        /// userId baz�nda arama yap�lan kelimeleri ve arama say�lar�n� d�ner.
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
        /// Aran�larak inceleme say�s�n� d�ner
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
                {//Aran�larak incelenmi� e�itimler
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
