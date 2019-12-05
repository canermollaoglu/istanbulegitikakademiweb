using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.Models.Category;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationCategoryController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/kategori-ekle")]
        public IActionResult Add()
        {
            var data = _unitOfWork.EducationCategory.Get(null, q => q.OrderBy(o => o.Name));
            var model = new AddGetVm
            {
                Categories = data
            };
            return View(model);
        }

        [HttpPost, Route("admin/kategori-ekle")]
        public JsonResult Add(AddPostVm data)
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
            _unitOfWork.EducationCategory.Add(new Core.Entities.EducationCategory
            {
                Name = data.Name,
                Description = data.Description,
                BaseCategoryId = data.BaseCategoryId
            });
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla eklenmiştir"
            });
        }

        [Route("admin/kategoriler")]
        public IActionResult List()
        {

            return View();
        }
    }
}