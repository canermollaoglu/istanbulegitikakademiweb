using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Data;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorRepository : BaseRepository<Educator, string>
    {
        private readonly NbDataContext _context;
        public EducatorRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<_Educator> GetEducators()
        {
            var model = Context.Users
                .Join(Context.Educators, l => l.Id, r => r.Id, (x, y) => new _Educator
                {
                    Id = x.Id,
                    FullName = x.Name + " " + x.Surname,
                    Title = y.Title,
                    Phone = x.PhoneNumber,
                    Email = x.Email,
                    SocialMediaCount = Context.EducatorSocialMedias.Count(z => z.EducatorId == x.Id)
                }).ToList();
            return model;
        }
        public override int Delete(string id, bool isSaveLater = false)
        {
            var educationSocialMedia = _context.EducatorSocialMedias.Where(x => x.EducatorId == id).ToList();
            _context.EducatorSocialMedias.RemoveRange(educationSocialMedia);
            _context.SaveChanges();
            return base.Delete(id, isSaveLater);
        }
    }
}
