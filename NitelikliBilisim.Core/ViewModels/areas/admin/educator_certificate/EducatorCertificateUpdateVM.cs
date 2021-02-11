using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator_certificate
{
    public class EducatorCertificateUpdateVM : EducatorCertificateAddVM
    {
       
    }

    public class EducatorCertificateUpdateGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public  string CertificateImagePath { get; set; }
    }
}
