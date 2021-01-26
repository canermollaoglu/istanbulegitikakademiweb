using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationCommentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCommentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/egitim-yorumlari")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationCommentList");
            
            return View();
        }



        [Route("admin/get-comment-status")]
        public IActionResult GetCommentStatus()
        {
            try
            {
                EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<CommentApprovalStatus>().Select(x =>
            new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = retVal
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { ex.Message }
                });
            }

        }


        [Route("admin/set-comment-status")]
        public IActionResult SetCommentStatus(Guid commentId,int status)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _unitOfWork.EducationComment.SetCommentStatus(commentId, (CommentApprovalStatus)status,userId);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("admin/toggle-highlight-comment")]
        public IActionResult ToggleHighlightComment(Guid commentId)
        {
            try
            {
                _unitOfWork.EducationComment.ToggleHighlight(commentId);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
