using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class ReportRepository
    {
        private NbDataContext _context;
        public ReportRepository(NbDataContext context)
        {
            _context = context;
        }

        public IncomeReportVm FetchIncomeReport(int year, int month)
        {
            var data = _context.OnlinePaymentDetailsInfos
                .Include(x => x.InvoiceDetail)
                .ThenInclude(x => x.Invoice)
                .ThenInclude(x => x.Customer)
                .ThenInclude(x => x.User)
                //.Where(x => x.BlockageResolveDate.Year == year && x.BlockageResolveDate.Month == month)
                .Where(x =>
                x.BlockageResolveDate.AddMonths(x.InvoiceDetail.Invoice.PaymentCount - 1).AddDays(7).Year == year
                && x.BlockageResolveDate.AddMonths(x.InvoiceDetail.Invoice.PaymentCount - 1).AddDays(7).Month == month)
                .ToList();

            var payouts = new List<_Payout>();
            foreach (var item in data)
            {
                var paymentCount = item.InvoiceDetail.Invoice.PaymentCount;
                var payout = !item.IsCancelled ? item.MerchantPayout / paymentCount : item.MerchantPayout / paymentCount * -1;
                payouts.Add(new _Payout
                {
                    PayoutNumeric =  payout,
                    PayoutText = payout.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    IsNegative = payout < 0,
                    PayoutDate = item.BlockageResolveDate.AddDays(7),
                    PayoutDateText = item.BlockageResolveDate.AddDays(7).ToShortDateString(),
                    Payer = $"{item.InvoiceDetail.Invoice.Customer.User.Name.ToUpper()} {item.InvoiceDetail.Invoice.Customer.User.Surname.ToUpper()}"
                });
            }

            var model = new IncomeReportVm
            {
                Payouts = payouts,
                SumOfNegative = payouts.Where(x => x.PayoutNumeric < 0.0m).Sum(x => x.PayoutNumeric),
                SumOfPositive = payouts.Where(x => x.PayoutNumeric >= 0.0m).Sum(x => x.PayoutNumeric)
            };

            return model;
        }
    }
}
