using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducationComments")]
    public class EducationComment : BaseEntity<Guid>
    {
        public EducationComment()
        {
            Id = Guid.NewGuid();
        }
        [MaxLength(512)]
        public string Content { get; set; }
        [Range(1, 5)]
        public byte Points { get; set; }
        public bool IsEducatorComment { get; set; }
        [MaxLength(128)]
        public string CommentatorId { get; set; }
        [MaxLength(128)]
        public string ApproverId { get; set; }
        [Column(TypeName = "datetime2(3)")]
        public DateTime? ApprovalDate { get; set; }

        public Guid? BaseCommentId { get; set; }
        [ForeignKey("BaseCommentId")]
        public virtual EducationComment BaseComment { get; set; }

        public Guid EducationId { get; set; }
        [ForeignKey("EducationId")]
        public virtual Education Education { get; set; }
    }
}
