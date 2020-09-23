using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using NitelikliBilisim.Core.ViewModels.areas.admin.student;
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

        public IQueryable<StudentBasedSalesReport> GetStudentBasedSalesReport(string studentId)
        {
            var data = (from paymentDetailInfo in _context.OnlinePaymentDetailsInfos
                        join invoicedetails in _context.InvoiceDetails on paymentDetailInfo.Id equals invoicedetails.Id
                        join education in _context.Educations on invoicedetails.EducationId equals education.Id
                        where paymentDetailInfo.CreatedUser == studentId
                        select new StudentBasedSalesReport
                        {
                            IsCancelled = paymentDetailInfo.IsCancelled,
                            EducationName = education.Name,
                            CancellationDate = paymentDetailInfo.CancellationDate,
                            PaymentDate = paymentDetailInfo.CreatedDate,
                            PaidPrice = paymentDetailInfo.PaidPrice,
                            CommissionFee = paymentDetailInfo.CommissionFee,
                            CommissionRate = paymentDetailInfo.CommisionRate,
                            MerchantPayout = paymentDetailInfo.MerchantPayout
                        });
            return data;
        }

        public IQueryable<StudentTicketsVm> GetStudentTickets(string studentId)
        {
            var data = (from ticket in _context.Tickets
                        join education in _context.Educations on ticket.EducationId equals education.Id
                        join host in _context.EducationHosts on ticket.HostId equals host.Id
                        where ticket.OwnerId == studentId
                        select new StudentTicketsVm
                        {
                            TicketId = ticket.Id,
                            CreatedDate = ticket.CreatedDate,
                            EducationName = education.Name,
                            HostName = host.HostName,
                            IsUsed = ticket.IsUsed
                        });

            return data;
        }

        public IQueryable<GroupBasedSalesReportStudentsVm> GetGroupBasedSalesReportStudents(Guid groupId)
        {
            return (from gs in _context.Bridge_GroupStudents
                    join student in _context.Users on gs.Id2 equals student.Id
                    join ticket in _context.Tickets on gs.TicketId equals ticket.Id
                    join educationGroup in _context.EducationGroups on gs.Id equals educationGroup.Id
                    join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                    join invoiceDetail in _context.InvoiceDetails on ticket.InvoiceDetailsId equals invoiceDetail.Id
                    where !paymentDetailInfo.IsCancelled
                    orderby gs.CreatedDate
                    where gs.Id == groupId
                    select new GroupBasedSalesReportStudentsVm
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Surname = student.Surname,
                        PaymentDate = paymentDetailInfo.CreatedDate,
                        ListPrice = invoiceDetail.PriceAtCurrentDate,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        Commission = paymentDetailInfo.CommissionFee+paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout
                    });
        }
        public IQueryable<GroupBasedSalesReportStudentsVm> GetGroupBasedSalesReportCancellationPaymentStudents(Guid groupId)
        {
            return (from gs in _context.Bridge_GroupStudents
                    join student in _context.Users on gs.Id2 equals student.Id
                    join ticket in _context.Tickets on gs.TicketId equals ticket.Id
                    join educationGroup in _context.EducationGroups on gs.Id equals educationGroup.Id
                    join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                    where paymentDetailInfo.IsCancelled
                    orderby gs.CreatedDate
                    where gs.Id == groupId
                    select new GroupBasedSalesReportStudentsVm
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Surname = student.Surname,
                        PaymentDate = paymentDetailInfo.CreatedDate,
                        ListPrice = educationGroup.NewPrice,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        Commission = paymentDetailInfo.CommissionFee + paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout
                    });
        }


        public IQueryable<GeneralSalesReportVm> GetGeneralSalesReport()
        {
             return (from paymentDetailInfo in _context.OnlinePaymentDetailsInfos
                    join invoiceDetail in _context.InvoiceDetails on paymentDetailInfo.Id equals invoiceDetail.Id
                    join invoice in _context.Invoices on invoiceDetail.InvoiceId equals invoice.Id
                    join education in _context.Educations on invoiceDetail.EducationId equals education.Id
                    join educationGroup in _context.EducationGroups on invoiceDetail.GroupId equals educationGroup.Id 
                    join educator in _context.Users on educationGroup.EducatorId equals educator.Id
                    join student in _context.Users on invoice.CustomerId equals student.Id
                    orderby paymentDetailInfo.CreatedDate descending
                    select new GeneralSalesReportVm
                    { 
                        SalesDate = paymentDetailInfo.CreatedDate,
                        BlockageResolveDate = paymentDetailInfo.BlockageResolveDate,
                        EducationName = education.Name,
                        GroupName = educationGroup.GroupName,
                        EducatorName = educator.Name,
                        EducatorSurname = educator.Surname,
                        Name = student.Name,
                        Surname = student.Surname,
                        Phone = student.PhoneNumber,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        Commission = paymentDetailInfo.CommissionFee + paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout,
                        Status =paymentDetailInfo.IsCancelled?"İade":paymentDetailInfo.BlockageResolveDate.Date<DateTime.Now.Date?"Aktarıldı":"Bekliyor",
                    });


        }

        public object GetGroupBasedSalesReportEducatorPriceTable(Guid groupId)
        {
            var educators = _context.Educators.Include(x=>x.User).ToList();
            var data = _context.GroupLessonDays.Where(x => x.GroupId == groupId).GroupBy(x => x.EducatorId)
                .Select(x => new EducatorPriceTableVm
                {
                    EducatorName = educators.First(e => e.Id == x.Key).User.Name,
                    AvgPrice = x.Average(p => p.EducatorSalary.GetValueOrDefault()),
                    

                });

            throw new NotImplementedException();
        }
    }
}
