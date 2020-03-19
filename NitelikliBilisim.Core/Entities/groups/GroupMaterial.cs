using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("GroupMaterials")]
    public class GroupMaterial : BaseEntity<Guid>
    {
        public GroupMaterial()
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(64)]
        public string MaterialName { get; set; }
        public decimal Price { get; set; }
        public byte Count { get; set; }

        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        public virtual EducationGroup Group { get; set; }
    }
}
