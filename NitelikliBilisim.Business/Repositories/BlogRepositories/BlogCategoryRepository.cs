using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogCategoryRepository :BaseRepository<BlogCategory,Guid>
    {

        NbDataContext _context;
        public BlogCategoryRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public List<BlogCategoryNameIdVm> GetListForBlogListPage()
        {
            return _context.BlogCategories.Include(x=>x.BlogPosts).Select(x => new BlogCategoryNameIdVm
            {
                Id = x.Id,
                Name = x.Name,
                PostCount = x.BlogPosts.Count()
            }).ToList();
        }
    }
}
