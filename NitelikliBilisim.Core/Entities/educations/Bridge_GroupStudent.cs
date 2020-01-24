using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Bridge_GroupStudents")]
    public class Bridge_GroupStudent : BaseEntity2<Guid, string>
    {
        [ForeignKey("Id")]
        public virtual EducationGroup Group { get; set; }
        [ForeignKey("Id2")]
        public virtual Customer Customer { get; set; }

        public Guid SaleId { get; set; }
    }
}
