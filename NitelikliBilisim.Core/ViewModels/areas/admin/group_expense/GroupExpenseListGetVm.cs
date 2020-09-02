using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_expense
{
    public class GroupExpenseListGetVm
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public byte Count { get; set; }
        public decimal TotalPrice => Count * Price;
        public string ExpenseTypeName { get; set; }

    }
}
