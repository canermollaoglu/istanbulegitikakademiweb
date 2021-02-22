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

        public AdminDashboardWidgetVm GetStudentInfo()
        {
            // Son iki aylık veri çekiliyor.
            var data = _context.Customers.Where(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-2)).ToList();
            var firstMonth = data.Count(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1));
            var secondMonth = data.Count(x => x.CreatedDate < DateTime.Now.Date.AddMonths(-1));
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString();
            var rate = (((firstMonth - secondMonth) * 100) / secondMonth);
            retVal.IsPositive = rate > 0;
            retVal.Rate = Math.Abs(rate).ToString();

            return retVal;
        }

        public AdminDashboardWidgetVm GetSalesInfo()
        {
            var culture = CultureInfo.CreateSpecificCulture("tr-TR");
            var data = _context.OnlinePaymentDetailsInfos.Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-2) && !x.IsCancelled).ToList();
            var firstMonth = data.Where(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1)).Sum(x => x.PaidPrice);
            var secondMonth = data.Where(x => x.CreatedDate < DateTime.Now.Date.AddMonths(-1)).Sum(x => x.PaidPrice);
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString(culture);
            var rate = (((firstMonth - secondMonth) * 100) / secondMonth);
            retVal.IsPositive = rate > 0;
            retVal.Rate = Math.Abs((int)rate).ToString();
            return retVal;
        }

        public AdminDashboardWidgetVm GetEducationGroupInfo()
        {
            var data = _context.EducationGroups.Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-2)).ToList();
            var firstMonth = data.Count(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1));
            var secondMonth = data.Count(x => x.CreatedDate < DateTime.Now.Date.AddMonths(-1));
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonth.ToString();
            var rate = (((firstMonth - secondMonth) * 100) / secondMonth);
            retVal.IsPositive = rate > 0;
            retVal.Rate = Math.Abs(rate).ToString();

            return retVal;
        }

        public AdminDashboardWidgetVm GetProfitInfo()
        {
            var groups = _context.EducationGroups.Include(x => x.GroupExpenses).Include(x=>x.GroupLessonDays).Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-2)).ToList();
            
            var firstMonthGroups = groups.Where(x => x.CreatedDate >= DateTime.Now.Date.AddMonths(-1));
            var firstMonthGroupIds = firstMonthGroups.Select(x => x.Id).ToList();
            var firstMonthIncomes = _context.InvoiceDetails.Include(x => x.OnlinePaymentDetailInfo)
                .Where(x => firstMonthGroupIds.Contains(x.GroupId) && !x.OnlinePaymentDetailInfo.IsCancelled)
                .Select(x=>x.OnlinePaymentDetailInfo)
                .Sum(x => x.MerchantPayout);
            var firstMonthExpense = _context.GroupExpenses.Where(x => firstMonthGroupIds.Contains(x.GroupId)).Sum(x=>x.Price);
            var firstMonthEducatorExpense = firstMonthGroups.Sum(x => x.GroupLessonDays.Sum(y => y.EducatorSalary));
            var firstMonthProfit = firstMonthIncomes - (firstMonthExpense + firstMonthEducatorExpense);

            var secondMonthGroups = groups.Where(x => x.CreatedDate < DateTime.Now.Date.AddMonths(-1));
            var secondMonthGroupIds = secondMonthGroups.Select(x => x.Id).ToList();
            var secondMonthIncomes = _context.InvoiceDetails.Include(x => x.OnlinePaymentDetailInfo)
               .Where(x => secondMonthGroupIds.Contains(x.GroupId) && !x.OnlinePaymentDetailInfo.IsCancelled)
               .Sum(x => x.OnlinePaymentDetailInfo.MerchantPayout);
            var secondMonthExpense = _context.GroupExpenses.Where(x => secondMonthGroupIds.Contains(x.GroupId)).Sum(x => x.Price);
            var secondMonthEducatorExpense = secondMonthGroups.Sum(x => x.GroupLessonDays.Sum(y => y.EducatorSalary));
            var secondMonthProfit = secondMonthIncomes - (secondMonthExpense + secondMonthEducatorExpense);

            var culture = CultureInfo.CreateSpecificCulture("tr-TR");
            var retVal = new AdminDashboardWidgetVm();
            retVal.Value = firstMonthProfit.GetValueOrDefault().ToString(culture);
            var rate = (((firstMonthProfit.GetValueOrDefault() - secondMonthProfit.GetValueOrDefault()) * 100) / secondMonthProfit.GetValueOrDefault());
            retVal.IsPositive = rate > 0;
            retVal.Rate = Math.Abs((int)rate).ToString();

            return retVal;
        }

        public AdminDashboardSalesChartDataVm GetSalesChartData()
        {
            //var salesData = _context.OnlinePaymentDetailsInfos
            //    .Where(x => x.CreatedDate.Year == DateTime.Now.Date.Year)
            //    .GroupBy(x => x.CreatedDate.Month)
            //    .Select(x => new
            //    {
            //        Key = x.Key,
            //        Value = x.Sum(x => x.MerchantPayout)
            //    }).ToDictionary(x=>x.Key,x=>x.Value);
            var currentCulture = CultureInfo.CreateSpecificCulture("tr-TR");
            var salesData = _context.OnlinePaymentDetailsInfos
                .Where(x => x.CreatedDate.Year == DateTime.Now.Date.Year)
                .GroupBy(x => new { x.CreatedDate.Date.Month, x.CreatedDate.Year })
                .Select(x => new ApexChartModel
                {
                    x = x.Key.Month.ToString()+"."+x.Key.Year.ToString(),
                    y = x.Sum(x => x.PaidPrice).ToString(currentCulture)
                }).ToArray();

            var retVal = new AdminDashboardSalesChartDataVm();
            retVal.Values =salesData;
            return retVal;
        }
    }
}

