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
        private GroupLessonDayRepository _groupLessonDayRepository;
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

        public GroupLessonDayRepository GroupLessonDay
        {
            get
            {
                return _groupLessonDayRepository ?? (_groupLessonDayRepository = new GroupLessonDayRepository(_context));
            }
        }
    }
}
