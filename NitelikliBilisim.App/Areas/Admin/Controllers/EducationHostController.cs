using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_host;
using NitelikliBilisim.Support.Enums;
using System;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationHostController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationHostController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("admin/egitim-kurumlari")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationHostList");
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationHostAdd");
            var model = new EducationHostAddGetVm
            {
                HostCities = EnumSupport.ToKeyValuePair<HostCity>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(EducationHostAddPostVm data)
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
                var host = new EducationHost
                {
                    HostName = data.HostName,
                    Address = data.Address,
                    City = (HostCity)data.City.GetValueOrDefault(),
                    Latitude = data.Latitude,
                    Longitude = data.Longitude
                };
                _unitOfWork.EducationHost.Insert(host);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [HttpGet]
        [Route("admin/egitim-kurumlari/guncelle")]
        public IActionResult Update(Guid educationHostId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationHostUpdate");
            var educationHost = _unitOfWork.EducationHost.GetById(educationHostId);
            var educationHostVm = new EducationHostUpdateGetVm
            {
                EducationHost = educationHost,
                HostCities = EnumSupport.ToKeyValuePair<HostCity>()
            };
            return View(educationHostVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/egitim-kurumlari/guncelle")]
        public IActionResult Update(EducationHostUpdatePostVm data)
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
                var educationHost = _unitOfWork.EducationHost.GetById(data.Id);
                educationHost.HostName = data.HostName;
                educationHost.Address = data.Address;
                educationHost.Latitude = data.Latitude;
                educationHost.Longitude = data.Longitude;
                educationHost.City = (HostCity)data.City;
                _unitOfWork.EducationHost.Update(educationHost);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }


        [HttpGet]
        [Route("admin/egitim-kurumlari/sil")]
        public IActionResult Delete(Guid educationHostId)
        {
            try
            {
                _unitOfWork.EducationHost.Delete(educationHostId);
            }
            catch (Exception)
            {
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Silme işlemi gerçekleşmedi! Bu işlem için sistem yöneticiniz ile iletişime geçiniz."
                });
            }
            
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });

        }

    }

}

