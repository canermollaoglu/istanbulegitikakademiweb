using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{

    public class BlogCategoryController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-blog-categories")]
        public IActionResult GetBlogCategories(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.BlogCategory.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
