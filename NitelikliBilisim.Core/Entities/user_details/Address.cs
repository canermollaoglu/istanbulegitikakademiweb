using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums.user_details;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("Addresses")]
    public class Address : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDefaultAddress { get; set; }
        public string NameSurname { get; set; }
        public string PhoneCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string IdentityNumber { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public AddressTypes AddressType { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
