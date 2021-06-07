using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.popular_topic
{
    public class PopularTopicListVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string TargetUrl { get; set; }
        public string RelatedCategory { get; set; }
    }
}
