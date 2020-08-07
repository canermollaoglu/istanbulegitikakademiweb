using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogCategoryRepository :BaseRepository<BlogCategory,Guid>
    {

        NbDataContext _context;
        public BlogCategoryRepository(NbDataContext context):base(context)
        {
            _context = context;
        }
    }
}
