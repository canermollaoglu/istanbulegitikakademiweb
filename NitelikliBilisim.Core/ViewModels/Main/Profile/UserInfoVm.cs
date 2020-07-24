
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class UserInfoVm
    {
        public _PersonalAccountInfo PersonalAndAccountInfo { get; set; }
        public _EducationInfo EducationInfo { get; set; }
        public List<_Ticket> Tickets { get; set; }
        public List<EducationVm> WishList { get; set; }
    }

    public class _PersonalAccountInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FilePath { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public University LastGraduatedSchool { get; set; }
        public string Job { get; set; }
        public City City { get; set; }
        public string WebSiteUrl { get; set; }
        public string LinkedInProfileUrl { get; set; }

    }
    public class _EducationInfo
    {
        public string EducationCenter { get; set; }
        public DateTime StartedAt { get; set; }
        public string EducationCategory { get; set; }
    }
    public class _Ticket
    {
        public Guid TicketId { get; set; }
        public bool IsUsed { get; set; }
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
        public Guid HostId { get; set; }
        public string HostName { get; set; }
        public string HostCity { get; set; }
    }
}
