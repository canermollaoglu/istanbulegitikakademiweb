using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CoursePageCommentsVm
    {
        public List<PagedCommentVm> Comments { get; set; }
        public bool IsLoadCommentButtonActive { get; set; }
    }
}
