using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Applications
{
    public class CorporateMembershipApplicationController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public CorporateMembershipApplicationController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminCorporateMembershipApplicationList");
            return View();
        }

        /// <summary>
        /// Kayıt görüldü bilgisini true olarak günceller.
        /// </summary>
        /// <param name="cMAID">corporateMembershipId</param>
        /// <returns></returns>
        public IActionResult SetViewedStatus(Guid cMAID)
        {
            try
            {
                var corporateMembershipApplication = _unitOfWork.CorporateMembershipApplication.GetById(cMAID);
                corporateMembershipApplication.IsViewed = true;
                _unitOfWork.CorporateMembershipApplication.Update(corporateMembershipApplication);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata: {ex.Message}" }
                });
            }
        }

    }
}
