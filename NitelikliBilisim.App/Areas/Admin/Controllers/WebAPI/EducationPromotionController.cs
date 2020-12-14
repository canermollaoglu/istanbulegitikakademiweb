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
        [Route("get-coupon-code-based-promotion-list")]
        public IActionResult GetCouponCodeBasedPromotionList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationPromotionCode.GetCouponCodeBasedPromotionList();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        [HttpGet]
        [Route("get-basket-based-promotion-list")]
        public IActionResult GetBasketBasedPromotionList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationPromotionCode.GetBasketBasedPromotionList();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-usage-promotion-list")]
        public IActionResult GetUsagePromotionList(DataSourceLoadOptions loadOptions,Guid promotionCodeId)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationPromotionCode.GetUsagePromotionList(promotionCodeId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        
    }
}
