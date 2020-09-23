using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_groups
{
    public class UpdateGroupGeneralInformationVm
    {
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        [MinLength(3,ErrorMessage ="Grup adı minimum 3 karakterden oluşmalıdır.")]
        public string GroupName { get; set; }
        [Required]
        public decimal NewPrice { get; set; }

    }
}
