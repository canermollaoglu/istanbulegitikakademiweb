﻿using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.student;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
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


        public IQueryable<Customer> GetCustomerListQueryable()
        {
            return Context.Customers.Include(x => x.User);

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
                    EducationCenter = EnumSupport.GetDescription(studentNBUYEducationDetails.EducationCenter),
                    EducationDay = educationDay!=null?educationDay.OrderBy(x => x.Date).LastOrDefault().Day:0
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
