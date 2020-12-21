using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class SubscriptionBlogRepository:BaseRepository<BlogSubscriber, Guid>
    {
        public SubscriptionBlogRepository(NbDataContext context) : base(context)
        {

        }
    }
    public class SubscriptionNewsletterRepository : BaseRepository<NewsletterSubscriber, Guid>
    {
        public SubscriptionNewsletterRepository(NbDataContext context) : base(context)
        {

        }
    }

}
