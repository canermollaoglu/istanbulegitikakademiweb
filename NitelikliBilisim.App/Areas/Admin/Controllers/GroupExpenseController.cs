using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.groups;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class GroupExpenseController : Controller
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
    }
}
