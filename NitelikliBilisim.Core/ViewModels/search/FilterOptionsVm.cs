using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels.search
{
    public class FilterOptionsVm
    {
        public List<CategoryOptionVm> categories { get; set; } = new List<CategoryOptionVm>();
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

    public class CategoryOptionVm
    {
        public string BaseCategoryName { get; set; }
        public string CategoryName { get; set; }
        public int Count { get; set; }
    }
}
