using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("EducationDays")]
    public class EducationDay : IEntity<int>
    {
        [Key]
        [Column(Order =1)]
        public int Id { get; set; }
        /// <summary>
        /// Eğitimin kaçıncı günü olduğu bilgisini tutar.
        /// </summary>
        [Column(Order=2)]
        public int Day { get; set; }
        /// <summary>
        /// Eğitim gününün tarihini tutar.
        /// </summary>
        [Column(Order=3)]
        public DateTime Date { get; set; }

        public Guid StudentEducationInfoId { get; set; }
        [ForeignKey("StudentEducationInfoId")]
        public virtual StudentEducationInfo StudentEducationInfo { get; set; }
    }
}
