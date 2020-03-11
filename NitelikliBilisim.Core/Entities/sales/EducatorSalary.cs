using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducatorSalaries")]
    public class EducatorSalary : BaseEntity<Guid>
    {
        public EducatorSalary()
        {
            Id = Guid.NewGuid();
        }
        public decimal Paid { get; set; }
        [MaxLength(450)]
        public string EducatorId { get; set; }
        public DateTime EarnedAt { get; set; }
        public Guid? EarnedForGroup { get; set; }
    }
}
