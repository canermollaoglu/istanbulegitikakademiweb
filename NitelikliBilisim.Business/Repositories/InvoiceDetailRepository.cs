using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class InvoiceDetailRepository : BaseRepository<InvoiceDetail,Guid>
    {
        private readonly NbDataContext _context;
        public InvoiceDetailRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public InvoiceDetail GetByIdWithOnlinePaymentDetailInfo(Guid invoiceDetailId)
        {
            return _context.InvoiceDetails.Include(x => x.OnlinePaymentDetailInfo).First(x => x.Id == invoiceDetailId);
        }
    }
}
