using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationCommentController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCommentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-education-comments")]
        public IActionResult GetEducationComments(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationComment.GetEducationCommentsQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-comment-status")]
        public IActionResult GetCommentStatus()
        {
            EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<CommentApprovalStatus>().Select(x =>
           new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();
            return Ok(retVal);
        }
    }
}
