using NitelikliBilisim.Core.Enums.promotion;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class EducationPromotionConditionAddVm
    {
        public Guid PromotionId { get; set; }
        public ConditionType ConditionType { get; set; }
        public string SingleValue { get; set; }
        public Guid[] MultipleValue { get; set; }
    }
}
