using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.helper;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{

    public class OffDayController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public OffDayController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-offday-list")]
        public IActionResult GetOffDayList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.OffDay.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
