using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class HomeController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public HomeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-last-sales")]
        public IActionResult GetLastSales(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Dashboard.GetLastSales();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }

        [HttpGet]
        [Route("get-last-refunds")]
        public IActionResult GetLastRefunds(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Dashboard.GetLastRefunds();
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }
    }
}
