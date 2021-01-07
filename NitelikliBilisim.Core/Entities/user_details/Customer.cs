using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Customers")]
    public class Customer : BaseEntity<string>
    {
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
        public CustomerType CustomerType { get; set; }
        public bool IsNbuyStudent { get; set; }
        public Genders Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Son Mezun Olunan Okul
        /// </summary>
        public int? LastGraduatedSchoolId { get; set; }
        public Jobs Job { get; set; }
        public int? CityId { get; set; }
        public string WebSiteUrl { get; set; }
        public string LinkedInProfileUrl { get; set; }

        public virtual List<Address> Addresses { get; set; }
        public virtual List<StudentEducationInfo> StudentEducationInfos { get; set; }
    }
}
