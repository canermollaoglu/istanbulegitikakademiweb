using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.helper;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class OffDayController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public OffDayController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Route("admin/offday")]
        public IActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(OffDay data)
        {
            try
            {
                #region Validasyonlar
                var temp = _unitOfWork.OffDay.IsDuplicate(data.Day, data.Month, data.Year);
                if (temp)
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { "Tatil günü zaten kayıtlı." }
                    });
                }

                if (string.IsNullOrEmpty(data.Name) || data.Day<1 || data.Month<1||data.Year<1)
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { "Tatil nedeni girilmeli ve geçerli bir tarih seçilmelidir." }
                    });
                }

                #endregion
                if (data.Id == 0)
                    _unitOfWork.OffDay.Insert(data);
                else
                    _unitOfWork.OffDay.Update(data);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Tatil günü başarıyla güncellenmiştir."
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
        public IActionResult Delete(int id)
        {
            try
            {
                _unitOfWork.OffDay.Delete(id);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Tatil günü başarıyla silinmiştir."
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
