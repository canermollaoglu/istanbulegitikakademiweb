using System;

namespace NitelikliBilisim.Core.ViewModels
{
    public class HostVm
    {
        public Guid HostId { get; set; }
        public string HostName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LocationUrl { get; set; }
    }
}
