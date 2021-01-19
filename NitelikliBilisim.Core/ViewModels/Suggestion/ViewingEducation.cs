using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Suggestion
{
    public class ViewingEducation
    {
        public string SeoUrl{ get; set; }
        /// <summary>
        /// Sunum amaçlı
        /// </summary>
        public Education Education { get; set; }
        public int ViewingCount { get; set; }
        public double Point { get; set; }
    }
}
