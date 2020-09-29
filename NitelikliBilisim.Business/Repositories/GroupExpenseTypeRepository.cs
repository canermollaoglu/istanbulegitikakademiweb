using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupExpenseTypeRepository : BaseRepository<GroupExpenseType,Guid>
    {
        private NbDataContext _context;
        public GroupExpenseTypeRepository(NbDataContext context):base(context)
        {
            _context = context;
        }
    }
}
