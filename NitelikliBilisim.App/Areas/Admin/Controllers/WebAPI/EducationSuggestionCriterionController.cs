using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{

    public class EducationSuggestionCriterionController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationSuggestionCriterionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-education-list")]
        public IActionResult GetEducationList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Education.GetListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

    }
}
