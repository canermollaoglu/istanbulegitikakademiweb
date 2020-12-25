using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.EducationComment
{
    public class FeaturedCommentVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Content { get; set; }
        public string PreviewVideo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string PreviewImage { get; set; }
    }
}
