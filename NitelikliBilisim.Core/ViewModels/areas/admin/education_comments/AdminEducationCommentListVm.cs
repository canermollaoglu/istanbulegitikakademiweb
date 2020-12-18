using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_comments
{
    public class AdminEducationCommentListVm
    {
        public Guid Id { get; set; }
        public byte Point { get; set; }
        public string Content { get; set; }
        public string CommenterName { get; set; }
        public string CommenterSurname { get; set; }
        public string CommenterEmail { get; set; }
        public string EducationName { get; set; }
        public Guid EducationId { get; set; }
        public CommentApprovalStatus ApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string CommenterId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsHighlight { get; set; }
    }
}
