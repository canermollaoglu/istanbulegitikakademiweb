using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.HelperVM;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationController :   BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationController(UnitOfWork unitOfWork)
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

        [HttpGet]
        [Route("get-levels")]
        public IActionResult GetHostCities()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<EducationLevel>().Select(x =>
           new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();

            return Ok(retVal);
        }

    }
}
