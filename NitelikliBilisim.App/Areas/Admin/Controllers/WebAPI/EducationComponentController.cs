using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost;
using NitelikliBilisim.Core.ViewModels.HelperVM;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationComponentController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationComponentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetComponentItems(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };

            var data = _unitOfWork.EducationComponentItem.GetQueryableList();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
        [HttpGet]
        [Route("component-types")]
        public IActionResult GetComponentTypes()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<EducationComponentType>().Select(x =>
                new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();

            return Ok(retVal);
        }
        [HttpGet]
        [Route("suggestion-types")]
        public IActionResult GetSuggestionTypes()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<EducationComponentSuggestionType>().Select(x =>
                new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();

            return Ok(retVal);
        }
    }
}
