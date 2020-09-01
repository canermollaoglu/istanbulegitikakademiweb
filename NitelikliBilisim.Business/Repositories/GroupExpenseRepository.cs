using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupExpenseRepository : BaseRepository<GroupExpense,Guid>
    {
        private readonly NbDataContext _context;
        public GroupExpenseRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


    }
}
