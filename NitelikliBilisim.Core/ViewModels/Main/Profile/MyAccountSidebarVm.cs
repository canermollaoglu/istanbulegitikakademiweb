using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyAccountSidebarVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsNBUY { get; set; }
        public string University { get; set; }
        public string AvatarPath { get; set; }
        public string PageName { get; set; }
        public string NbuyCategory { get; set; }
        public int CertificateCount { get; set; }
    }
}
