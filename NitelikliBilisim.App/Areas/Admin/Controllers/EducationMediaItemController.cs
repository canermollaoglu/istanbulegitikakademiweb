using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationMediaItems;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationMediaItemController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationMediaItemVmCreator _vmCreator;
        public EducationMediaItemController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationMediaItemVmCreator(_unitOfWork);
        }
        [Route("admin/egitim-medya-yonetimi/{educationId}")]
        public IActionResult Manage(Guid? educationId)
        {
            if (educationId == null)
                return Redirect("/");
            var model = _vmCreator.CreateManageVm(educationId.Value);

            return View(model);
        }
        [Route("admin/get-education-media-items/{educationId}")]
        public IActionResult GetEducationMediaItems(Guid? educationId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin medyalarını getirirken bir hata oluştu" }
                });

            var model = _vmCreator.CreateEducationMediaItemsVm(educationId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
        [HttpPost, Route("admin/add-education-media-item")]
        public IActionResult AddMediaItem(AddMediaItemVm data)
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

            _unitOfWork.EducationMedia.Insert(new Core.Entities.EducationMedia
            {
                EducationId = data.EducationId,
                MediaType = (EducationMediaType)data.MediaItemType,
                FileUrl = null
            });

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/delete-education-media-item/{mediaItemId}")]
        public IActionResult DeleteMediaItem(Guid? mediaItemId)
        {
            if (mediaItemId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin medyasını silerken bir hata oluştu" }
                });

            _unitOfWork.EducationMedia.Delete(mediaItemId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}
