using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorRepository : BaseRepository<Educator, string>
    {
        public EducatorRepository(NbDataContext context) : base(context)
        {
        }

        public List<_Educator> GetEducators()
        {
            var model = _context.Users
                .Join(_context.Educators, l => l.Id, r => r.Id, (x, y) => new _Educator
                {
                    Id = x.Id,
                    FullName = x.Name + " " + x.Surname,
                    Title = y.Title,
                    Phone = x.PhoneNumber,
                    Email = x.Email,
                    SocialMediaCount = _context.EducatorSocialMedias.Count(z => z.EducatorId == x.Id)
                }).ToList();
            return model;
        }
    }
}
