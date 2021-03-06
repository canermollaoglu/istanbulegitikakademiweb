using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.groups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class GroupExpenseController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupExpenseController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/add-group-expense")]
        public IActionResult Add(GroupExpense data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }

            try
            {
                _unitOfWork.GroupExpense.Insert(data);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Gider tipi başarı ile eklenmiştir."
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }

        }

        [Route("admin/delete-group-expense")]
        public IActionResult DeleteExpense(Guid expenseId)
        {
            try
            {
                _unitOfWork.GroupExpense.Delete(expenseId);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }
        }

        [HttpGet]
        [Route("admin/get-expense-list-by-group-id")]
        public IActionResult GetListByGroupId(Guid groupId)
        {
            try
            {
                List<GroupExpense> data = _unitOfWork.GroupExpense.GetListByGroupIdWidthExpenseType(groupId);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }

        }

    }
}
