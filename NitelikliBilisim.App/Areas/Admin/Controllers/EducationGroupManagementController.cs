using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Notificator.Services;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("admin"), Authorize(Roles = "Admin")]
    public class EducationGroupManagementController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EmailSender _emailSender;
        public EducationGroupManagementController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _emailSender = new EmailSender();
        }
        [Route("admin/grup/ayarlar/{groupId?}")]
        public IActionResult Management(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("admin/gruplar");
            var model = _unitOfWork.GroupLessonDay.CreateManagementVm(groupId.Value);
            return View(model);
        }

        [Route("admin/get-lesson-days/{groupId?}")]
        public IActionResult GetLessonDays(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.GroupLessonDay.GetGroupLessonDays(groupId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("admin/determine-postpone-dates")]
        public IActionResult DeterminePostponeDates(Guid? groupId, DateTime from, DateTime? to)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.GroupLessonDay.DeterminePostponeDates(groupId.Value, from, to);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
        [Route("admin/determine-to-be-changed-dates")]
        public IActionResult DetermineToBeChangedDates(Guid? groupId, DateTime from, DateTime? to)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.GroupLessonDay.DetermineToBeChangedDatesAsText(groupId.Value, from, to);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });

        }
        [HttpPost, Route("admin/postpone-dates")]
        public async Task<IActionResult> PostponeDates(PostponeData data)
        {
            if (!data.groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            _unitOfWork.GroupLessonDay.PostponeLessons(data.groupId.Value, data.from, data.to);
            
            var emails = _unitOfWork.EmailHelper.GetEmailsOfStudentsByGroup(data.groupId.Value);
            emails.Add(_unitOfWork.EmailHelper.GetEmailOfTeacherAtDate(data.groupId.Value, data.from));
            await _emailSender.SendAsync(new EmailMessage {
                Contacts = emails.ToArray()
            });

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = null
            });
        }
        [HttpPost, Route("admin/switch-educators")]
        public async Task<IActionResult> SwitchEducators(SwitchEducatorData data)
        {
            if (!data.groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            _unitOfWork.GroupLessonDay.SwitchEducator(data.groupId.Value, data.from, data.to, data.educatorId);
            var switchEducatorMessage = _unitOfWork.EmailHelper.GetEmailOfTeacherAtDate(data.groupId.Value, data.from);
            var educationGrupName = _unitOfWork.EducationGroup.Get(x => x.Id == data.groupId).First().GroupName;
            if (educationGrupName != null)
            {
                await _emailSender.SendAsync(new EmailMessage
                { 
                    Subject = "Eğitmen Değiştirme | Nitelikli Bilişim",
                    Body = $"{educationGrupName} eğitimi için Başlangıç tarihi = {data.from.ToShortDateString()}" +
                           $" Bitiş Tarihi =  {data.to.Value.ToShortDateString()} yapılacak olan derslere atamanız yapılmıştır.",
                    Contacts = new string[] { switchEducatorMessage }
                });
            }
       
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = null
            });
        }
        [HttpPost, Route("admin/change-classrooms")]
        public async Task<IActionResult> ChangeClassrooms(ChangeClassroomData data)
        {
            if (!data.groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            _unitOfWork.GroupLessonDay.ChangeClassroom(data.groupId.Value, data.from, data.to, data.classroomId);
            var studentEmails = _unitOfWork.EmailHelper.GetEmailsOfStudentsByGroup(data.groupId.Value);
            await _emailSender.SendAsync(new EmailMessage
            {
                Body = "Sınıf değişmiştir",
                Contacts = studentEmails.ToArray()
            });
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
        [HttpPost, Route("admin/change-educator-salary")]
        public async Task<IActionResult> ChangeEducatorSalary(EducatorSalaryData data)
        {
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
    public class PostponeData
    {
        public Guid? groupId { get; set; }
        public DateTime from { get; set; }
        public DateTime? to { get; set; }
    }
    public class SwitchEducatorData : PostponeData
    {
        public string educatorId { get; set; }
    }
    public class ChangeClassroomData : PostponeData
    {
        public Guid classroomId { get; set; }
    }

    public class EducatorSalaryData : PostponeData
    {
        public decimal SalaryPerHour { get; set; }
        public byte HourCount { get; set; }
    }
}
