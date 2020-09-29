using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.groups;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class GroupExpenseTypeController : Controller
    {

        private readonly UnitOfWork _unitOfWork;
        public GroupExpenseTypeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminExpenseTypeList");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(GroupExpenseType data)
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
                _unitOfWork.GroupExpenseType.Insert(data);
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

        public IActionResult Delete(Guid? expenseTypeId)
        {
            if (!expenseTypeId.HasValue)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata: Sayfayı yenileyerek tekrar deneyiniz." }
                });
            }
            try
            {
                _unitOfWork.GroupExpenseType.Delete(expenseTypeId.Value);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Gider tipi başarıyla silinmiştir."
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
        public IActionResult Update(Guid? expenseTypeId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminExpenseTypeUpdate");
            if (expenseTypeId == null)
                return Redirect("/admin/GroupExpenseType/list");
            var expenseType = _unitOfWork.GroupExpenseType.GetById(expenseTypeId.Value);
            return View(expenseType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(GroupExpenseType data)
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
                _unitOfWork.GroupExpenseType.Update(data);
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


        [Route("admin/get-expense-types")]
        public IActionResult GetExpenseTypes()
        {
            var model = _unitOfWork.GroupExpenseType.Get().ToList();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
    }
}
