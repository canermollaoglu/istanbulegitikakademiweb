using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationHostClassroomRepository: BaseRepository<Classroom,Guid>
    {
        private readonly NbDataContext _context;
        public EducationHostClassroomRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<Classroom> GetClassRoomsByHostId(Guid hostId)
        {
            return _context.Classrooms.Where(x => x.HostId == hostId).ToList();
        }

        public IQueryable<Classroom> GetClassRoomsByHostIdQueryable(Guid hostId)
        {
            return _context.Classrooms.Where(x => x.HostId == hostId).OrderBy(x => x.CreatedDate);
        }
    }
}
