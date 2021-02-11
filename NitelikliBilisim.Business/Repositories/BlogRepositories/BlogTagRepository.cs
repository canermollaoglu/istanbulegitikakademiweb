using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogTagRepository:BaseRepository<BlogTag,Guid>
    {
        NbDataContext _context;
        public BlogTagRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

    }
}
