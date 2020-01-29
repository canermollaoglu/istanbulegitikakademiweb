using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels
{
    public class GroupVm
    {
        public Guid GroupId { get; set; }
        public DateTime StartDate { get; set; }
        public byte Quota { get; set; }
        public HostVm Host { get; set; }
    }
}
