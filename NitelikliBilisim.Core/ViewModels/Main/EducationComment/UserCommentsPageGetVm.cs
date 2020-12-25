using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.EducationComment
{
    public class UserCommentsPageGetVm
    {
        public Dictionary<int, string> SortingTypes { get; set; }
        public Dictionary<Guid, string> EducationCategories { get; set; }
        public EducationCommentsVm PageDetails { get; set; }
        public List<FeaturedCommentVm> FeaturedComments { get; set; }
    }
}
