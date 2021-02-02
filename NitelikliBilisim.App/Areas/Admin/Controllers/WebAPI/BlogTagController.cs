using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{

    public class BlogTagController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogTagController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-blog-tags")]
        public IActionResult GetBlogTags(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.BlogTag.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
