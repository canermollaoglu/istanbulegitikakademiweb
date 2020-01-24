using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.DTO
{
    public class BaseDto<TKey>
    {
        public TKey Id { get; set; }
        [StringLength(128)]
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(128)]
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
