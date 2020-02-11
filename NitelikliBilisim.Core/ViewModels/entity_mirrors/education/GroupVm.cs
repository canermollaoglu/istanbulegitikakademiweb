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
}
