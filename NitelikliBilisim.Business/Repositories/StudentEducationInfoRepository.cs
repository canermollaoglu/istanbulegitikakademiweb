using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class StudentEducationInfoRepository : BaseRepository<StudentEducationInfo, Guid>
    {
        public StudentEducationInfoRepository(NbDataContext context) : base(context)
        {

        }
    }
}
