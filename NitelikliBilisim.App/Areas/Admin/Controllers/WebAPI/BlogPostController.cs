using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class BlogPostController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogPostController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-blog-posts")]
        public IActionResult GetBlogPosts(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.BlogPost.Get().Include(x => x.Category).Select(x => new BlogPostListVM
            {
                Id = x.Id,
                Title = x.Title,
                CategoryName = x.Category.Name,
                CreatedDate = x.CreatedDate
            });
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
