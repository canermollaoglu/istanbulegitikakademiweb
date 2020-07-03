using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducatorCertificateController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorCertificateController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-certificate-list")]
        public  IActionResult GetCertificateList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducatorCertificate.GetListQueryable();
            return Ok(DataSourceLoader.Load(data,loadOptions));
        }

    }
}
