using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGroupRepository : BaseRepository<EducationGroup, Guid>
    {
        private readonly NbDataContext _context;
        public EducationGroupRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<EducationGroupListVm> GetListQueryable()
        {
            var groups = _context.EducationGroups.Include(x => x.Education).Include(x => x.Host)
                .Include(x => x.GroupStudents)
                .Select(x => new EducationGroupListVm
                         {
                             Id = x.Id,
                             EducationName = x.Education.Name,
                             GroupName = x.GroupName,
                             HostName = x.Host.HostName,
                             HostCity = EnumSupport.GetDescription(x.Host.City),
                             StartDate = x.StartDate
                         });

            return groups;
        }
       
        public bool Insert(EducationGroup entity, List<int> days,Guid? classRoomId,decimal educatorSalary)
        {
            using (var transation = _context.Database.BeginTransaction())
            {
                try
                {
                    var daysJson = SerializeDays(days);
                    if (daysJson == null)
                        return false;

                    _context.EducationGroups.Add(entity);
                    _context.SaveChanges();
                    _context.WeekDaysOfGroups.Add(new WeekDaysOfGroup
                    {
                        DaysJson = daysJson,
                        GroupId = entity.Id
                    });
                    _context.SaveChanges();
                    transation.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transation.Rollback();
                    return false;
                }

                // to transaction?
                var dates = CreateGroupLessonDays(
                    group: _context.EducationGroups.Include(x => x.Education).FirstOrDefault(x => x.Id == entity.Id),
                    daysInt: days,
                    unwantedDays: new List<DateTime>());
                entity.StartDate = dates[0];
                var groupLessonDays = new List<GroupLessonDay>();
                foreach (var date in dates)
                    groupLessonDays.Add(new GroupLessonDay
                    {
                        DateOfLesson = date,
                        GroupId = entity.Id,
                        ClassroomId =classRoomId,
                        HasAttendanceRecord = false,
                        IsImmuneToAutoChange = false,
                        EducatorId = entity.EducatorId,
                        EducatorSalary = educatorSalary
                    });

                _context.GroupLessonDays.AddRange(groupLessonDays);
                _context.SaveChanges();
                return true;
            }
        }

        public EducationGroup GetEducationGroupByTicketId(Guid ticketId)
        {
            var groupStudent = _context.Bridge_GroupStudents.FirstOrDefault(x => x.TicketId == ticketId);
            if (groupStudent == null)
            {
                throw new Exception($"{ticketId} Id ile ticket bulunamadı!");
            }
            return _context.EducationGroups.Include(x=>x.GroupLessonDays).FirstOrDefault(x => x.Id == groupStudent.Id);
        }

        public List<DateTime> CreateGroupLessonDays(EducationGroup group, List<int> daysInt, List<DateTime> unwantedDays)
        {
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
        public List<GroupVm> GetFirstAvailableGroups(Guid educationId)
        {
            var groups = _context.EducationGroups
                .Include(x => x.Host)
                .Where(x => x.StartDate.Date > DateTime.Now.Date && x.EducationId == educationId && x.IsGroupOpenForAssignment)
                .OrderBy(o => o.StartDate)
                .ToList();

            var model = new List<GroupVm>();
            var hostIds = new List<Guid>();
            foreach (var item in groups)
                if (!hostIds.Contains(item.HostId))
                {
                    hostIds.Add(item.HostId);
                    model.Add(new GroupVm
                    {
                        GroupId = item.Id,
                        StartDate = item.StartDate,
                        StartDateText = item.StartDate.ToLongDateString(),
                        Joined = _context.Bridge_GroupStudents.Count(x => x.Id == item.Id),
                        Quota = item.Quota,
                        Host = new HostVm
                        {
                            HostId = item.Host.Id,
                            Address = item.Host.Address,
                            City = EnumSupport.GetDescription(item.Host.City),
                            HostName = item.Host.HostName,
                            Latitude = item.Host.Latitude,
                            Longitude = item.Host.Longitude
                        }
                    });
                }

            return model;
        }
        private string SerializeDays(List<int> days)
        {
            if (days == null || days.Count == 0)
                return null;

            return JsonConvert.SerializeObject(days);
        }
        public GetEligibleAndAssignedStudentsVm GetEligibleAndAssignedStudents(Guid groupId)
        {
            var group = _context.EducationGroups.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
                return null;

            var eligibleTickets = _context.Tickets
                .Include(x => x.Owner)
                .ThenInclude(x => x.User)
                .Where(x => !x.IsUsed && x.EducationId == group.EducationId)
                .Select(x => new _Ticket
                {
                    TicketId = x.Id,
                    CustomerName = x.Owner.User.Name,
                    CustomerSurname = x.Owner.User.Surname
                })
                .ToList();

            var groupTickets = _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Include(x => x.Customer)
                .ThenInclude(x => x.User)
                .Select(x => new _Ticket
                {
                    TicketId = x.TicketId,
                    CustomerName = x.Customer.User.Name,
                    CustomerSurname = x.Customer.User.Surname
                })
                .ToList();

            return new GetEligibleAndAssignedStudentsVm
            {
                AssignedStudents = groupTickets,
                EligibleTickets = eligibleTickets
            };
        }
        public AssignStudentsVm GetAssignStudentsVm(Guid groupId)
        {
            var group = _context.EducationGroups
                .Include(x => x.Education)
                .Include(x => x.Host)
                .FirstOrDefault(x => x.Id == groupId);

            var educator = _context.Educators
                .Include(x => x.User)
                .Where(x => x.Id == group.EducatorId)
                .Select(x => new
                {
                    EducatorName = x.User.Name + " " + x.User.Surname
                })
                .FirstOrDefault();

            var data = new _Group
            {
                GroupId = group.Id,
                GroupName = group.GroupName,
                EducationName = group.Education.Name,
                EducatorName = educator.EducatorName,
                StartDate = group.StartDate,
                Location = $"{group.Host.HostName} ({EnumSupport.GetDescription(group.Host.City)})"
            };

            return new AssignStudentsVm
            {
                Group = data
            };
        }
        private List<int> MakeSureWeekDaysExists(Guid groupId, List<int> daysInt)
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
    }
}
