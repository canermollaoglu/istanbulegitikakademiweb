using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.groups
{
    [Table("GroupExpenses")]
    public class GroupExpense : BaseEntity<Guid>
    {
        public GroupExpense() 
        {
            Id = Guid.NewGuid();
        }

        [MaxLength(256)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public byte Count { get; set; }

        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        public virtual EducationGroup Group { get; set; }

        [ForeignKey("GroupExpenseType")]
        public Guid ExpenseTypeId { get; set; }
        public virtual GroupExpenseType ExpenseType { get; set; }
    }
}
