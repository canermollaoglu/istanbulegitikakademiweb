using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupExpenseRepository : BaseRepository<GroupExpense,Guid>
    {
        private readonly NbDataContext _context;
        public GroupExpenseRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<GroupExpense> GetListByGroupIdWidthExpenseType(Guid groupId)
        {
            var data = _context.GroupExpenses.Include(x => x.ExpenseType).Where(x => x.GroupId == groupId).OrderByDescending(x => x.CreatedDate).ToList();
            return data;
        }
    }
}
