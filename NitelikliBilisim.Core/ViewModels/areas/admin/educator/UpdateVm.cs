using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.educator
{
    public class UpdateGetVm : AddGetVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Biography { get; set; }
    }

    public class UpdatePostVm : AddPostVm
    {
        public Guid EducatorId { get; set; }
    }
}
