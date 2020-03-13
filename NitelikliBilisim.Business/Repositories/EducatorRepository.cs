﻿using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Core.ViewModels.areas.educator_area.group;
using NitelikliBilisim.Core.ViewModels.areas.educator_area.payment;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorRepository : BaseRepository<Educator, string>
    {
        private readonly NbDataContext _context;
        public EducatorRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<_Educator> GetEducators()
        {
            var model = Context.Users
                .Join(Context.Educators, l => l.Id, r => r.Id, (x, y) => new _Educator
                {
                    Id = x.Id,
                    FullName = x.Name + " " + x.Surname,
                    Title = y.Title,
                    Phone = x.PhoneNumber,
                    Email = x.Email,
                    SocialMediaCount = Context.EducatorSocialMedias.Count(z => z.EducatorId == x.Id)
                }).ToList();
            return model;
        }
        public override int Delete(string id, bool isSaveLater = false)
        {
            var educationSocialMedia = _context.EducatorSocialMedias.Where(x => x.EducatorId == id).ToList();
            _context.EducatorSocialMedias.RemoveRange(educationSocialMedia);
            _context.SaveChanges();
            return base.Delete(id, isSaveLater);
        }
        public MyGroupsGetVm GetMyGroupsVm(string userId)
        {
            var groups = _context.EducationGroups
                .Include(x => x.Education)
                .Where(x => x.EducatorId == userId)
                .ToList();
            return new MyGroupsGetVm
            {
                Groups = groups.Select(x => new _Group
                {
                    GroupId = x.Id,
                    GroupName = x.GroupName,
                    EducationName = x.Education.Name
                }).ToList()
            };
        }
        public GroupDetailsVm GetGroupDetailsVm(Guid groupId, string userId)
        {
            var group = _context.EducationGroups
                .Include(x => x.Education)
                .FirstOrDefault(x => x.Id == groupId);
            if (group == null)
                return null;
            var lessonDaysQuery = _context.GroupLessonDays
                .Where(x => x.GroupId == groupId && x.EducatorId == userId)
                .OrderBy(o => o.DateOfLesson)
                .ToList();
            if (!lessonDaysQuery.Select(x => x.EducatorId).Contains(userId))
                return null;

            var lessonDays = lessonDaysQuery.Select(x => new _EducationDay
            {
                Id = x.Id,
                Date = x.DateOfLesson,
                DateText = x.DateOfLesson.ToLongDateString(),
                HasAttendanceRecord = x.HasAttendanceRecord
            }).ToList();
            var students = _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Join(_context.Users, l => l.Id2, r => r.Id, (x, y) => new
                {
                    Student = y
                }).ToList()
                .Select(x => new _GroupStudent
                {
                    FullName = $"{x.Student.Name} {x.Student.Surname}"
                }).ToList();
            return new GroupDetailsVm
            {
                Group = new _Group
                {
                    GroupId = group.Id,
                    EducationName = group.Education.Name,
                    GroupName = group.GroupName
                },
                Days = lessonDays,
                GroupStudents = students
            };
        }
        public bool IsValidEducatorForGroup(Guid groupId, string educatorId)
        {
            var days = _context.GroupLessonDays.Where(x => x.GroupId == groupId)
                .Select(x => x.EducatorId);
            return days.Contains(educatorId);
        }
        public MyPaymentsVm GetMySalaries(string userId)
        {
            var salaries = _context.EducatorSalaries
                .Where(x => x.EducatorId == userId)
                .ToList();

            var grouped = salaries.GroupBy(x => x.EarnedForGroup)
                .Select(x => new
                {
                    GroupId = x.Key,
                    Sum = x.Sum(x => x.Paid)
                });

            var groups = _context.EducationGroups
                .Where(x => grouped.Select(y => y.GroupId).Contains(x.Id))
                .ToList();

            var payments = grouped.Select(x => new _PaidByGroup
            {
                GroupName = groups.First(y => y.Id == x.GroupId).GroupName,
                Paid = x.Sum.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
            }).ToList();

            return new MyPaymentsVm
            {
                Payments = payments
            };
        }
    }
}
