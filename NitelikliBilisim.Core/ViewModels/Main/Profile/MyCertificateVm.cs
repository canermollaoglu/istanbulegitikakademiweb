using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyCertificateVm
    {
        public Guid Id { get; set; }
        public string EducationName { get; set; }
        public DateTime EducationDate { get; set; }
        public string EducationDateText { get; set; }
        public string EducationSeoUrl { get; set; }
        public string CategorySeoUrl { get; set; }
        public string CategoryName { get; set; }
        public Guid GroupId { get; set; }
        public string EducationImageUrl { get; set; }
        public string EducationCardImageUrl { get; set; }
    }
}
