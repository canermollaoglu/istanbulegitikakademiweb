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
        public UnitOfWork(NbDataContext context)
        {
            _context = context;
        }
        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public EducationCategoryRepository EducationCategory
        {
            get
            {
                return _educationCategoryRepository ?? (_educationCategoryRepository = new EducationCategoryRepository(_context));
            }
        }
        public EducationTagRepository EducationTag
        {
            get
            {
                return _educationTagRepository ?? (_educationTagRepository = new EducationTagRepository(_context));
            }
        }
        public EducationRepository Education
        {
            get
            {
                return _education ?? (_education = new EducationRepository(_context));
            }
        }
        public EducationMediaItemRepository EducationMedia
        {
            get
            {
                return _educationMediaItem ?? (_educationMediaItem = new EducationMediaItemRepository(_context));
            }
        }
        public EducationPartRepository EducationPart
        {
            get
            {
                return _educationPart ?? (_educationPart = new EducationPartRepository(_context));
            }
        }
        public EducationGainRepository EducationGain
        {
            get
            {
                return _educationGain ?? (_educationGain = new EducationGainRepository(_context));
            }
        }
        public EducatorRepository Educator
        {
            get
            {
                return _educator ?? (_educator = new EducatorRepository(_context));
            }
        }
        public EducatorSocialMediaRepository EducatorSocialMedia
        {
            get
            {
                return _educatorSocialMedia ?? (_educatorSocialMedia = new EducatorSocialMediaRepository(_context));
            }
        }
        public StudentEducationInfoRepository StudentEducationInfo
        {
            get
            {
                return _studentEducationInfo ?? (_studentEducationInfo = new StudentEducationInfoRepository(_context));
            }
        }
        public EducationSuggestionRepository Suggestion
        {
            get
            {
                return _suggestionRepository ?? (_suggestionRepository = new EducationSuggestionRepository(_context));
            }
        }
        public CustomerRepository Customer
        {
            get
            {
                return _customerRepository ?? (_customerRepository = new CustomerRepository(_context));
            }
        }
        public BridgeEducationEducatorRepository Bridge_EducationEducator
        {
            get
            {
                return _bridgeEducationEducatorRepository ?? (_bridgeEducationEducatorRepository = new BridgeEducationEducatorRepository(_context));
            }
        }
    }
}
