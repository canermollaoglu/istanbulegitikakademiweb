using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class TicketRepository : BaseRepository<Ticket, Guid>
    {
        private readonly NbDataContext _context;
        public TicketRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
