using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Testler")]
    public class Test : BaseEntity<Guid>
    {
        public Guid Code { get; set; } = Guid.NewGuid();
    }
}
