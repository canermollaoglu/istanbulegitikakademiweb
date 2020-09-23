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
   
    public class EarningsReportController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EarningsReportController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-group-based-sales-report")]
        public IActionResult GetGroupBasedSalesReport(DataSourceLoadOptions loadOptions,Guid groupId)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Report.GetGroupBasedSalesReportStudents(groupId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        
            [HttpGet]
        [Route("get-group-based-cancellation-sales-report")]
        public IActionResult GetGroupBasedCancellationSalesReport(DataSourceLoadOptions loadOptions, Guid groupId)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Report.GetGroupBasedCancellationSalesReport(groupId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        public IActionResult GetStudentBasedSalesReport(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Report.GetStudentBasedSalesReport(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-general-sales-report")]
        public IActionResult GetGeneralSalesReport(DataSourceLoadOptions loadOptions)
        {
            var data = _unitOfWork.Report.GetGeneralSalesReport();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
