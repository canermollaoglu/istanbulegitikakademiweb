using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
