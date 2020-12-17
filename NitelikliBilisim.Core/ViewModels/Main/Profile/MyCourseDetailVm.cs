using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyCourseDetailVm
    {
        public Guid EducationId { get; set; }
        public string EducatorId { get; set; }
        public string EducationName { get; set; }
        public decimal? PriceText { get; set; }
        public string Days { get; set; }
        public string Hours { get; set; }
        public string Host { get; set; }
        public string EducatorName { get; set; }
        public string EducatorTitle { get; set; }
        public string EducatorAvatarPath { get; set; }
        public string EducationFeaturedImage { get; set; }
        public string EducationShortDescription { get; set; }
        public List<EducatorCertificate> EducatorCertificates { get; set; }
        public string SeoUrl { get; set; }
    }
}
