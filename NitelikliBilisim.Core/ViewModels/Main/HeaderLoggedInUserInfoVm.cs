using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main
{
    public class HeaderLoggedInUserInfoVm
    {
        public string AvatarPath { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsNbuyStudent { get; set; }
    }
}
