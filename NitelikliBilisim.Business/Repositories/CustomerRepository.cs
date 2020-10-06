using Microsoft.EntityFrameworkCore;
using MUsefullMethods;
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
    }
}
