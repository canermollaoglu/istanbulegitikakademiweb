using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator_certificate;
using NitelikliBilisim.Data;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorCertificateRepository: BaseRepository<EducatorCertificate,int>
    {
        private readonly NbDataContext _context;
        public EducatorCertificateRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public IQueryable<EducatorCertificateListVM> GetListQueryable()
        {
            return _context.EducatorCertificates.OrderBy(x => x.Name).Select(x=> new EducatorCertificateListVM { 
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CertificateImageFullPath = x.CertificateImagePath
            });
        }


    }
}
