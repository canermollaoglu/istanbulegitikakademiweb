using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ViewModels.areas.admin.dashboard;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class DashboardRepository
    {
        private readonly NbDataContext _context;
        public DashboardRepository(NbDataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Son 1 ay içerisinde kayıt olan öğrenci sayısını ve bir önceki aya olan oranını döner.
        /// </summary>
        /// <returns></returns>
        public AdminDashboardWidgetVm GetStudentInfo()
        {
            var firstMonth = _context.Customers.Count(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1));
            var secondMonth = _context.Customers.Count(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-2) && x.CreatedDate < DateTime.Now.Date.AddMonths(-1));
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString();

            var rate =secondMonth>0 && firstMonth>0? (((firstMonth - secondMonth) * 100) / secondMonth):0;
            retVal.IsPositive = rate >= 0;
            retVal.Rate = Math.Abs(rate).ToString();

            return retVal;
        }
        /// <summary>
        /// Yapılan iade bilgilerini döner
        /// </summary>
        /// <returns></returns>
        public IQueryable<LastRefundVm> GetLastRefunds()
        {
            var data = from onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos
                       join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetailInfo.Id equals invoiceDetail.Id
                       join invoice in _context.Invoices on invoiceDetail.InvoiceId equals invoice.Id
                       join eGroup in _context.EducationGroups on invoiceDetail.GroupId equals eGroup.Id
                       join education in _context.Educations on eGroup.EducationId equals education.Id
                       join user in _context.Users on invoice.CustomerId equals user.Id
                       where onlinePaymentDetailInfo.IsCancelled
                       select new LastRefundVm
                       {
                           Id = onlinePaymentDetailInfo.Id,
                           StudentId=user.Id,
                           GroupId = eGroup.Id,
                           RefundDate = onlinePaymentDetailInfo.CancellationDate.Value,
                           StudentName = user.Name,
                           StudentSurname = user.Surname,
                           EducationName = education.Name,
                           GroupName = eGroup.GroupName,
                           RefundPrice = onlinePaymentDetailInfo.RefundPrice
                       };
            return data;
        }
        /// <summary>
        /// Satış bilgilerini döner (İadeler hariç)
        /// </summary>
        /// <returns></returns>
        public IQueryable<LastSalesVm> GetLastSales()
        {
            var data = from onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos
                       join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetailInfo.Id equals invoiceDetail.Id
                       join invoice in _context.Invoices on invoiceDetail.InvoiceId equals invoice.Id
                       join eGroup in _context.EducationGroups on invoiceDetail.GroupId equals eGroup.Id
                       join education in _context.Educations on eGroup.EducationId equals education.Id
                       join user in _context.Users on invoice.CustomerId equals user.Id
                       where !onlinePaymentDetailInfo.IsCancelled
                       select new LastSalesVm
                       {
                           Id = onlinePaymentDetailInfo.Id,
                           StudentId = user.Id,
                           GroupId  = eGroup.Id,
                           Date = onlinePaymentDetailInfo.CreatedDate,
                           StudentName = user.Name,
                           StudentSurname = user.Surname,
                           EducationName = education.Name,
                           GroupName = eGroup.GroupName,
                           PaidPrice = onlinePaymentDetailInfo.PaidPrice
                       };
            return data;
        }
        /// <summary>
        /// Son 1 ay içerisinde yapılan ödemeleri (İptal edilmemişler) ve bir önceki 30 güne oranını döner
        /// </summary>
        /// <returns></returns>
        public AdminDashboardWidgetVm GetSalesInfo()
        {
            var culture = CultureInfo.CreateSpecificCulture("tr-TR");
            var data = _context.OnlinePaymentDetailsInfos.Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-2) && !x.IsCancelled).ToList();
            var firstMonth = data.Where(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1)).Sum(x => x.PaidPrice);
            var secondMonth = data.Where(x => x.CreatedDate < DateTime.Now.Date.AddMonths(-1)).Sum(x => x.PaidPrice);
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString(culture);
            var rate = secondMonth > 0 && firstMonth > 0 ? (((firstMonth - secondMonth) * 100) / secondMonth):0;
            retVal.IsPositive = rate >= 0;
            retVal.Rate = Math.Abs((int)rate).ToString();
            return retVal;
        }
        /// <summary>
        /// Son 1 ay içerisinnde açılan grup sayısını ve önceki aya oranını döner (Grup başlangıç tarihi baz alınır.)
        /// </summary>
        /// <returns></returns>
        public AdminDashboardWidgetVm GetEducationGroupInfo()
        {
            var firstMonth = _context.EducationGroups.Count(x => x.StartDate >= DateTime.Now.Date.AddMonths(-1));
            var secondMonth = _context.EducationGroups.Count(x => x.StartDate < DateTime.Now.Date.AddMonths(-1) && x.StartDate >= DateTime.Now.Date.AddMonths(-2));
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString();
            var rate = secondMonth > 0 && firstMonth > 0 ? (((firstMonth - secondMonth) * 100) / secondMonth):0;
            retVal.IsPositive = rate >= 0;
            retVal.Rate = Math.Abs(rate).ToString();

            return retVal;
        }
        /// <summary>
        /// Son 1 ay içerisinde açılan gruplara yapılan ödemelerden (İptal edilmemiş) grup giderleri ve eğitmen ücretleri çılarılarak kar hesaplanır.
        /// </summary>
        /// <returns></returns>
        public AdminDashboardWidgetVm GetProfitInfo()
        {
            var groups = _context.EducationGroups.Include(x=>x.GroupExpenses).Include(x => x.GroupLessonDays).Where(x => x.StartDate.Date >= DateTime.Now.AddMonths(-2).Date && x.StartDate.Date<=DateTime.Now.Date).ToList();

            var firstMonthGroups = groups.Where(x => x.StartDate >= DateTime.Now.Date.AddMonths(-1));
            var firstMonthGroupIds = firstMonthGroups.Select(x => x.Id).ToList();
            var firstMonthIncomes = _context.InvoiceDetails.Include(x => x.OnlinePaymentDetailInfo)
                .Where(x => firstMonthGroupIds.Contains(x.GroupId) && !x.OnlinePaymentDetailInfo.IsCancelled)
                .Sum(x => x.OnlinePaymentDetailInfo.MerchantPayout);
            var firstMonthExpense = firstMonthGroups.Sum(x => x.GroupExpenses.Sum(y => y.Price)); //_context.GroupExpenses.Where(x => firstMonthGroupIds.Contains(x.GroupId)).Sum(x => x.Price * x.Count);
            var firstMonthEducatorExpense = firstMonthGroups.Sum(x => x.GroupLessonDays.Sum(y => y.EducatorSalary));
            var firstMonthProfit = firstMonthIncomes - (firstMonthExpense + firstMonthEducatorExpense);

            var secondMonthGroups = groups.Where(x => x.StartDate < DateTime.Now.Date.AddMonths(-1));
            var secondMonthGroupIds = secondMonthGroups.Select(x => x.Id).ToList();
            var secondMonthIncomes = _context.InvoiceDetails.Include(x => x.OnlinePaymentDetailInfo)
               .Where(x => secondMonthGroupIds.Contains(x.GroupId) && !x.OnlinePaymentDetailInfo.IsCancelled)
               .Sum(x => x.OnlinePaymentDetailInfo.MerchantPayout);
            var secondMonthExpense = secondMonthGroups.Sum(x => x.GroupExpenses.Sum(y => y.Price));//_context.GroupExpenses.Where(x => secondMonthGroupIds.Contains(x.GroupId)).Sum(x => x.Price * x.Count);
            var secondMonthEducatorExpense = secondMonthGroups.Sum(x => x.GroupLessonDays.Sum(y => y.EducatorSalary));
            var secondMonthProfit = secondMonthIncomes - (secondMonthExpense + secondMonthEducatorExpense);

            var culture = CultureInfo.CreateSpecificCulture("tr-TR");
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = decimal.Round(firstMonthProfit.GetValueOrDefault(), 2).ToString(culture);
            var rate = firstMonthProfit.GetValueOrDefault() > 0 && secondMonthProfit.GetValueOrDefault() > 0 ? (((firstMonthProfit.GetValueOrDefault() - secondMonthProfit.GetValueOrDefault()) * 100) / secondMonthProfit.GetValueOrDefault()):0;
            retVal.IsPositive = rate >= 0;
            retVal.Rate = Math.Abs((int)rate).ToString();

            return retVal;
        }

        public AdminDashboardChartDataVm GetSalesChartData()
        {
            var currentCulture = CultureInfo.CreateSpecificCulture("tr-TR");
            var salesData = _context.OnlinePaymentDetailsInfos
                .Where(x => x.CreatedDate.Year == DateTime.Now.Date.Year && !x.IsCancelled);
            var sales = new List<ApexChartModel>();
            var month = DateTime.Now.Month;
            for (int i = 1; i <= month; i++)
            {
                var currentMonthSales = salesData.Where(x => x.CreatedDate.Month == i).Sum(x => x.PaidPrice);
                sales.Add(new ApexChartModel
                {
                    x = new DateTime(2020, i, 1).ToString("MMMM", currentCulture),
                    y = currentMonthSales.ToString(currentCulture)
                });
            }

            var retVal = new AdminDashboardChartDataVm();
            retVal.Values = sales.ToArray();
            return retVal;
        }

        public AdminDashboardChartDataVm GetGroupExpenseChartData()
        {
            var currentCulture = CultureInfo.CreateSpecificCulture("tr-TR");
            var expenseData = _context.GroupExpenses
                .Where(x => x.CreatedDate.Year == DateTime.Now.Date.Year);
            var expenses = new List<ApexChartModel>();
            var month = DateTime.Now.Month;
            for (int i = 1; i <= month; i++)
            {
                var currentMonthSales = expenseData.Where(x => x.CreatedDate.Month == i).Sum(x => x.Price * x.Count);
                expenses.Add(new ApexChartModel
                {
                    x = new DateTime(2020, i, 1).ToString("MMMM", currentCulture),
                    y = currentMonthSales.ToString(currentCulture)
                });
            }
            var retVal = new AdminDashboardChartDataVm();
            retVal.Values = expenses.ToArray();
            return retVal;
        }

        public AdminDashboardChartDataVm GetEducatorExpenseChartData()
        {
            var currentCulture = CultureInfo.CreateSpecificCulture("tr-TR");
            var expenseData = _context.GroupLessonDays
                .Where(x => x.DateOfLesson.Year == DateTime.Now.Date.Year);
            var expenses = new List<ApexChartModel>();
            var month = DateTime.Now.Month;
            for (int i = 1; i <= month; i++)
            {
                var currentMonthSales = expenseData.Where(x => x.DateOfLesson.Month == i).Sum(x => x.EducatorSalary.GetValueOrDefault());
                expenses.Add(new ApexChartModel
                {
                    x = new DateTime(2020, i, 1).ToString("MMMM", currentCulture),
                    y = currentMonthSales.ToString(currentCulture)
                });
            }
            var retVal = new AdminDashboardChartDataVm();
            retVal.Values = expenses.ToArray();
            return retVal;
        }


    }
}

