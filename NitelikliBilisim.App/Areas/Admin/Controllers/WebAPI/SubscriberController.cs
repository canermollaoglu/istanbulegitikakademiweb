using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class SubscriberController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public SubscriberController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-blog-subscribers-list")]
        public IActionResult GetBlogSubscribers(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.SubscriptionBlog.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        [HttpGet]
        [Route("get-newsletter-subscribers-list")]
        public IActionResult GetNewsLetterSubscribers(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.SubscriptionNewsletter.Get();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
