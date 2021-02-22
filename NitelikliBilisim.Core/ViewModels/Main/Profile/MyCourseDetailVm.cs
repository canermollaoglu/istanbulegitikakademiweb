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
        public string PriceText { get; set; }
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
        public string CategorySeoUrl { get; set; }
        public double EducatorPoint { get; set; }
        public int EducatorStudentCount { get; set; }
        public Guid GroupId { get; set; }
        public string CategoryName { get; set; }
        public DateTime EducationEndDate { get; set; }
        public DateTime EducationDate { get; set; }
        public bool IsCertificateAvailable { get; set; }
        public Guid InvoiceDetailId { get; set; }
        public bool IsRefundable { get; set; }
        public bool IsCancelled { get; set; }
    }
}
