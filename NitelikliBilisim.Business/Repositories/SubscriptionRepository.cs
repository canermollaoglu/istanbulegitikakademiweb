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
        private readonly NbDataContext _context;
        public SubscriptionBlogRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Aynı email ile üyelik varsa true döner.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckSubscriber(string email)
        {
            return _context.BlogSubscribers.Any(x => x.Email == email);
        }
    }
    public class SubscriptionNewsletterRepository : BaseRepository<NewsletterSubscriber, Guid>
    {
        private readonly NbDataContext _context;
        public SubscriptionNewsletterRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public bool CheckSubscriber(string email)
        {
            return _context.NewsletterSubscribers.Any(x => x.Email == email);
        }
    }

}
