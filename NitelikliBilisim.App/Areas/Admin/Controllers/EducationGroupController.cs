﻿using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using System;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationGroupController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult List()
        {
            return View();
        }

        [Route("admin/grup-olustur")]
        public IActionResult Add()
        {
            var model = new AddGetVm
            {
                Educations = _unitOfWork.Education.Get(x => x.IsActive, x => x.OrderBy(o => o.CategoryId)),
                Hosts = _unitOfWork.EductionHost.Get(null, x => x.OrderBy(o => o.HostName))
            };
            return View(model);
        }

        [Route("admin/get-assigned-educators-for-group-add/{educationId?}")]
        public IActionResult GetEducatorsOfEducation(Guid? educationId)
        {
            if (!educationId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(educationId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/grup-olustur")]
        public IActionResult Add(AddPostVm data)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}