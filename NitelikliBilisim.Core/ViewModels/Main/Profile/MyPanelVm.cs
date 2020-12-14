using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyPanelVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsNBUY { get; set; }
        public int? University { get; set; }
        public string AvatarPath { get; set; }
        public List<FavoriteEducationVm> FavoriteEducations { get; set; }
        public int FavoriteEducationCount { get; set; }
        public List<PurchasedEducationVm> PurchasedEducations { get; set; }
        public int PurchasedEducationCount { get; set; }
        public int EducationWeek { get; set; }
        public int TotalEducationWeek { get; set; }
    }
}
