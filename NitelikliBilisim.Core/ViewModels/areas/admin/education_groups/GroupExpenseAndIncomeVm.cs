using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class GroupExpenseAndIncomeVm
    {
        public decimal GroupExpenses { get; set; } = 0;
        public decimal EducatorExpenses { get; set; } = 0;

        public decimal TotalStudentIncomes { get; set; } = 0;


        public decimal GrandTotal => TotalStudentIncomes - (GroupExpenses + EducatorExpenses);

    }
}
