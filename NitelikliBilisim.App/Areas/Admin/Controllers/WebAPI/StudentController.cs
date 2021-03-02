using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.areas.admin.student;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class StudentController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly TransactionLogRepository _transactionLog;
        public StudentController(UnitOfWork unitOfWork,TransactionLogRepository transactionLog)
        {
            _unitOfWork = unitOfWork;
            _transactionLog = transactionLog;
        }

        [HttpGet]
        [Route("get-student-list")]
        public IActionResult GetCustomerList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.Customer.GetCustomerListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        [HttpGet]
        [Route("get-student-log-list")]
        public IActionResult Logs(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _transactionLog.GetListQueryable().Where(x => x.UserId == studentId).Select(x => new TransactionLogListViewModel
            {
                ActionName = x.ActionName,
                ControllerName = x.ControllerName,
                CreatedDate = x.CreatedDate,
                Id = x.Id,
                IpAddress = x.IpAddress,
                SessionId = x.SessionId,
                UserId = x.UserId
            });

            loadOptions.PrimaryKey = new[] { "Id" };

            return Ok(DataSourceLoader.Load(data, loadOptions));
        }


        [HttpGet]
        [Route("get-student-joined-groups")]
        public IActionResult GetJoinedGroups(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Customer.GetJoinedGroups(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }

        [HttpGet]
        [Route("get-student-payments")]
        public IActionResult GetStudentPayments(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Report.GetStudentBasedSalesReport(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }

        [HttpGet]
        [Route("get-student-absences")]
        public IActionResult GetStudentAbsences(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Customer.GetStudentAbsences(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-student-tickets")]
        public IActionResult GetStudentTickets(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Customer.GetStudentTickets(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }

        [HttpGet]
        [Route("get-student-used-promotions")]
        public IActionResult GetStudentUsedPromotions(DataSourceLoadOptions loadOptions, string studentId)
        {
            var data = _unitOfWork.Customer.GetStudentUsedPromotions(studentId);
            return Ok(DataSourceLoader.Load(data, loadOptions));

        }

        [HttpGet]
        [Route("get-jobs")]
        public IActionResult GetJobs()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<Jobs>().Select(x =>
           new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();
            return Ok(retVal);
        }
    }
}
