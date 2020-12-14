using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationHostController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public EducationHostController(UnitOfWork unitOfWork, IStorageService storage, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _storage = storage;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
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
                HostCities = EnumHelpers.ToKeyValuePair<HostCity>()
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
                    Longitude = data.Longitude,
                    GoogleMapUrl = data.GoogleMapUrl
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
                HostCities = EnumHelpers.ToKeyValuePair<HostCity>()
            };
            return View(educationHostVm);
        }

        [HttpGet]
        [Route("admin/egitim-kurumlari/sinif-yonetimi")]
        public IActionResult ClassList(Guid educationHostId)
        {
            var host = _unitOfWork.EducationHost.GetById(educationHostId);
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationClassRoomList");
            ViewData["educationHostId"] = educationHostId;
            ViewData["educationHostName"] = host.HostName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddClassRoom(Classroom data)
        {
            if (string.IsNullOrEmpty(data.Name))
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sınıf için geçerli bir isim girmelisiniz." }
                });
            }

            try
            {
                 _unitOfWork.ClassRoom.Insert(data);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Sınıf başarıyla güncellenmiştir."
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

        [HttpPost]
        public IActionResult DeleteClassRoom(Guid id)
        {
            try
            {
                _unitOfWork.ClassRoom.Delete(id);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Sınıf başarıyla silinmiştir."
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

        [HttpPost]
        public IActionResult UpdateClassRoom(Classroom data)
        {
            if (string.IsNullOrEmpty(data.Name))
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sınıf için geçerli bir isim girmelisiniz." }
                });
            }
            try
            {
                var classRoom = _unitOfWork.ClassRoom.GetById(data.Id);
                classRoom.Name = data.Name;
                _unitOfWork.ClassRoom.Update(classRoom);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Sınıf başarıyla güncellenmiştir."
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
                educationHost.GoogleMapUrl = data.GoogleMapUrl;
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



        [HttpGet]
        [Route("admin/egitim-kurumlari/gorsel-yonetimi")]
        public IActionResult ManageImages(Guid? educationHostId)
        {
            if (educationHostId == null)
                return Redirect("/");

            var educationHost = _unitOfWork.EducationHost.GetById(educationHostId.Value);
            var model = new EducationHostImageManageVm
            {
                EducationHostId = educationHost.Id,
                EducationHostName = educationHost.HostName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/egitim-kurumlari/gorsel-ekle")]
        public async Task<IActionResult> AddHostImage(EducationHostImageAddVm data)
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
                var educationHost = _unitOfWork.EducationHost.GetById(data.EducationHostId);
                var mediaStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PostedFile.Base64Content));
                var mediaPath = await _storage.UploadFile(mediaStream, $"{educationHost.HostName}.{data.PostedFile.Extension.ToLower()}", "education-host-images");

                _unitOfWork.EducationHostImage.Insert(new EducationHostImage
                {
                    EducationHostId = educationHost.Id,
                    IsActive = true,
                    FileUrl = mediaPath
                });
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Dosya yüklenirken hata oluştu" }
                });
            }
        }

        [Route("admin/egitim-kurumlari/gorsel-sil")]
        public IActionResult DeteleHostImage(Guid? educationHostImageId)
        {
            if (educationHostImageId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sayfayı yenileyerek tekar deneyin." }
                });
            var educationHostImage = _unitOfWork.EducationHostImage.GetById(educationHostImageId.Value);
            if (educationHostImage != null)
            {
                _fileManager.Delete(educationHostImage.FileUrl);
                _unitOfWork.EducationHostImage.Delete(educationHostImageId.Value);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            return Json(new ResponseModel
            {
                isSuccess = false,
                errors = new List<string> { "Sayfayı yenileyerek tekar deneyin." }
            });

        }

        [HttpGet]
        [Route("admin/egitim-kurumlari/gorsel-listele")]
        public IActionResult GetHostImages(Guid? educationHostId)
        {
            if (educationHostId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Kurum görsellerini getirirken bir hata oluştu" }
                });

            var images = _unitOfWork.EducationHostImage.GetByEducationHostId(educationHostId.Value);

            List<EducationHostImageGetVm> model = new List<EducationHostImageGetVm>();

            foreach (var image in images)
            {
                EducationHostImageGetVm educationHostImageVm = new EducationHostImageGetVm
                {
                    Id = image.Id,
                    IsActive = image.IsActive
                };
                try
                {
                    educationHostImageVm.FullPath = _storage.DownloadFile(Path.GetFileName(image.FileUrl), "education-host-images").Result;
                }
                catch
                {
                }
                model.Add(educationHostImageVm);
            }

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpGet]
        [Route("admin/egitim-kurumlari/gorsel-statu-degistir")]
        public IActionResult ChangeStatusHostImage(Guid? educationHostImageId)
        {
            if (educationHostImageId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Kurum görsel statüsü değiştirilirken bir hata oluştu" }
                });

            var image = _unitOfWork.EducationHostImage.GetById(educationHostImageId.Value);
            image.IsActive = image.IsActive ? false : true;
            _unitOfWork.EducationHostImage.Update(image);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

    }
}


