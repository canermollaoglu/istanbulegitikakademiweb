using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducatorController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly UnitOfWork _unitOfWork;
        public EducatorController(IHostingEnvironment hostingEnvironment, UnitOfWork unitOfWork)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
        }
        [Route("admin/egitmen-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorAdd");
            return View();
        }

        [HttpPost, Route("admin/egitmen-ekle")]
        public IActionResult Add(AddPostVm data)
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

            _fileManager.Upload("/uploads/educator-photos/", data.ProfilePhoto.Base64Content, data.ProfilePhoto.Extension, "profile-photo", $"{data.Name} {data.Surname}");

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}