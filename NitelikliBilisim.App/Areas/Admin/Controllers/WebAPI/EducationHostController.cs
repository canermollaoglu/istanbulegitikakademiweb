using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationHostController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationHostController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-education-host-list")]
        public IActionResult GetCertificateList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationHost.GetListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }



    }
}
