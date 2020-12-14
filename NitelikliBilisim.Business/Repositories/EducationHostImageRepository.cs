using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_host;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostImageRepository :BaseRepository<EducationHostImage,Guid>
    {
        private readonly NbDataContext _context;
        public EducationHostImageRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public List<EducationHostImage> GetByEducationHostId(Guid educationHostId)
        {

            return _context.EducationHostImages.Where(x => x.EducationHostId == educationHostId).ToList();
        }

    }
}
