using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MUsefulMethods;
using NitelikliBilisim.App.Areas.Admin.Models.Category;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.Debugging;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationCategoryController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/kategori-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationCategoryAdd");
            var data = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, q => q.OrderBy(o => o.Name));
            var model = new AddGetVm
            {
                Categories = data,
                Types = EnumHelpers.ToKeyValuePair<CategoryType>()
            };
            return View(model);
        }

        [Route("admin/kategori-guncelle/{categoryId}")]
        public IActionResult Update(Guid? categoryId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationCategoryUpdate");
            if (categoryId == null)
                return Redirect("/admin/kategoriler");

            var category = _unitOfWork.EducationCategory.GetById(categoryId.Value);
            var categories = _unitOfWork.EducationCategory.Get(null, q => q.OrderBy(o => o.Name));
            EducationCategory baseCategory = null;
            if (category.BaseCategoryId.HasValue)
                baseCategory = _unitOfWork.EducationCategory.GetById(category.BaseCategoryId.Value);
            var model = new UpdateGetVm
            {
                Category = category,
                Categories = categories,
                BaseCategory = baseCategory
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
            _unitOfWork.EducationCategory.Insert(new EducationCategory
            {
                Name = data.Name,
                Description = data.Description,
                SeoUrl = data.SeoUrl,
                IconUrl = data.IconUrl,
                IconColor = data.IconColor,
                EducationDayCount = data.EducationDayCount,
                BaseCategoryId = data.BaseCategoryId,
                CategoryType = (CategoryType)data.CategoryType,
                IsCurrent = true
            });
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla eklenmiştir"
            });
        }
        [HttpPost, Route("admin/kategori-guncelle")]
        public IActionResult Update(UpdatePostVm data)
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
            var category = _unitOfWork.EducationCategory.GetById(data.CategoryId);
            category.BaseCategoryId = data.BaseCategoryId;
            category.Description = data.Description;
            category.Name = data.Name;
            category.SeoUrl = data.SeoUrl;
            category.EducationDayCount = data.EducationDayCount;
            category.IconUrl = data.IconUrl;
            category.IconColor = data.IconColor;

            _unitOfWork.EducationCategory.Update(category);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla güncellenmiştir"
            });
        }

        [Route("admin/kategoriler")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationCategoryList");
            var performer = new Performer();
            var model = _unitOfWork.EducationCategory.Get(null, order => order.OrderBy(o => o.Name), x => x.BaseCategory);
            performer.Watch("List");

            return View(model);
        }

        [Route("admin/get-category-list")]
        public JsonResult GetList()
        {
            //ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationCategoryGetList");
            var performer = new Performer();
            var model = _unitOfWork.EducationCategory.Get(null, order => order.OrderBy(o => o.Name), x => x.BaseCategory);
            performer.Watch("List");

            return Json(new ResponseModel(){ 
                isSuccess = true,
                data = model
            });
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

        [Route("admin/education-categories-list-fill-select")]
        public IActionResult EducationCategoryListFillSelect()
        {
            try
            {
                List<SelectListItem> categoryList = _unitOfWork.EducationCategory.Get().Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Id.ToString()
                }).ToList();

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = categoryList
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata" + ex.Message }
                }); ;
            }

        }

    }
}