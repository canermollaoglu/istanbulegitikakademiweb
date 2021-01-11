using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.EducationComment
{
    public class EducationCommentListVm
    {
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public byte Point { get; set; }
        public string UserName { get; set; }
        public string AvatarPath { get; set; }
        public string Job { get; set; }
        public string Date { get; set; }
        public Guid CategoryId { get; set; }
    }
}
