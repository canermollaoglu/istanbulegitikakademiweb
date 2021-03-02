using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Business.Repositories.BlogRepositories;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Data;
using NitelikliBilisim.Notificator.Services;

namespace NitelikliBilisim.Business.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Repositories
        private readonly NbDataContext _context;
        private EducationCategoryRepository _educationCategoryRepository;
        private EducationTagRepository _educationTagRepository;
        private EducationRepository _education;
        private EducationMediaItemRepository _educationMediaItem;
        private EducationPartRepository _educationPart;
        private EducationGainRepository _educationGain;
        private EducatorRepository _educator;
        private EducatorSocialMediaRepository _educatorSocialMedia;
        private StudentEducationInfoRepository _studentEducationInfo;
        private CustomerRepository _customerRepository;
        private BridgeEducationEducatorRepository _bridgeEducationEducatorRepository;
        private EducationGroupRepository _educationGroupRepository;
        private EducationHostRepository _educationHostRepository;
        private WeekDaysOfGroupRepository _weekDaysOfGroupRepository;
        private TicketRepository _ticketRepository;
        private SaleRepository _saleRepository;
        private TempSaleDataRepository _tempSaleDataRepository;
        private GroupLessonDayRepository _groupLessonDayRepository;
        private ReportRepository _reportRepository;
        private GroupAttendanceRepository _groupAttendanceRepository;
        private EmailRepository _emailRepository;
        private EducatorSalaryRepository _educatorSalaryRepository;
        private EducatorCertificateRepository _educatorCertificateRepository;
        private StateRepository _stateRepository;
        private CityRepository _cityRepository;
        private AddressRepository _addressRepository;
        private EducationHostImageRepository _educationHostImageRepository;
        private OffDayRepository _offDayRepository;
        private EducationDayRepository _educationDayRepository;
        private EducationSuggestionCriterionRepository _educationSuggestionCriterionRepository;
        private WishListRepository _wishListItemRepository;
        private SuggestionRepository _suggestionRepository;
        private BlogPostRepository _blogPostRepository;
        private BlogCategoryRepository _blogCategoryRepository;
        private BlogTagRepository _blogTagRepository;
        private InvoiceRepository _invoiceRepository;
        private InvoiceDetailRepository _invoiceDetailRepository;
        private GroupExpenseRepository _groupExpenseRepository;
        private GroupExpenseTypeRepository _groupExpenseTypeRepository;
        private EducationHostClassroomRepository _educationHostClassroomRepository;
        private EducationPromotionCodeRepository _educationPromotionCodeRepository;
        private EducationPromotionItemRepository _educationPromotionItemRepository;
        private EducationPromotionConditionRepository _educationPromotionConditionRepository;
        private EducatorApplicationRepository _educatorApplicationRespository;
        private CorporateMembershipApplicationRepository _corporateMembershipApplicationRepository;
        private EducationCommentRepository _educationCommentRepository;
        private SubscriptionBlogRepository _subscriptionBlogRepository;
        private SubscriptionNewsletterRepository _subscriptionNewsletterRepository;
        private ContactFormRepository _contactFormRepository;
        private FeaturedCommentRepository _featuredCommentRepository;
        private BannerAdsRepository _bannerAdsRepository;
        private CustomerCertificateRepository _customerCertificateRepository;
        private DashboardRepository _dashboardRepository;
        private CampaignRepository _campaignRepository;
        #endregion
        #region Mongo Repositories
        private BlogViewLogRepository _blogViewLogRepository;
        private CampaignLogRepository _campaignLogRepository;
        private TransactionLogRepository _transactionLogRepository;
        #endregion
        private IConfiguration _configuration;
        private IEmailSender _emailSender;
        public UnitOfWork(NbDataContext context,TransactionLogRepository transactionLogRepository,CampaignLogRepository campaignLogRepository,BlogViewLogRepository blogViewLogRepository,IConfiguration configuration,IEmailSender emailSender)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _blogViewLogRepository = blogViewLogRepository;
            _campaignLogRepository = campaignLogRepository;
            _transactionLogRepository = transactionLogRepository;
        }
        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public CampaignRepository Campaign => _campaignRepository ??= new CampaignRepository(_context,_campaignLogRepository);
        public DashboardRepository Dashboard => _dashboardRepository ??= new DashboardRepository(_context);
        public CustomerCertificateRepository CustomerCertificate => _customerCertificateRepository ??= new CustomerCertificateRepository(_context);
        public FeaturedCommentRepository FeaturedComment => _featuredCommentRepository ??= new FeaturedCommentRepository(_context);
        public ContactFormRepository ContactForm => _contactFormRepository??= new ContactFormRepository(_context);
        public SubscriptionBlogRepository SubscriptionBlog => _subscriptionBlogRepository ??= new SubscriptionBlogRepository(_context);
        public SubscriptionNewsletterRepository SubscriptionNewsletter => _subscriptionNewsletterRepository ??= new SubscriptionNewsletterRepository(_context);
        public EducationCommentRepository EducationComment => _educationCommentRepository ??= new EducationCommentRepository(_context);
        public CorporateMembershipApplicationRepository CorporateMembershipApplication => _corporateMembershipApplicationRepository ??= new CorporateMembershipApplicationRepository(_context);
        public EducatorApplicationRepository EducatorApplication => _educatorApplicationRespository ??= new EducatorApplicationRepository(_context);
        public EducationPromotionConditionRepository EducationPromotionCondition => _educationPromotionConditionRepository ??= new EducationPromotionConditionRepository(_context);
        public EducationPromotionItemRepository EducationPromotionItem => _educationPromotionItemRepository ??= new EducationPromotionItemRepository(_context);
        public EducationPromotionCodeRepository EducationPromotionCode => _educationPromotionCodeRepository ??= new EducationPromotionCodeRepository(_context);
        public EducationHostClassroomRepository ClassRoom => _educationHostClassroomRepository ??= new EducationHostClassroomRepository(_context);
        public GroupExpenseRepository GroupExpense => _groupExpenseRepository ??= new GroupExpenseRepository(_context);
        public GroupExpenseTypeRepository GroupExpenseType => _groupExpenseTypeRepository ??= new GroupExpenseTypeRepository(_context);
        public InvoiceDetailRepository InvoiceDetail => _invoiceDetailRepository ??= new InvoiceDetailRepository(_context);
        public InvoiceRepository Invoice => _invoiceRepository ??= new InvoiceRepository(_context);
        public SuggestionRepository Suggestions => _suggestionRepository ??= new SuggestionRepository(_context, _transactionLogRepository,_configuration);
        public EducationCategoryRepository EducationCategory => _educationCategoryRepository ??= new EducationCategoryRepository(_context);

        public EducationTagRepository EducationTag => _educationTagRepository ??= new EducationTagRepository(_context);

        public EducationRepository Education => _education ??= new EducationRepository(_context,_configuration);

        public EducationMediaItemRepository EducationMedia => _educationMediaItem ??= new EducationMediaItemRepository(_context);

        public EducationPartRepository EducationPart => _educationPart ??= new EducationPartRepository(_context);

        public EducationGainRepository EducationGain => _educationGain ??= new EducationGainRepository(_context);

        public EducatorRepository Educator => _educator ??= new EducatorRepository(_context);

        public EducatorSocialMediaRepository EducatorSocialMedia => _educatorSocialMedia ??= new EducatorSocialMediaRepository(_context);

        public StudentEducationInfoRepository StudentEducationInfo => _studentEducationInfo ??= new StudentEducationInfoRepository(_context);

        public CustomerRepository Customer => _customerRepository ??= new CustomerRepository(_context);

        public BridgeEducationEducatorRepository Bridge_EducationEducator => _bridgeEducationEducatorRepository ??= new BridgeEducationEducatorRepository(_context);
        public EducationGroupRepository EducationGroup => _educationGroupRepository ??= new EducationGroupRepository(_context,_configuration);

        public EducationHostRepository EducationHost => _educationHostRepository ??= new EducationHostRepository(_context);

        public WeekDaysOfGroupRepository WeekDaysOfGroup => _weekDaysOfGroupRepository ??= new WeekDaysOfGroupRepository(_context);

        public TicketRepository Ticket => _ticketRepository ??= new TicketRepository(_context);

        public SaleRepository Sale
        {
            get
            {
                return _saleRepository ?? (_saleRepository = new SaleRepository(_context,_emailSender,_configuration));
            }
        }
        public TempSaleDataRepository TempSaleData
        {
            get
            {
                return _tempSaleDataRepository ?? (_tempSaleDataRepository = new TempSaleDataRepository(_context));
            }
        }
        public GroupLessonDayRepository GroupLessonDay
        {
            get
            {
                return _groupLessonDayRepository ?? (_groupLessonDayRepository = new GroupLessonDayRepository(_context));
            }
        }
        public ReportRepository Report
        {
            get
            {
                return _reportRepository ?? (_reportRepository = new ReportRepository(_context));
            }
        }
        public GroupAttendanceRepository GroupAttendance
        {
            get
            {
                return _groupAttendanceRepository ?? (_groupAttendanceRepository = new GroupAttendanceRepository(_context));
            }
        }
        public EmailRepository EmailHelper
        {
            get
            {
                return _emailRepository ?? (_emailRepository = new EmailRepository(_context,_configuration));
            }
        }
        public EducatorSalaryRepository Salary
        {
            get
            {
                return _educatorSalaryRepository ?? (_educatorSalaryRepository = new EducatorSalaryRepository(_context));
            }
        }
        
        public EducatorCertificateRepository EducatorCertificate
        {
            get
            {
                return _educatorCertificateRepository ?? (_educatorCertificateRepository = new EducatorCertificateRepository(_context));
            }
        }
        
        public StateRepository State => _stateRepository ??= new StateRepository(_context);
        public CityRepository City => _cityRepository ??= new CityRepository(_context);
        public AddressRepository Address => _addressRepository ??= new AddressRepository(_context);
        public EducationHostImageRepository EducationHostImage => _educationHostImageRepository ??= new EducationHostImageRepository(_context);
        public OffDayRepository OffDay => _offDayRepository ??= new OffDayRepository(_context);
        public EducationDayRepository EducationDay => _educationDayRepository ??= new EducationDayRepository(_context);
        public EducationSuggestionCriterionRepository EducationSuggestionCriterion => _educationSuggestionCriterionRepository ??= new EducationSuggestionCriterionRepository(_context);
        public WishListRepository WishListItem => _wishListItemRepository ??= new WishListRepository(_context);
        public BlogPostRepository BlogPost => _blogPostRepository ??= new BlogPostRepository(_context, _blogViewLogRepository);
        public BlogCategoryRepository BlogCategory => _blogCategoryRepository ??= new BlogCategoryRepository(_context);
        public BlogTagRepository BlogTag => _blogTagRepository ??= new BlogTagRepository(_context);
        public BannerAdsRepository BannerAds => _bannerAdsRepository ??= new BannerAdsRepository(_context);
     }
}
