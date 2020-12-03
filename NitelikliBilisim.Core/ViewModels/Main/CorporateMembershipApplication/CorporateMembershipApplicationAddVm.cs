using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.CorporateMembershipApplication
{
    public class CorporateMembershipApplicationAddVm
    {
        [Required, MaxLength(150)]
        public string CompanyName { get; set; }
        [Required, MaxLength(100)]
        public string CompanySector { get; set; }
        [Required, MaxLength(100)]
        public string NameSurname { get; set; }
        [Required, MaxLength(15)]
        public string PhoneCode { get; set; }
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        [Required, MaxLength(100)]
        public string Department { get; set; }
        public int NumberOfEmployees { get; set; }
        public string Address { get; set; }
        public string RequestNote { get; set; }
    }
}
