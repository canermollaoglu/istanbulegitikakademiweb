using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.EducationComment
{
    public class EducationCommentsVm
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public List<EducationCommentListVm> Comments { get; set; }
        public int TotalPageCount { get; set; }
    }
}
