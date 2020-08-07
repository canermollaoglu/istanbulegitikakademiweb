using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Blog
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class BlogCategoryController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(BlogCategory data)
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
                _unitOfWork.BlogCategory.Insert(data);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Blog kategorisi başarı ile eklenmiştir."
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

        public IActionResult Delete(Guid? categoryId)
        {
            if (!categoryId.HasValue)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata: Sayfayı yenileyerek tekrar deneyiniz." }
                });
            }
            try
            {
                _unitOfWork.BlogCategory.Delete(categoryId.Value);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Kategori başarıyla silinmiştir."
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
