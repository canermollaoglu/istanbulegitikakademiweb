using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.Abstracts
{
    public abstract class AuditBase
    {
        [StringLength(128)]
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [StringLength(128)]
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
