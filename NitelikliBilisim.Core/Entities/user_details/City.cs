using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("Cities")]
    public class City : IEntity<int>
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    [Table("States")]
    public class State: IEntity<int>
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
    }
}
