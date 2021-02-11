using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ESOptions.ESEntities
{
    public class BlogViewLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Keyword]
        public string IpAddress { get; set; }
        [Keyword]
        public string UserId { get; set; }
        [Keyword]
        public string SessionId { get; set; }
        [Keyword]
        public object CatSeoUrl { get; set; }
        [Keyword]
        public object SeoUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
