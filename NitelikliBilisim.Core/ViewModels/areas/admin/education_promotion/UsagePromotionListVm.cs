using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class UsagePromotionListVm
    {
        public Guid Id { get; set; }
        public DateTime DateOfUse { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid InvoiceId { get; set; }
    }
}
