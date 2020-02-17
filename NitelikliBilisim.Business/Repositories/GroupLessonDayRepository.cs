using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
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

            return dates;
        }
        private List<int> MakeSureWeekDaysExists(Guid groupId, List<int> daysInt)
        {
            if (daysInt == null || daysInt.Count == 0)
            {
                var weekDays = _context.WeekDaysOfGroups.FirstOrDefault(x => x.GroupId == groupId);
                if (weekDays == null)
                    daysInt = new List<int> { 6, 0 };
                //TODO: weekDays null gelirse hata verecektir!!!!!!!!!!!!!!!!!
                daysInt = JsonConvert.DeserializeObject<List<int>>(weekDays.DaysJson);
            }

            return daysInt;
        }
    }
}
