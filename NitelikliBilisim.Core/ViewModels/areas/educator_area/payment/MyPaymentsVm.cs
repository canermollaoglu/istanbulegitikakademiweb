using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.educator_area.payment
{
    public class MyPaymentsVm
    {
        public List<_PaidByGroup> Payments { get; set; }
    }
    public class _PaidByGroup
    {
        public string GroupName { get; set; }
        public string Paid { get; set; }
    }
}
