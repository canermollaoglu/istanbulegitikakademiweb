using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentListVm
    {
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsNbuyStudent { get; set; }
        public string NbuyCategory { get; set; }
        public string Id { get; set; }
        public Jobs Job { get; set; }
    }
}
