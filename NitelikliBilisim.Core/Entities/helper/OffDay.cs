using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.helper
{
    [Table("OffDays")]
    public class OffDay : BaseEntity<int>
    {
        [Required]
        public string Name { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        [NotMapped]
        public DateTime Date => new DateTime(this.Year, this.Month, this.Day, 0, 0, 0);
    }
}
