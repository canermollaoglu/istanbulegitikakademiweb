using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationGroupController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public EducationGroupController(UnitOfWork unitOfWork, IConfiguration configuration,IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _configuration = configuration;
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
            ViewData["groupId"] = groupId;
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationGrupDetail");
            return View();
        }



        [Route("admin/get-calculate-sales-price-model/{groupId?}")]
        public IActionResult GetCalculateSalesPriceInformation(Guid groupId)
        {
            try
            {
                var expectedProfitRate = _configuration.GetValue<int>("ApplicationSettings:ExpectedProfitRate");
                var data = _unitOfWork.EducationGroup.GetCalculateSalesPriceInformation(groupId, expectedProfitRate);

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata : {ex.Message}" }
                });
            }


        }


        [Route("admin/get-group-detail/{groupId?}")]
        public IActionResult GetGroupDetail(Guid groupId)
        {
            try
            {
                
                var groupDetail = _unitOfWork.EducationGroup.GetDetailByGroupId(groupId);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = groupDetail
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata : {ex.Message}" }
                });
            }
        }

        [Route("admin/get-group-detail-and-calculations-table/{gId?}")]
        public IActionResult GetGroupDetailsAndCalculationsTable(Guid gId)
        {
            try
            {
                var groupDetail = _unitOfWork.EducationGroup.GetDetailByGroupId(gId);
                var calculationsTable = _unitOfWork.EducationGroup.CalculateGroupExpenseAndIncome(gId);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = new { groupDetail, calculationsTable }
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata : {ex.Message}" }
                });
            }

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

        [Route("admin/grup-adi-dogrula")]
        public IActionResult CheckGroupName(string groupName)
        {
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = IsUniqueGroupName(groupName)
            });

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

        

       



        [Route("admin/get-education-days-info/{educationId?}")]
        public IActionResult GetEducationDaysInfo(Guid educationId)
        {
            var model = _unitOfWork.Education.GetById(educationId);
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

            var model = _unitOfWork.ClassRoom.GetClassRoomsByHostId(hostId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
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

        [HttpPost, Route("admin/add-group")]
        public async Task<IActionResult> Add(AddPostVm data)
        {
            if (!ModelState.IsValid || data.LessonDays == null || data.LessonDays.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            try
            {
                var group = new EducationGroup
                {
                    IsGroupOpenForAssignment = true,
                    GroupName = data.Name,
                    EducationId = data.EducationId.Value,
                    EducatorId = data.EducatorId,
                    HostId = data.HostId.Value,
                    StartDate = data.StartDate.Value,
                    Quota = data.Quota.Value
                };

                var groupId = _unitOfWork.EducationGroup.Insert(group, data.LessonDays, data.ClassRoomId, data.EducatorPrice);
                var emails = _unitOfWork.EmailHelper.GetAdminEmails();
                await _emailSender.SendAsync(new Core.ComplexTypes.EmailMessage
                {
                    Body = "Grup açılmıştır",
                    Contacts = emails.ToArray()
                });

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = groupId
                });


            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata : " + ex.Message }
                });
            }

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

        [Route("admin/calculate-group-expense-and-income/{groupId}")]
        public IActionResult CalculateGroupExpensesAndIncome(Guid groupId)
        {
            try
            {
                var model = _unitOfWork.EducationGroup.CalculateGroupExpenseAndIncome(groupId);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = model
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata : {ex.Message}" }
                });
            }

        }

        public IActionResult ChangeGeneralInformation(UpdateGroupGeneralInformationVm data)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            try
            {
                var group = _unitOfWork.EducationGroup.GetById(data.GroupId);
                group.GroupName = data.GroupName;
                group.NewPrice = data.NewPrice;
                group.OldPrice = data.OldPrice;
                _unitOfWork.EducationGroup.Update(group);
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
        public IActionResult ChangeNewPrice(UpdateGroupNewPriceVm data)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            try
            {
                var group = _unitOfWork.EducationGroup.GetById(data.GroupId);
                group.NewPrice = data.NewPrice.GetValueOrDefault();
                _unitOfWork.EducationGroup.Update(group);
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

        [HttpPost]
        public async Task<IActionResult> PostponementOfGroup(PostponementGroupVm data)
        {
            try
            {
                var group = _unitOfWork.EducationGroup.GetById(data.GroupId);
                var education = _unitOfWork.Education.GetById(group.EducationId);
                if (group.StartDate.Date <= DateTime.Now.Date)
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { "Grup eğitime başladığı için erteleme yapılamaz. Lütfen yalnızca ilgili günleri güncelleyin!." }
                    });

                _unitOfWork.GroupLessonDay.PostponeLessons(data.GroupId, data.StartDate);
                var emails = _unitOfWork.EmailHelper.GetEmailsOfStudentsByGroup(data.GroupId);
                await _emailSender.SendAsync(new EmailMessage
                {
                    Subject="Nitelikli Bilişim Eğitim Tarihi Değişikliği",
                    Body =$"Katıldığınız {education.Name} eğitimi {data.StartDate} tarihinden itibaren devam edecek şekilde güncellenmiştir.",
                    Contacts = emails.ToArray()
                });
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
                    errors = new List<string> { $"Hata : {ex.Message}" }
                }); ;
            }
        }


        [Route("admin/get-host-city-enums/")]
        public IActionResult GetHostCityEnums()
        {
            try
            {
                EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<HostCity>().Select(x =>
            new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = retVal
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { ex.Message }
                });
            }

        }

        /// <summary>
        /// Grup Adı daha önce kaydolmamış ise true döner
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool IsUniqueGroupName(string groupName)
        {
            var code = _unitOfWork.EducationGroup.Get(x => x.GroupName == groupName);
            if (code.Count() == 0)
                return true;
            else
                return false;
        }

        #region Select doldurmak için kullanılan Actionlar
        
        [Route("admin/get-assigned-class-rooms-for-group-detail/{gId?}")]
        public IActionResult GetClassRoomsOfGroup(Guid? gId)
        {
            if (!gId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
            var group = _unitOfWork.EducationGroup.GetById(gId.Value);
            var model = _unitOfWork.ClassRoom.GetClassRoomsByHostId(group.HostId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
        [Route("admin/get-assigned-educators-for-group-detail/{gId?}")]
        public IActionResult GetEducatorsOfGroup(Guid? gId)
        {
            if (!gId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
            var group = _unitOfWork.EducationGroup.GetById(gId.Value);
            var model = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(group.EducationId);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("admin/group-detail-fill-all-select/{gId?}")]
        /// <summary>
        /// Group Detail sayfası içerisinde bulunan tüm selectleri doldurur.
        /// </summary>
        /// <param name="gId"></param>
        /// <returns></returns>
        public IActionResult FillAllSelect(Guid gId)
        {
            var group = _unitOfWork.EducationGroup.GetById(gId);
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(group.EducationId);
            var classRooms = _unitOfWork.ClassRoom.GetClassRoomsByHostId(group.HostId);
            var expenseTypes = _unitOfWork.GroupExpenseType.Get().ToList();

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = new { educators, classRooms, expenseTypes }
            }) ;
        }

        #endregion



        #region Aktif olarak kullanılmayan Actionlar
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
        #endregion
    }
}