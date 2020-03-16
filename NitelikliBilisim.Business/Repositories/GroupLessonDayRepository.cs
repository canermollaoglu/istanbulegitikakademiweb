using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupLessonDayRepository : BaseRepository<GroupLessonDay, Guid>
    {
        private readonly NbDataContext _context;
        public GroupLessonDayRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
        public GroupLessonDayManagementVm CreateManagementVm(Guid groupId)
        {
            var group = _context.EducationGroups
                .Where(x => x.Id == groupId)
                .Include(x => x.Host)
                .First();
            var educatorsQuery = _context.Bridge_EducationEducators
                .Where(x => x.Id == group.EducationId)
                .Include(x => x.Educator)
                .ThenInclude(x => x.User)
                .ToList();
            var educators = educatorsQuery.Select(x => new _GroupLessonDayEducator
            {
                EducatorId = x.Educator.Id,
                FullName = $"{x.Educator.User.Name} {x.Educator.User.Surname}"
            }).ToList();
            var classroomsQuery = _context.Classrooms
                .Where(x => x.HostId == group.HostId)
                .ToList();
            var classrooms = classroomsQuery.Select(x => new _GroupLessonDayClassroom
            {
                ClassroomId = x.Id,
                Name = x.Name
            }).ToList();
            return new GroupLessonDayManagementVm
            {
                Group = group,
                Educators = educators,
                Classrooms = classrooms
            };
        }
        public List<GroupLessonDayVm> GetGroupLessonDays(Guid groupId)
        {
            var joined = (from groupLessonDay in _context.GroupLessonDays.Where(x => x.GroupId == groupId)
                          join classroom in _context.Classrooms
                          on groupLessonDay.ClassroomId equals classroom.Id
                          into groupLessonDayClassroom
                          from classroom in groupLessonDayClassroom.DefaultIfEmpty()
                          join educator in _context.Users
                          on groupLessonDay.EducatorId equals educator.Id
                          into groupLessonDayClassroomEducator
                          from educator in groupLessonDayClassroomEducator.DefaultIfEmpty()
                          select new
                          {
                              LessonDay = groupLessonDay,
                              Classroom = classroom,
                              Educator = educator
                          }).ToList();
            var hoursPerDay = _context.EducationGroups
                .Include(x => x.Education)
                .First(x => x.Id == groupId)
                .Education.HoursPerDay;
                
            var model = joined
                .Select(x => new GroupLessonDayVm
                {
                    Id = x.LessonDay.Id,
                    DateOfLesson = x.LessonDay.DateOfLesson,
                    DateOfLessonText = x.LessonDay.DateOfLesson.ToLongDateString(),
                    HasAttendanceRecord = x.LessonDay.HasAttendanceRecord,
                    Classroom = x.Classroom != null ? x.Classroom.Name : "SINIF YOK",
                    EducatorName = x.Educator != null ? $"{x.Educator.Name} {x.Educator.Surname}" : "EĞİTMEN YOK",
                    HoursPerDay = hoursPerDay
                })
                .OrderBy(o => o.DateOfLesson)
                .ToList();
            return model;
        }
        public List<DateTime> CreateGroupLessonDays(EducationGroup group, List<int> daysInt, List<DateTime> unwantedDays, bool isReset = false)
        {
            if (isReset)
                _context.GroupLessonDays.RemoveRange(_context.GroupLessonDays.Where(x => x.GroupId == group.Id));

            daysInt = MakeSureWeekDaysExists(group.Id, daysInt);
            var validDays = new List<DayOfWeek>();
            foreach (var dayInt in daysInt)
                validDays.Add((DayOfWeek)dayInt);

            var dayCount = group.Education.Days;
            var date = group.StartDate;
            var dates = new List<DateTime>();
            for (int i = 0; i < dayCount; i++)
            {
                if (!validDays.Contains(date.DayOfWeek) || unwantedDays.Contains(date))
                {
                    date = date.AddDays(1);
                    i--;
                    continue;
                }
                dates.Add(date);
                date = date.AddDays(1);
            }
            var groupLessonDays = new List<GroupLessonDay>();
            foreach (var item in dates)
                groupLessonDays.Add(new GroupLessonDay
                {
                    DateOfLesson = item,
                    GroupId = group.Id,
                    HasAttendanceRecord = false,
                    IsImmuneToAutoChange = false
                });

            if (isReset)
            {
                _context.GroupLessonDays.AddRange(groupLessonDays);
                _context.SaveChanges();
            }

            return dates;
        }
        public List<int> MakeSureWeekDaysExists(Guid groupId, List<int> daysInt)
        {
            if (daysInt == null || daysInt.Count == 0)
            {
                var weekDays = _context.WeekDaysOfGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (weekDays == null)
                {
                    daysInt = new List<int> { 6, 0 };
                    _context.WeekDaysOfGroups.Add(new WeekDaysOfGroup
                    {
                        GroupId = groupId,
                        DaysJson = JsonConvert.SerializeObject(daysInt)
                    });
                    _context.SaveChanges();
                }
                else
                    daysInt = JsonConvert.DeserializeObject<List<int>>(weekDays.DaysJson);
            }

            return daysInt;
        }
        public EliminatedAndNewDates DeterminePostponeDates(Guid groupId, DateTime from, DateTime? to)
        {
            if ((!to.HasValue) || (to < from))
                to = from;

            var groupLessonDays = _context.GroupLessonDays
                .Where(x => x.GroupId == groupId)
                .OrderBy(o => o.DateOfLesson)
                .ToList();
            var eliminatedDates = groupLessonDays
                .Where(x => (x.DateOfLesson.Date >= from && x.DateOfLesson <= to))
                .ToList();
            var weekDaysOfGroup = GetWeekDaysOfGroup(groupId, null);
            var lastDate = groupLessonDays
                .OrderByDescending(o => o.DateOfLesson)
                .First();
            var newDates = CreateNewDates(lastDate.DateOfLesson, eliminatedDates.Count, weekDaysOfGroup, null);
            return new EliminatedAndNewDates
            {
                EliminatedDates = eliminatedDates.Select(x => x.DateOfLesson).ToList(),
                NewDates = newDates,
                EliminatedDateTexts = eliminatedDates.Select(x => x.DateOfLesson.ToLongDateString()).ToList(),
                NewDateTexts = newDates.Select(x => x.ToLongDateString()).ToList()
            };
        }
        public List<DateTime> DetermineToBeChangedDates(Guid groupId, DateTime from, DateTime? to)
        {
            if ((!to.HasValue) || (to < from))
                to = from;

            var dates = _context.GroupLessonDays
                .Where(x => x.GroupId == groupId && (x.DateOfLesson.Date >= from && x.DateOfLesson <= to))
                .OrderBy(o => o.DateOfLesson)
                .Select(x => x.DateOfLesson)
                .ToList();

            return dates;
        }
        public List<string> DetermineToBeChangedDatesAsText(Guid groupId, DateTime from, DateTime? to)
        {
            return DetermineToBeChangedDates(groupId, from, to).Select(x => x.ToLongDateString()).ToList();
        }
        public void SwitchEducator(Guid groupId, DateTime from, DateTime? to, string educatorId)
        {
            var dates = DetermineToBeChangedDates(groupId, from, to);
            var lessonDays = _context.GroupLessonDays
                .Where(x => dates.Contains(x.DateOfLesson))
                .ToList();
            foreach (var item in lessonDays)
                item.EducatorId = educatorId;

            var group = _context.EducationGroups.First(x => x.Id == groupId);
            var lastEducatorId = lessonDays.Last().EducatorId;
            if (group.EducatorId != lastEducatorId)
                group.EducatorId = lastEducatorId;
            _context.SaveChanges();
        }
        public void ChangeClassroom(Guid groupId, DateTime from, DateTime? to, Guid classroomId)
        {
            var dates = DetermineToBeChangedDates(groupId, from, to);
            var lessonDays = _context.GroupLessonDays
                .Where(x => dates.Contains(x.DateOfLesson))
                .ToList();
            foreach (var item in lessonDays)
                item.ClassroomId = classroomId;
            _context.SaveChanges();
        }

        public void PostponeLessons(Guid groupId, DateTime from, DateTime? to)
        {
            var data = DeterminePostponeDates(groupId, from, to);
            var lessonDays = _context.GroupLessonDays
                .Where(x => data.EliminatedDates.Contains(x.DateOfLesson))
                .ToList();
            for (int i = 0; i < data.EliminatedDates.Count; i++)
            {
                var lessonDay = lessonDays.First(x => x.DateOfLesson == data.EliminatedDates[i]);
                lessonDay.DateOfLesson = data.NewDates[i];
            }
            _context.SaveChanges();
        }
        public List<DateTime> CreateNewDates(DateTime lastDate, int newDateCount, DayOfWeek[] eligibleDays, List<DateTime> unwantedDates)
        {
            if (unwantedDates == null)
                unwantedDates = new List<DateTime>();

            var dates = new List<DateTime>();
            for (int i = 0; i < newDateCount; i++)
            {
                lastDate = lastDate.AddDays(1);
                if (!eligibleDays.Contains(lastDate.DayOfWeek) || unwantedDates.Any(x => x.Date == lastDate.Date))
                {
                    i--;
                    continue;
                }
                dates.Add(lastDate);
            }
            return dates;
        }
        private DayOfWeek[] GetWeekDaysOfGroup(Guid groupId, List<int> daysInt)
        {
            daysInt = MakeSureWeekDaysExists(groupId, daysInt);
            var days = new List<DayOfWeek>();
            foreach (var item in daysInt)
                days.Add((DayOfWeek)item);

            return days.ToArray();
        }
    }
}
