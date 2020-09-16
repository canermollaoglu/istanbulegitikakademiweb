using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
                .Where(x => x.BlockageResolveDate.Year == year - 1 || x.BlockageResolveDate.Year == year)
                .ToList();

//TODO: eğitim bilgisi de ekleyelim

            var payouts = new List<_Payout>();
            foreach (var item in data)
            {
                var paymentCount = item.InvoiceDetail.Invoice.PaymentCount;
                for (int i = 0; i < paymentCount; i++)
                {
                    var blockageResolveDate = item.BlockageResolveDate.AddMonths(i);
                    if (blockageResolveDate.Month == month)
                    {
                        var payout = !item.IsCancelled ? item.MerchantPayout / paymentCount : item.MerchantPayout / paymentCount * -1;
                        payouts.Add(new _Payout
                        {
                            PayoutNumeric = payout,
                            PayoutText = payout.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                            IsNegative = payout < 0,
                            TotalNumeric = item.MerchantPayout,
                            TotalText = item.MerchantPayout.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                            PayoutDate = item.BlockageResolveDate.AddMonths(i),
                            PayoutDateText = item.BlockageResolveDate.AddMonths(i).ToShortDateString(),
                            OrderOfPayment = paymentCount > 1 ? $"{i + 1}. taksit" : "Tek çekim",
                            Payer = $"{item.InvoiceDetail.Invoice.Customer.User.Name.ToUpper()} {item.InvoiceDetail.Invoice.Customer.User.Surname.ToUpper()}"
                        });
                    }
                }
            }

            var model = new IncomeReportVm
            {
                Payouts = payouts,
                SumOfNegative = payouts.Where(x => x.PayoutNumeric < 0.0m).Sum(x => x.PayoutNumeric),
                SumOfPositive = payouts.Where(x => x.PayoutNumeric >= 0.0m).Sum(x => x.PayoutNumeric)
            };

            return model;
        }

        public IQueryable<GroupBasedSalesReportStudentsVm> GetGroupBasedSalesReportStudents(Guid groupId)
        {
            return (from gs in _context.Bridge_GroupStudents
                    join student in _context.Users on gs.Id2 equals student.Id
                    join ticket in _context.Tickets on gs.TicketId equals ticket.Id
                    join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                    where !paymentDetailInfo.IsCancelled
                    orderby gs.CreatedDate
                    where gs.Id == groupId
                    select new GroupBasedSalesReportStudentsVm
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Surname = student.Surname,
                        RegistrationDate = gs.CreatedDate,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout
                    });
        }

        public IQueryable<GeneralSalesReportVm> GetGeneralSalesReport()
        {
            //Todo yalnızca aktarılmış olan ücretleri göstermek için datetime kontrolü BlockageResolveDate üzerinden yapılabilir.
            return (from paymentDetailInfo in _context.OnlinePaymentDetailsInfos
                    join ticket in _context.Tickets on paymentDetailInfo.Id equals ticket.InvoiceDetailsId
                    join education in _context.Educations on ticket.EducationId equals education.Id
                    join groupStudent in _context.Bridge_GroupStudents on ticket.Id equals groupStudent.TicketId
                    join student in _context.Users on groupStudent.Id2 equals student.Id
                    orderby paymentDetailInfo.CreatedDate descending
                    where !paymentDetailInfo.IsCancelled
                    select new GeneralSalesReportVm
                    {
                        BlockageResolveDate= paymentDetailInfo.BlockageResolveDate,
                        EducationName = education.Name,
                        Name = student.Name,
                        Surname = student.Surname,
                        Phone = student.PhoneNumber,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout
                    });


        }

    }
}
