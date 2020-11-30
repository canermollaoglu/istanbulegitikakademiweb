using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_host;
using NitelikliBilisim.Core.ViewModels.Main.AboutUs;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using MUsefulMethods;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostRepository : BaseRepository<EducationHost, Guid>
    {
        private readonly NbDataContext _context;
        public EducationHostRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<EducationHostVm> EducationHostList()
        {
            var data = _context.EducationHosts.Include(x => x.EducationHostImages).Select(x => new EducationHostVm
            {
                City = EnumHelpers.GetDescription(x.City),
                HostName = x.HostName,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                ImagePath = x.EducationHostImages.First().FileUrl
            }).ToList();
            return data;
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
