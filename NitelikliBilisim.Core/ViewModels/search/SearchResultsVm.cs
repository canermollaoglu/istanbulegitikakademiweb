using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels
{
    public class SearchResultsGetVm
    {
        public string SearchText { get; set; }
        public Dictionary<int, string> OrderCriterias { get; set; }
        public string ShowAs { get; set; }
    }
}
