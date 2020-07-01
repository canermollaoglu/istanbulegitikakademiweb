using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationGroupController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-education-group-list")]
        public IActionResult GetEducationGroupList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationGroup.GetListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
