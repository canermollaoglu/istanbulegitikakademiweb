using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator_certificate;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducatorCertificateController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storageService;

        public EducatorCertificateController(UnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
            _storageService = storageService;
        }

        [Route("admin/egitmensertifika/ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorCertificateAdd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/egitmensertifika/ekle")]
        public async Task<IActionResult> Add(EducatorCertificateAddVM data)
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
                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.CertificateImage.Base64Content));
                var fileName = StringHelpers.FormatForTag(data.Name);
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{data.CertificateImage.Extension}", "educator-certificate-images");

                var newCertificate = new EducatorCertificate
                {
                    Name = data.Name,
                    Description = data.Description,
                    CertificateImagePath = dbPath
                };
                _unitOfWork.EducatorCertificate.Insert(newCertificate);

                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (System.Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                }) ;
            }

        }


        [Route("admin/egitmensertifika/sertifikalar")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorCertificateList");
            return View();
        }


         [Route("admin/educatorcertificate/delete")]
        public IActionResult Delete(int certificateId)
        {
            if (certificateId == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });

            _unitOfWork.EducatorCertificate.Delete(certificateId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }


        [Route("admin/egitmensertifika/guncelle")]
        public IActionResult Update(int certificateId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorCertificateUpdate");
            var certificate = _unitOfWork.EducatorCertificate.GetById(certificateId);
            var certificateVm = new EducatorCertificateUpdateGetVM
            {
                Id = certificate.Id,
                Name = certificate.Name,
                Description = certificate.Description,
                CertificateImagePath = certificate.CertificateImagePath
            };
            return View(certificateVm);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("admin/egitmensertifika/guncelle")]
        public async Task<IActionResult> Update(EducatorCertificateUpdateVM data)
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
            var certificate = _unitOfWork.EducatorCertificate.GetById(data.Id);
            if (!string.IsNullOrEmpty(data.CertificateImage.Base64Content))
            {
                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.CertificateImage.Base64Content));
                var fileName = data.Name.FormatForTag();
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{data.CertificateImage.Extension}", "educator-certificate-images");
                
                certificate.CertificateImagePath = dbPath;
            }
            certificate.Name = data.Name;
            certificate.Description = data.Description;

            _unitOfWork.EducatorCertificate.Update(certificate);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Sertifika başarıyla güncellenmiştir"
            });
        }
    }
}
