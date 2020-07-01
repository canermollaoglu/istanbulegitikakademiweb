using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    public class EducationHostUpdatePostVm : EducationHostAddPostVm
    {
        [Required]
        public Guid Id { get; set; }

    }
}
