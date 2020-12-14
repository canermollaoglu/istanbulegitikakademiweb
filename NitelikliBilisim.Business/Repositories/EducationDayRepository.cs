using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationDayRepository: BaseRepository<EducationDay,int>
    {
        private readonly NbDataContext _context;
        public EducationDayRepository(NbDataContext context):base(context)
        {
            _context = context;
        }
    }
}
