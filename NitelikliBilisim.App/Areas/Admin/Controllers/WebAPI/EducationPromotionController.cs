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
    
    public class EducationPromotionController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationPromotionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-promotion-list")]
        public IActionResult GetCustomerList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationPromotionCode.GetPromotionCodeList();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

    }
}
