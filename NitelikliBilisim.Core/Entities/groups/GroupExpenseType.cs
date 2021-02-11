using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.groups
{
    [Table("GroupExpenseTypes")]

    public class GroupExpenseType : BaseEntity<Guid>
    {
        public GroupExpenseType()
        {
            Id = Guid.NewGuid();
        }

        public string Name { get; set; }
        public string Description { get; set; }

    }
}
