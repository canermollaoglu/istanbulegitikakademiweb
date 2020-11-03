using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.student;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, string>
    {
        private readonly NbDataContext _context;
        public CustomerRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable<StudentListVm> GetCustomerListQueryable()
        {
            var data = (from student in _context.Customers
                        join user in _context.Users on student.Id equals user.Id
                        join educationInfo in _context.StudentEducationInfos on user.Id equals educationInfo.CustomerId into userEducation
                        from userEducationInfo in userEducation.DefaultIfEmpty()
                        join educationCategory in _context.EducationCategories on userEducationInfo.CategoryId equals educationCategory.Id into eCategory
                        from userEducationCategory in eCategory.DefaultIfEmpty()
                        select new StudentListVm
                        {
                            Id=student.Id,
                            CreatedDate = student.CreatedDate,
                            Name = user.Name,
                            Surname = user.Surname,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Job = student.Job,
                            IsNbuyStudent = student.IsNbuyStudent,
                            NbuyCategory = userEducationCategory.Name
                        }
                );
            return data;

        }

        public StudentDetailVm GetCustomerDetail(string studentId)
        {
            var student = _context.Customers.Include(x => x.User)
                .Include(x => x.Addresses).ThenInclude(x => x.City)
                .Include(x => x.Addresses).ThenInclude(x => x.State)
                .First(x => x.Id == studentId);

            #region NBUY Öğrencileri için NBUY bilgileri
            StudentNBUYEducationInfoVm educationInfo = null;
            var studentNBUYEducationDetails = _context.StudentEducationInfos
                    .Include(x => x.Category)
                    .FirstOrDefault(x => x.CustomerId == studentId);
            var educationDay = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentNBUYEducationDetails.Id && x.Date.Date < DateTime.Now.Date);
            if (studentNBUYEducationDetails!=null)
            {
                educationInfo= new StudentNBUYEducationInfoVm
                {
                    StartDate = studentNBUYEducationDetails.StartedAt,
                    CategoryName = studentNBUYEducationDetails.Category.Name,
                    EducationCenter = EnumHelpers.GetDescription(studentNBUYEducationDetails.EducationCenter),
                    EducationDay = educationDay!=null&&educationDay.Count()>0?educationDay.OrderBy(x => x.Date).LastOrDefault().Day:0
                };
            }
            #endregion
            return new StudentDetailVm
            {
                Id = student.Id,
                RegistrationDate = student.CreatedDate,
                UserName = student.User.UserName,
                Name = student.User.Name,
                Surname = student.User.Surname,
                Email = student.User.Email,
                Phone = student.User.PhoneNumber,
                LinkedInProfile = student.LinkedInProfileUrl,
                Website = student.WebSiteUrl,
                DateOfBirth = student.DateOfBirth,
                IsNBUYStudent = student.IsNbuyStudent,
                AvatarPath = student.User.AvatarPath,
                Addresses = student.Addresses,
                Job = student.Job,
                StudentNBUYEducationInfo = educationInfo
            };
        }

        public IQueryable<JoinedGroupVm> GetJoinedGroups(string studentId)
        {
            var groups = from gs in _context.Bridge_GroupStudents
                         join g in _context.EducationGroups.Include(x => x.Education).Include(x => x.Host)
                         on gs.Id equals g.Id
                         where gs.Id2 == studentId
                         select new JoinedGroupVm
                         {
                             GroupId = g.Id,
                             JoinedDate = gs.CreatedDate,
                             GroupStartDate = g.StartDate,
                             GroupName = g.GroupName,
                             HostName = g.Host.HostName,
                             EducationName = g.Education.Name,
                         };
            return groups;
        }

        public IQueryable<StudentUsedPromotionListVm> GetStudentUsedPromotions(string studentId)
        {
            var data = (from promotion in _context.EducationPromotionCodes
                        join usePromotion in _context.EducationPromotionItems on promotion.Id equals usePromotion.EducationPromotionCodeId
                        where usePromotion.UserId == studentId
                        select new StudentUsedPromotionListVm
                        {
                            Id = promotion.Id,
                            Name = promotion.Name,
                            Description = promotion.Description,
                            PromotionCode = promotion.PromotionCode,
                            PromotionType = promotion.PromotionType,
                            DiscountAmount = promotion.DiscountAmount,
                            UsedDate = usePromotion.CreatedDate,
                            InvoiceId = usePromotion.InvoiceId
                        });
            return data;
        }

        public IQueryable<StudentAbsenceListVm> GetStudentAbsences(string studentId)
        {
            var data = (from absence in _context.GroupAttendances
                        join egroup in _context.EducationGroups on absence.GroupId equals egroup.Id
                        join education in _context.Educations on egroup.EducationId equals education.Id
                        where absence.CustomerId == studentId
                        select new StudentAbsenceListVm
                        {
                            Id = absence.Id,
                            GroupName = egroup.GroupName,
                            EducationName = education.Name,
                            Reason = absence.Reason,
                            Date = absence.Date
                        });
            return data;
        }

        public IQueryable<StudentTicketsVm> GetStudentTickets(string studentId)
        {
            var data = (from ticket in _context.Tickets
                        join opdInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals opdInfo.Id
                        join education in _context.Educations on ticket.EducationId equals education.Id
                        join host in _context.EducationHosts on ticket.HostId equals host.Id
                        where ticket.OwnerId == studentId
                        select new StudentTicketsVm
                        {
                            TicketId = ticket.Id,
                            CreatedDate = ticket.CreatedDate,
                            EducationName = education.Name,
                            HostName = host.HostName,
                            IsUsed = ticket.IsUsed,
                            IsCancelled = opdInfo.IsCancelled ? "İptal Edildi" : "Devam Ediyor"
                        });

            return data;
        }

    }
}
