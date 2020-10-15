using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion
{
    public class EducationPromotionGetListVm
    {
        public Guid Id { get; set; }
        public string ConditionType { get; set; }
        public List<string> ConditionValues { get; set; }
    }
}
