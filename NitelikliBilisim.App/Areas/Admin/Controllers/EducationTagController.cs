using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.Debugging;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_tags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationTagController : TempSecurityController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationTagController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/etiket-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationTagAdd");
            var data = _unitOfWork.EducationTag.Get(null, q => q.OrderBy(o => o.Name));
            var model = new AddGetVm
            {
                Tags = data
            };
            return View(model);
        }

        [Route("admin/etiket-guncelle/{tagId}")]
        public IActionResult Update(Guid? tagId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationTagUpdate");
            if (tagId == null)
                return Redirect("/admin/etiketler");

            var tag = _unitOfWork.EducationTag.GetById(tagId.Value);
            var tags = _unitOfWork.EducationTag.Get(null, q => q.OrderBy(o => o.Name));
            var model = new UpdateGetVm
            {
                Id = tagId.Value,
                Tag = tag,
                Tags = tags
            };
            return View(model);
        }
        [HttpPost, Route("admin/etiket-ekle")]
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
                Name = StringHelpers.FormatForTag(data.Name),
                Description = data.Description
            });
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla eklenmiştir"
            });
        }
        [HttpPost, Route("admin/etiket-guncelle")]
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
            var tag = _unitOfWork.EducationTag.GetById(data.TagId);
            tag.Description = data.Description;
            tag.Name = data.Name;
            _unitOfWork.EducationTag.Update(tag);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla güncellenmiştir"
            });
        }

        [Route("admin/etiketler")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationTagList");
            var performer = new Performer();
            var model = _unitOfWork.EducationTag.Get(null, order => order.OrderBy(o => o.Name));
            performer.Watch("List");

            return View(model);
        }

        [Route("admin/get-tag-list")]
        public JsonResult GetList()
        {
            var performer = new Performer();
            var model = _unitOfWork.EducationTag.Get(null, order => order.OrderBy(o => o.Name));
            performer.Watch("List");

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }


        [Route("admin/etiket-sil")]
        public IActionResult Delete(Guid? tagId)
        {
            if (tagId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });

            //var subTags = _unitOfWork.EducationTag.Get(x => x.BaseTagId == tagId).ToList();
            //if (subTags.Count > 0)
            //    return Json(new ResponseModel
            //    {
            //        isSuccess = false,
            //        errors = new List<string> { "Silinmek istenilen etiket, bir ya da birden fazla etiketi barındırmaktadır. Lütfen önce o etiketleri siliniz ya da üst etiketini güncelleyiniz." }
            //    });

            _unitOfWork.EducationTag.Delete(tagId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }


        [HttpGet]
        public IActionResult SearchTag(string q)
        {
            try
            {
                var tags = _unitOfWork.EducationTag.Get(x => x.Name.StartsWith(q)).ToList();
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = tags
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