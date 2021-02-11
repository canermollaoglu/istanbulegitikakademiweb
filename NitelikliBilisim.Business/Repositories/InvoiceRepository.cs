using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.invoice;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice, Guid>
    {
        private readonly NbDataContext _context;
        public InvoiceRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public Invoice GetByIdWithOnlinePaymentInfos(Guid invoiceId)
        {
            return _context.Invoices.Include(x => x.OnlinePaymentInfo).First(x => x.Id == invoiceId);
        }

        public IQueryable<InvoiceListVm> GetListQueryable()
        {
            var data = from invoice in _context.Invoices
                       join customer in _context.Customers on invoice.CustomerId equals customer.Id
                       join onlinePaymentInfo in _context.OnlinePaymentInfos on invoice.Id equals onlinePaymentInfo.Id
                       join user in _context.Users on customer.Id equals user.Id
                       select new InvoiceListVm
                       {
                           Id = invoice.Id,
                           Date = invoice.CreatedDate,
                           CustomerName = user.Name,
                           CustomerSurname = user.Surname,
                           PaidPrice = onlinePaymentInfo.PaidPrice,
                           PaymentId = onlinePaymentInfo.PaymentId,
                           InvoicePdfUrl = invoice.InvoicePdfUrl
                       };
            return data;
        }

        public Invoice GetByIdWithCustomer(Guid invoiceId)
        {
            return _context.Invoices.Include(x=>x.OnlinePaymentInfo).Include(x => x.Customer).ThenInclude(x => x.User).First(x => x.Id == invoiceId);
        }
    }
}
