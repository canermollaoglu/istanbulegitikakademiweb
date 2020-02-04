using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels
{
    public class SearchResultsGetVm
    {
        public string CategoryName { get; set; }
        public string SearchText { get; set; }
        public Dictionary<int, string> OrderCriterias { get; set; }
        public string ShowAs { get; set; }
    }
}
