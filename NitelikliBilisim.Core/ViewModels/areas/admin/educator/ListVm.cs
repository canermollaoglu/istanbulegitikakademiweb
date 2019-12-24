using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class ListVm
    {
        public string EducationId { get; set; }
    }
    public class _Educator
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int SocialMediaCount { get; set; }
    }
}
