using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class GroupEducationInfoVm
    {
        public byte EducationDay { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool IsCreated { get; set; }
    }
}
