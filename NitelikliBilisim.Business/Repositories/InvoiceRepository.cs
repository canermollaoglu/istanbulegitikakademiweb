using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class InvoiceRepository:BaseRepository<Invoice,Guid>
    {
        private readonly NbDataContext _context;
        public InvoiceRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public Invoice GetByIdWithOnlinePaymentInfos(Guid invoiceId)
        {
            return _context.Invoices.Include(x => x.OnlinePaymentInfo).First(x => x.Id == invoiceId);
        }
    }
}
