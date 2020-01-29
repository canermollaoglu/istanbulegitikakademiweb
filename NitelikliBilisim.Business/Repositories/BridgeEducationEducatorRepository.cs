using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class BridgeEducationEducatorRepository
    {
        private readonly NbDataContext _context;
        public BridgeEducationEducatorRepository(NbDataContext context)
        {
            _context = context;
        }

        public void Insert(ManageAssignEducatorsPostVm data)
        {
            var bridgeEducationEducators = new List<Bridge_EducationEducator>();
            foreach (var item in data.Educators)
            {
                if (!_context.Bridge_EducationEducators.Any(x => x.Id == data.EducationId && x.Id2 == item))
                    bridgeEducationEducators.Add(new Bridge_EducationEducator
                    {
                        Id = data.EducationId,
                        Id2 = item
                    });
            }
            _context.Bridge_EducationEducators.AddRange(bridgeEducationEducators);
            _context.SaveChanges();
        }

        public List<EducatorVm> GetAssignedEducators(Guid educationId)
        {
            var educatorIds = _context.Bridge_EducationEducators.Where(x => x.Id == educationId)
                .Select(x => x.Id2)
                .ToList();

            if (educatorIds.Count == 0)
                return new List<EducatorVm>();

            var model = _context.Educators.Where(x => educatorIds.Contains(x.Id))
                .Include(x => x.User)
                .Select(x => new EducatorVm
                {
                    EducatorId = x.Id,
                    ProfilePhoto = x.User.AvatarPath,
                    Name = x.User.Name,
                    Surname = x.User.Surname,
                    Title = x.Title
                }).ToList();

            var storage = new StorageService();
            foreach (var item in model)
            {
                var folder = Path.GetDirectoryName(item.ProfilePhoto);
                var fileName = Path.GetFileName(item.ProfilePhoto);
                item.ProfilePhoto = storage.DownloadFile(fileName, folder).Result;
            }

            return model;
        }

        public void Delete(Guid educationId, Guid educatorId)
        {
           var educationEducator = _context.Bridge_EducationEducators.First(x => x.Id == educationId && x.Id2 == educatorId.ToString());
            _context.Bridge_EducationEducators.Remove(educationEducator);
            _context.SaveChanges();
        }
    }
}
