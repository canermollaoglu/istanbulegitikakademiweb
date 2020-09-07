using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_host;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostRepository : BaseRepository<EducationHost, Guid>
    {
        private readonly NbDataContext _context;
        public EducationHostRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable<EducationHostListVm> GetListQueryable()
        {
            return _context.EducationHosts.Select(x=> new EducationHostListVm { 
            Id = x.Id,
            Address = x.Address,
            City = x.City,
            HostName = x.HostName
            });
        }

    }
}
