using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationGroupController : TempSecurityController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EmailSender _emailSender;
        public EducationGroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _emailSender = new EmailSender();
        }
        [Route("admin/gruplar")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationGrupList");
            return View();
        }

        [Route("admin/grup-detay/{groupId?}")]
        public IActionResult Detail(Guid groupId)
        {
            var groupDetail = _unitOfWork.EducationGroup.GetDetailByGroupId(groupId);

            return View(groupDetail);
        }

        public IActionResult GetGroupExpensesByGroupId(Guid groupId)
        {
            var expenses = _unitOfWork.EducationGroup.GetExpensesByGroupId(groupId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = expenses
            });
        }

        public IActionResult GetLessonDaysByGroupId(Guid groupId)
        {
            var lessonDays = _unitOfWork.EducationGroup.GetLessonDaysByGroupId(groupId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = lessonDays
            });
        }
        public IActionResult AssignedUserByGroupId(Guid groupId)
        {
            var students = _unitOfWork.EducationGroup.GetAssignedStudentsByGroupId(groupId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = students
            });
        }
        [Route("admin/get-eligible-student/{groupId?}")]
        public IActionResult EligibleUserByGroupId(Guid groupId)
        {
            var model = _unitOfWork.EducationGroup.GetEligibleStudents(groupId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }


        [Route("admin/grup-olustur")]
        public IActionResult Add()
        {
            var model = new AddGetVm
            {
                Educations = _unitOfWork.Education.Get(x => x.IsActive, x => x.OrderBy(o => o.CategoryId)),
                Hosts = _unitOfWork.EducationHost.Get(null, x => x.OrderBy(o => o.HostName))
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
        [Route("admin/get-class-rooms-by-host-id/{hostId?}")]
        public IActionResult GetClassRoomsByHostId(Guid? hostId)
        {
            if (!hostId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.EducationHost.GetClassRoomsByHostId(hostId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/add-group")]
        public async Task<IActionResult> Add(AddPostVm data)
        {
            if (!ModelState.IsValid || data.LessonDays == null || data.LessonDays.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            var isSuccess = _unitOfWork.EducationGroup.Insert(entity: new EducationGroup
            {
                IsGroupOpenForAssignment = true,
                GroupName = data.Name,
                EducationId = data.EducationId.Value,
                EducatorId = data.EducatorId,
                HostId = data.HostId.Value,
                StartDate = data.StartDate.Value,
                Quota = data.Quota.Value
            }, days: data.LessonDays,data.ClassRoomId,data.EducatorPrice);
            var emails = _unitOfWork.EmailHelper.GetAdminEmails();
            await _emailSender.SendAsync(new Core.ComplexTypes.EmailMessage
            {
                Body = "Grup açılmıştır",
                Contacts = emails.ToArray()
            });
            return Json(new ResponseModel
            {
                isSuccess = isSuccess
            });
        }

        [Route("admin/gruba-ogrenci-ata/{groupId?}")]
        public IActionResult AssignStudents(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("/admin/gruplar");

            var model = _unitOfWork.EducationGroup.GetAssignStudentsVm(groupId.Value);

            return View(model);
        }

        [Route("admin/get-eligible-and-assigned-students/{groupId?}")]
        public IActionResult GetEligibleAndAssignedStudents(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.EducationGroup.GetEligibleAndAssignedStudents(groupId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/assign-ticket")]
        public IActionResult AssignTicket(AssignPostVm data)
        {
            _unitOfWork.Ticket.AssignTicket(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
        [HttpPost, Route("admin/unassign-ticket")]
        public IActionResult UnassignTicket(UnassignPostVm data)
        {
            _unitOfWork.Ticket.UnassignTicket(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
        [Route("make-sure-lesson-days-created/{groupId}")]
        public IActionResult CreateGroupLessonDays(Guid groupId)
        {
            var groupDays = _unitOfWork.WeekDaysOfGroup.GetById(groupId);
            List<int> daysInt = null;
            if (groupDays != null)
                daysInt = JsonConvert.DeserializeObject<List<int>>(groupDays.DaysJson);

            _unitOfWork.GroupLessonDay.CreateGroupLessonDays(
                group: _unitOfWork.EducationGroup.Get(x => x.Id == groupId, null, x => x.Education).FirstOrDefault(),
                daysInt: daysInt,
                unwantedDays: new List<DateTime>(),
                isReset: true);

            return Json(true);
        }
    }
}