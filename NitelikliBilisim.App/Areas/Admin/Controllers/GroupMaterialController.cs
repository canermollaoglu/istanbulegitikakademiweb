using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_material;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("admin"), Authorize(Roles = "Admin")]
    public class GroupMaterialController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupMaterialController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/grup-materyalleri/{groupId?}")]
        public IActionResult Manage(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("/admin/gruplar");
            return View(groupId.Value);
        }

        [HttpPost, Route("admin/add-material-item")]
        public IActionResult AddMaterial(AddMaterialPostVm data)
        {
            var isSuccess = _unitOfWork.Material.AddMaterial(data);
            return Json(new ResponseModel
            {
                isSuccess = isSuccess
            });
        }
        [Route("admin/get-materials/{groupId?}")]
        public IActionResult GetMaterials(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
            var model = _unitOfWork.Material.GetMaterials(groupId.Value).Materials;
            return Json(new ResponseModel
            {
                 isSuccess = true,
                 data = model
            });
        }
    }
}