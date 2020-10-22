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
    
    public class CorporateMembershipApplicationController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public CorporateMembershipApplicationController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-corporate-membership-applications")]
        public IActionResult GetCorporateMembershipApplications(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.CorporateMembershipApplication.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
