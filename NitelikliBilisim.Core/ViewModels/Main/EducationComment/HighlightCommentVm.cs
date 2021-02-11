using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.EducationComment
{
    public class HighlightCommentVm
    {
        public Guid Id { get; set; }
        public byte Point { get; set; }
        public string CommenterName { get; set; }
        public string CommenterJob { get; set; }
        public string CommenterAvatarPath { get; set; }
        public string Content { get; set; }
    }
}
