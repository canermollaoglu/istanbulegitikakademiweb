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
            var model = _unitOfWork.EducationCategory.Get(null, order => order.OrderBy(o => o.Name), x => x.BaseCategory);
            return View(model);
        }

        [Route("admin/kategori-sil")]
        public IActionResult Delete(Guid? categoryId)
        {
            if (categoryId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });

            var subCategories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == categoryId).ToList();
            if (subCategories.Count > 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Silinmek istenilen kategori, bir ya da birden fazla kategoriyi barındırmaktadır. Lütfen önce o kategorileri siliniz ya da üst kategorisini güncelleyiniz." }
                });

            _unitOfWork.EducationCategory.Delete(categoryId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }
    }
}