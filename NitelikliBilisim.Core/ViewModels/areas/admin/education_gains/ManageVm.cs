using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_gains
{
    public class ManageVm
    {
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
    }

    public class GetEducationGainsVm
    {
        public List<_EducationGain> EducationGains { get; set; }
    }

    public class AddGainVm
    {
        public Guid EducationId { get; set; }
        [Required(ErrorMessage = "Kazanım boş geçilemez"), MaxLength(255, ErrorMessage = "Kazanım 255 karakterden fazla olamaz")]
        public string Gain { get; set; }
    }

    public class _EducationGain
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public string Gain { get; set; }
    }
}
