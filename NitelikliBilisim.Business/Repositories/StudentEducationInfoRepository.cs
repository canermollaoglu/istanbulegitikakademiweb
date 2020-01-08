using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class StudentEducationInfoRepository : BaseRepository<StudentEducationInfo, Guid>
    {
        public StudentEducationInfoRepository(NbDataContext context) : base(context)
        {

        }
    }
}
