using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.WebAPI
{
    public class EducationHostController : BaseApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationHostController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-education-host-list")]
        public IActionResult GetEducationHostList(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.EducationHost.GetListQueryable();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        [Route("get-education-host-classroom-list")]
        public IActionResult GetEducationHostClassRoomList(DataSourceLoadOptions loadOptions,Guid educationHostId)
        {
            loadOptions.PrimaryKey = new[] { "Id" };
            var data = _unitOfWork.ClassRoom.GetClassRoomsByHostIdQueryable(educationHostId);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }



    }
}
