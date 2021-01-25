using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using System.Linq;

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

        [HttpGet]
        [Route("host-cities")]
        public IActionResult GetHostCities()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<HostCity>().Select(x =>
           new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();

            return Ok(retVal);
        }
    }
}
