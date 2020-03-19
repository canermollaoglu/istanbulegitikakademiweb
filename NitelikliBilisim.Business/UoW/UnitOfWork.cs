using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.Repositories;
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
        private EducationSuggestionRepository _suggestionRepository;
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
        public UnitOfWork(NbDataContext context)
        {
            _context = context;
        }
        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public EducationCategoryRepository EducationCategory => _educationCategoryRepository ??= new EducationCategoryRepository(_context);

        public EducationTagRepository EducationTag => _educationTagRepository ??= new EducationTagRepository(_context);

        public EducationRepository Education => _education ??= new EducationRepository(_context);

        public EducationMediaItemRepository EducationMedia => _educationMediaItem ??= new EducationMediaItemRepository(_context);

        public EducationPartRepository EducationPart => _educationPart ??= new EducationPartRepository(_context);

        public EducationGainRepository EducationGain => _educationGain ??= new EducationGainRepository(_context);

        public EducatorRepository Educator => _educator ??= new EducatorRepository(_context);

        public EducatorSocialMediaRepository EducatorSocialMedia => _educatorSocialMedia ??= new EducatorSocialMediaRepository(_context);

        public StudentEducationInfoRepository StudentEducationInfo => _studentEducationInfo ??= new StudentEducationInfoRepository(_context);

        public EducationSuggestionRepository Suggestion => _suggestionRepository ??= new EducationSuggestionRepository(_context);

        public CustomerRepository Customer => _customerRepository ??= new CustomerRepository(_context);

        public BridgeEducationEducatorRepository Bridge_EducationEducator => _bridgeEducationEducatorRepository ??= new BridgeEducationEducatorRepository(_context);
        public EducationGroupRepository EducationGroup => _educationGroupRepository ??= new EducationGroupRepository(_context);

        public EducationHostRepository EductionHost => _educationHostRepository ??= new EducationHostRepository(_context);

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
    }
}
