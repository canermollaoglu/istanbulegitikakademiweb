using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogPostRepository : BaseRepository<BlogPost,Guid>
    {
        NbDataContext _context;
        public BlogPostRepository(NbDataContext context):base(context)
        {
            _context = context;
        }
    }
}
