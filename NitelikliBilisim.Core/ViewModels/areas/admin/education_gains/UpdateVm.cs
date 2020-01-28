using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_gains
{
    public class UpdateGetVm : AddGainVm
    {
        public Guid Id { get; set; }
        public string EducationName { get; set; }
    }
    public class UpdatePostVm : AddGainVm
    {
        public Guid GainId { get; set; }
    }
}
