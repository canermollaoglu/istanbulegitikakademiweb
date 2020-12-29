using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class PagedCommentVm
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserAvatarPath { get; set; }
        public string CreatedDateText { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte Point { get; set; }
        public string UserJob { get; set; }
        public string Content { get; set; }
    }
}
