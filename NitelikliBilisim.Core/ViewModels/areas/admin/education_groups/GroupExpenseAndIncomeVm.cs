using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class GroupExpenseAndIncomeVm
    {
        public string GroupExpenses { get; set; } 
        public string EducatorExpenses { get; set; } 
        public string TotalStudentIncomes { get; set; } 
        public string GrandTotal { get; set; }
        public int TotalEducationHours { get; set; }
        public decimal EducatorExpensesAverage { get; set; }
        public decimal ProfitRate { get; set; }
        public string TotalExpenses { get; set; }
        public string KDV { get; set; }
        public string TotalPosCommissionAmount { get; set; }
    }
}
