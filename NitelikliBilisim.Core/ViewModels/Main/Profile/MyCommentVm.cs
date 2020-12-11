using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyCommentVm
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public byte Point { get; set; }
        public DateTime Date { get; set; }
        public string EducationName { get; set; }
        public string CategoryName { get; set; }
        public string EducationFeaturedImage { get; set; }
        public CommentApprovalStatus ApprovalStatus { get; set; }
        public Guid EducationId { get; set; }
        public bool IsFavoriteEducation { get; set; }
    }
}
