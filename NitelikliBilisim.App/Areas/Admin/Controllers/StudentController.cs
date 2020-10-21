using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public StudentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Route("admin/ogrenci-yonetimi")]
        public IActionResult Index()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminStudentList");
            return View();
        }

        [Route("admin/ogrenci-hareketleri")]
        public IActionResult StudentLogs(string studentId)
        {

            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminStudentLogList");
            ViewData["studentId"] = studentId;

            return View();
        }

        [Route("admin/ogrenci-detay")]
        public IActionResult Detail(string studentId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminStudentDetail");
            var model = _unitOfWork.Customer.GetCustomerDetail(studentId);

            return View(model);
        }


        [Route("admin/student-list-fill-select")]
        public IActionResult StudentListFillSelect()
        {
            try
            {
                List<SelectListItem> studentList = _unitOfWork.Customer.GetCustomerListQueryable().Select(e => new SelectListItem
                {
                    Text = $"{e.Name} {e.Surname}",
                    Value = e.Id.ToString()
                }).ToList();

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = studentList
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata" + ex.Message }
                }); ;
            }

        }

    }
}
