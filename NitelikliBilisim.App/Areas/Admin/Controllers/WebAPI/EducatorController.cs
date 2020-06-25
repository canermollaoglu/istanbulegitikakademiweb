using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Educator;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    [Route("api/[controller]")]
    public class EducatorController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-educators-list")]
        public IActionResult GetEducatorslist(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Educator.GetListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }


    }
}
