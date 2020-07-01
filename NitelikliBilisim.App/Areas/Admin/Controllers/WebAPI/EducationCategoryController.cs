using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationCategoryController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-base-category-list")]
        public IActionResult GetBaseCategoryList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationCategory.GetBaseCategoryListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-categories-by-base-category-id")]
        public IActionResult GetCategoriesByBaseCategoryId(DataSourceLoadOptions loadOptions,string baseCategoryId)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationCategory.GetCategoriesByBaseCategoryId(Guid.Parse(baseCategoryId));
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
