using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
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
                NewDates = newDates
            };
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
