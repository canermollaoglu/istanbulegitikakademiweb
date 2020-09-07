using Microsoft.EntityFrameworkCore;
using Nest;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Business.Repositories.BlogRepositories;
using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.Business.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
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
        private GroupMaterialRepository _groupMaterialRepository;
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

        private IElasticClient _elasticClient;
        public UnitOfWork(NbDataContext context, IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            _context = context;
        }
        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public EducationHostClassroomRepository ClassRoom => _educationHostClassroomRepository ??= new EducationHostClassroomRepository(_context);
        public GroupExpenseRepository GroupExpense => _groupExpenseRepository ??= new GroupExpenseRepository(_context);
        public GroupExpenseTypeRepository GroupExpenseType => _groupExpenseTypeRepository ??= new GroupExpenseTypeRepository(_context);
        public InvoiceDetailRepository InvoiceDetail => _invoiceDetailRepository ??= new InvoiceDetailRepository(_context);
        public InvoiceRepository Invoice => _invoiceRepository ??= new InvoiceRepository(_context);
        public SuggestionRepository Suggestions => _suggestionRepository ??= new SuggestionRepository(_context, _elasticClient);
        public EducationCategoryRepository EducationCategory => _educationCategoryRepository ??= new EducationCategoryRepository(_context);

        public EducationTagRepository EducationTag => _educationTagRepository ??= new EducationTagRepository(_context);

        public EducationRepository Education => _education ??= new EducationRepository(_context);

        public EducationMediaItemRepository EducationMedia => _educationMediaItem ??= new EducationMediaItemRepository(_context);

        public EducationPartRepository EducationPart => _educationPart ??= new EducationPartRepository(_context);

        public EducationGainRepository EducationGain => _educationGain ??= new EducationGainRepository(_context);

        public EducatorRepository Educator => _educator ??= new EducatorRepository(_context);

        public EducatorSocialMediaRepository EducatorSocialMedia => _educatorSocialMedia ??= new EducatorSocialMediaRepository(_context);

        public StudentEducationInfoRepository StudentEducationInfo => _studentEducationInfo ??= new StudentEducationInfoRepository(_context);

        public CustomerRepository Customer => _customerRepository ??= new CustomerRepository(_context);

        public BridgeEducationEducatorRepository Bridge_EducationEducator => _bridgeEducationEducatorRepository ??= new BridgeEducationEducatorRepository(_context);
        public EducationGroupRepository EducationGroup => _educationGroupRepository ??= new EducationGroupRepository(_context);

        public EducationHostRepository EducationHost => _educationHostRepository ??= new EducationHostRepository(_context);

        public WeekDaysOfGroupRepository WeekDaysOfGroup => _weekDaysOfGroupRepository ??= new WeekDaysOfGroupRepository(_context);

        public TicketRepository Ticket => _ticketRepository ??= new TicketRepository(_context);

        public SaleRepository Sale
        {
            get
            {
                return _saleRepository ?? (_saleRepository = new SaleRepository(_context));
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
                return _emailRepository ?? (_emailRepository = new EmailRepository(_context));
            }
        }
        public EducatorSalaryRepository Salary
        {
            get
            {
                return _educatorSalaryRepository ?? (_educatorSalaryRepository = new EducatorSalaryRepository(_context));
            }
        }
        public GroupMaterialRepository Material
        {
            get
            {
                return _groupMaterialRepository ?? (_groupMaterialRepository = new GroupMaterialRepository(_context));
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
        public BlogPostRepository BlogPost => _blogPostRepository ??= new BlogPostRepository(_context);
        public BlogCategoryRepository BlogCategory => _blogCategoryRepository ??= new BlogCategoryRepository(_context);
        public BlogTagRepository BlogTag => _blogTagRepository ??= new BlogTagRepository(_context);
     }
}
