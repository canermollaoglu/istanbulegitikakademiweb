using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Models.Education
{
    public class AddGetVm
    {
        public List<EducationCategory> Categories { get; set; }
        public Dictionary<int, string> Levels { get; set; }
    }

    public class AddPostVm
    {

    }
}
