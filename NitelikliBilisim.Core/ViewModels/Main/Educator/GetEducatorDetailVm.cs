using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Educator
{
    public class GetEducatorDetailVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarPath { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Biography { get; set; }
        public List<EducatorCertificate> Certificates { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GooglePlusUrl { get; set; }
        public int EducationCount { get; set; }
        public int StudentCount { get; set; }
        public List<EducationCategory> Categories { get; set; }
    }
}
