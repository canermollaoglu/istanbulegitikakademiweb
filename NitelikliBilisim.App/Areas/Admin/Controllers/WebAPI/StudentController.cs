using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using System;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class StudentController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IElasticClient _elasticClient;
        public StudentController(UnitOfWork unitOfWork, IElasticClient elasticClient)
        {
            _unitOfWork = unitOfWork;
            _elasticClient = elasticClient;
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
        public IActionResult Logs(DataSourceLoadOptions loadOptions,string studentId)
        {
            var count = _elasticClient.Count<TransactionLog>(s => s.Query
            (q => q.Term(t => t.UserId, studentId)));
            var data = _elasticClient.Search<TransactionLog>(s =>
            s.From(0).Size((int)count.Count).Query(q => q.Term(t => t.UserId, studentId))).Documents.Select(x => new TransactionLogListViewModel
            {
                ActionName = x.ActionName,
                ControllerName = x.ControllerName,
                CreatedDate = x.CreatedDate,
                Id = x.Id,
                IpAddress = x.IpAddress,
                Parameters = JsonConvert.SerializeObject(x.Parameters),
                SessionId = x.SessionId,
                UserId = x.UserId
            }).ToList();

            loadOptions.PrimaryKey = new[] { "Id" };

            var lastData = DataSourceLoader.Load(data, loadOptions);
            lastData.totalCount = (int)count.Count;

            return Ok(lastData);
        }

    }
}
