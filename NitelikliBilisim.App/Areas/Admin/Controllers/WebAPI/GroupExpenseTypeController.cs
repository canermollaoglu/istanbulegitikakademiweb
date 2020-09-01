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
    
    public class GroupExpenseTypeController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupExpenseTypeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-group-expense-types")]
        public IActionResult GetGroupExpenseTypes(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.GroupExpenseType.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
