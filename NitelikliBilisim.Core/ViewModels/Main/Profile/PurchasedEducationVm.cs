using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class PurchasedEducationVm
    {
        public Guid EducationId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string City { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string EducatorImageUrl { get; set; }
        public int CompletionRate { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
