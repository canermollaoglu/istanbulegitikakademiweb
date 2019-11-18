using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Customers")]
    public class Customer : BaseEntity<string>
    {
        public CustomerType CustomerType { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(32)]
        public string Surname { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
    }
}
