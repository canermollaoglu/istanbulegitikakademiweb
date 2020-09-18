using NitelikliBilisim.Core.Entities.user_details;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class StudentDetailVm
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LinkedInProfile { get; set; }
        public string Website { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsNBUYStudent { get; set; }
        public string AvatarPath { get; set; }
        public List<Address> Addresses { get; set; }
        public string Job { get; set; }
        public DateTime RegistrationDate { get; set; }
        public StudentNBUYEducationInfoVm StudentNBUYEducationInfo { get; set; }
    }
}
