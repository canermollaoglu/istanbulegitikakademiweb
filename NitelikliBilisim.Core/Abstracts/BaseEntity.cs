using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Abstracts
{
    public abstract class BaseEntity<TKey> : AuditBase, IEntity<TKey>
    {
        [Key]
        [Column(Order = 1)]
        public TKey Id { get; set; }
    }
}
