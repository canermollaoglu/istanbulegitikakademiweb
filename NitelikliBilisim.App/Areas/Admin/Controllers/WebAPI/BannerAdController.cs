using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class BannerAdController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public BannerAdController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-blog-banner-ads")]
        public IActionResult GetBannerAds(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.BannerAds.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
