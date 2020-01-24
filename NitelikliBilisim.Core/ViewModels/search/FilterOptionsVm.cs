using NitelikliBilisim.Core.Enums;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.search
{
    public class FilterOptionsVm
    {
        public Dictionary<string, int> categories { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> locations { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> levels { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> stars { get; set; } = new Dictionary<string, int>();
    }

    public class FiltersVm
    {
        public string[] categories { get; set; }
        public string[] locations { get; set; }
        public EducationLevel[] levels { get; set; }
        public int[] ratings { get; set; }
    }
}
