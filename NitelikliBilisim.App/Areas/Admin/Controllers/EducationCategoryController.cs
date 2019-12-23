﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.Models.Category;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.Debugging;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;

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
            var data = _unitOfWork.EducationTag.Get(null, q => q.OrderBy(o => o.Name));
            var model = new AddGetVm
            {
                Categories = data
            };
            return View(model);
        }

        [Route("admin/kategori-guncelle/{categoryId}")]
        public IActionResult Update(Guid? categoryId)
        {
            if (categoryId == null)
                return Redirect("/admin/kategoriler");

            var category = _unitOfWork.EducationTag.GetById(categoryId.Value);
            var categories = _unitOfWork.EducationTag.Get(null, q => q.OrderBy(o => o.Name));
            EducationTag baseTag = null;
            if (category.BaseTagId.HasValue)
                baseTag = _unitOfWork.EducationTag.GetById(category.BaseTagId.Value);
            var model = new UpdateGetVm
            {
                Tag = category,
                Categories = categories,
                BaseTag = baseTag
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
            _unitOfWork.EducationTag.Insert(new EducationTag
            {
                Name = data.Name,
                Description = data.Description,
                BaseTagId = data.BaseCategoryId
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
            var category = _unitOfWork.EducationTag.GetById(data.CategoryId);
            category.BaseTagId = data.BaseCategoryId;
            category.Description = data.Description;
            category.Name = data.Name;
            _unitOfWork.EducationTag.Update(category);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla güncellenmiştir"
            });
        }

        [Route("admin/kategoriler")]
        public IActionResult List()
        {
            var performer = new Performer();

            var model = _unitOfWork.EducationTag.Get(null, order => order.OrderBy(o => o.Name), x => x.BaseTag);

            performer.Watch("List");

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

            var subCategories = _unitOfWork.EducationTag.Get(x => x.BaseTagId == categoryId).ToList();
            if (subCategories.Count > 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Silinmek istenilen kategori, bir ya da birden fazla kategoriyi barındırmaktadır. Lütfen önce o kategorileri siliniz ya da üst kategorisini güncelleyiniz." }
                });

            _unitOfWork.EducationTag.Delete(categoryId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }
    }
}