using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.Core.ViewModels
{
    public class GroupVm
    {
        public Guid GroupId { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateText { get; set; }
        public int Joined { get; set; }
        public byte Quota { get; set; }
        public HostVm Host { get; set; }
    }

    public class EducationGroupListVm
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
        public  string EducationName { get; set; }
        public  string HostName { get; set; }
        public string HostCity { get; set; }
        public int GroupStudents { get; set; }

    }
}
