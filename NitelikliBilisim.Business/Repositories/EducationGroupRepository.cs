﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.customer;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_expense;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days;
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

        public GroupDetailVm GetDetailByGroupId(Guid groupId)
        {
            var group = _context.EducationGroups
                .Include(x => x.Education)
                .Include(x => x.Host)
                .Include(x => x.GroupStudents)
                .Include(x => x.GroupExpenses)
                .Include(x => x.GroupLessonDays).First(x => x.Id == groupId);
            var educatorIds = _context.Bridge_EducationEducators.Where(x => x.Id == group.EducationId)
                .Select(x => x.Id2)
                .ToList();
            var educators =new Dictionary<string,string>();
            if (educatorIds!=null && educatorIds.Count>0)
            {
                educators = _context.Users.Where(x => educatorIds.Contains(x.Id)).ToDictionary(x=>x.Id,x=>$"{x.Name} {x.Surname}");
            }

            var selectAllClassRooms = _context.Classrooms.Where(x => x.HostId == group.HostId).ToDictionary(x => x.Id, x => x.Name);
            var firstDay = group.GroupLessonDays.FirstOrDefault();
            Classroom classRoom = null;
            if (firstDay != null)
            {
                classRoom = _context.Classrooms.FirstOrDefault(x => x.Id == firstDay.ClassroomId);
            }
            var educator = _context.Users.First(x => x.Id == group.EducatorId);
            var model = new GroupDetailVm
            {
                GroupId = group.Id,
                GroupName = group.GroupName,
                Host = group.Host,
                Quota = group.Quota,
                AssignedStudentsCount = group.GroupStudents.Count,
                Education = new _Education
                {
                    Id = group.Education.Id,
                    Name = group.Education.Name
                },
                StartDate = group.StartDate,
                ClassRoomName = classRoom != null ? classRoom.Name : "Sınıf bilgisi girilmemiş.",
                EducatorName = $"{educator.Name} {educator.Surname}",
                GroupExpenseTypes = _context.GroupExpenseTypes.ToList(),
                SelectClassRooms = selectAllClassRooms,
                SelectEducators = educators
            };
            return model;
        }

        public List<GroupLessonDayGetListVm> GetLessonDaysByGroupId(Guid groupId)
        {
            var lessonDays = (from lessonDay in _context.GroupLessonDays
                              join educator in _context.Users on lessonDay.EducatorId equals educator.Id into le
                              from educator in le.DefaultIfEmpty()
                              join classRoom in _context.Classrooms on lessonDay.ClassroomId equals classRoom.Id into lc
                              from classRoom in lc.DefaultIfEmpty()
                              where lessonDay.GroupId == groupId
                              select new GroupLessonDayGetListVm
                              {
                                  ClassRoomName = classRoom.Name,
                                  DateOfLesson = lessonDay.DateOfLesson,
                                  EducatorFullName = $"{educator.Name} {educator.Surname}",
                                  EducatorSalary = lessonDay.EducatorSalary.GetValueOrDefault(),
                                  Id = lessonDay.Id,
                                  HasAttendanceRecord = lessonDay.HasAttendanceRecord
                              }).OrderBy(x => x.DateOfLesson).ToList();
            return lessonDays;
        }

        public List<GroupExpenseListGetVm> GetExpensesByGroupId(Guid groupId)
        {
            var expenses = _context.GroupExpenses.Include(x => x.ExpenseType).Where(x => x.GroupId == groupId)
                .Select(x => new GroupExpenseListGetVm
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Price = x.Price,
                    Count = x.Count,
                    Description = x.Description,
                    ExpenseTypeName = x.ExpenseType.Name
                }).OrderByDescending(x => x.CreatedDate).ToList();
            return expenses;
        }

        public List<_Ticket> GetEligibleStudents(Guid groupId)
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
            return eligibleTickets;
        }
        public List<AssignedStudentVm> GetAssignedStudentsByGroupId(Guid groupId)
        {
            var groupAttendances = _context.GroupAttendances.Where(x => x.GroupId == groupId);
            var assignedTickets = _context.Bridge_GroupStudents
                .Where(x => x.Id == groupId)
                .Include(x => x.Customer)
                .ThenInclude(x => x.User)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new AssignedStudentVm
                {
                    TicketId = x.TicketId,
                    CustomerFullName = $"{x.Customer.User.Name} {x.Customer.User.Surname}",
                    CustomerId = x.Customer.User.Id,
                    Email = x.Customer.User.Email,
                    Job = x.Customer.Job,
                    PhoneNumber = x.Customer.User.PhoneNumber,
                    NonAttendance = groupAttendances.Count(c => c.CustomerId == x.Customer.Id),
                    IsNbuyStudent = x.Customer.IsNbuyStudent
                })
                .ToList();
            return assignedTickets;
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

        public bool Insert(EducationGroup entity, List<int> days, Guid? classRoomId, decimal educatorSalary)
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
                        ClassroomId = classRoomId,
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

        public CalculateExpectedProfitabilityReturnVm CalculateExpectedRateOfProfitability(CalculateExpectedProfitabilityVm data)
        {
            decimal totalExpenses = GetGroupTotalExpenses(data.GroupId);
            decimal totalIncomes = GetGroupTotalIncomes(data.GroupId);
            decimal newTotal = totalExpenses + (totalExpenses * data.ExpectedRateOfProfitability / 100);
            var group = _context.EducationGroups.Include(x => x.Education).First(x => x.Id == data.GroupId);
            decimal educationPrice = group.Education.NewPrice.GetValueOrDefault();


            return new CalculateExpectedProfitabilityReturnVm
            {
                ExpectedRateOfProfitability = data.ExpectedRateOfProfitability,
                PlannedAmount = newTotal,
                MinStudentCount = CalculateMinimumStudentCount(newTotal-(totalIncomes-totalExpenses),educationPrice)
            };

        }


        public GroupExpenseAndIncomeVm CalculateGroupExpenseAndIncome(Guid groupId)
        {
            decimal groupExpenses = _context.GroupExpenses.Where(x => x.GroupId == groupId).Sum(x => (x.Price * x.Count));
            decimal educatorExpenses = _context.GroupLessonDays.Where(x => x.GroupId == groupId).Sum(x => x.EducatorSalary.GetValueOrDefault());
            decimal studentIncomes = (from grupStudent in _context.Bridge_GroupStudents
                                      join ticket in _context.Tickets on grupStudent.TicketId equals ticket.Id
                                      join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                                      where grupStudent.Id == groupId
                                      select paymentDetailInfo).Sum(x => x.PaidPrice);


            return new GroupExpenseAndIncomeVm
            {
                GroupExpenses = groupExpenses,
                EducatorExpenses = educatorExpenses,
                TotalStudentIncomes = studentIncomes
            };
        }

        public EducationGroup GetEducationGroupByTicketId(Guid ticketId)
        {
            var groupStudent = _context.Bridge_GroupStudents.FirstOrDefault(x => x.TicketId == ticketId);
            if (groupStudent == null)
            {
                throw new Exception($"{ticketId} Id ile ticket bulunamadı!");
            }
            return _context.EducationGroups.Include(x => x.GroupLessonDays).FirstOrDefault(x => x.Id == groupStudent.Id);
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

        #region Helper Methods
        private decimal GetGroupTotalExpenses(Guid groupId)
        {
            decimal groupExpenses = _context.GroupExpenses.Where(x => x.GroupId == groupId).Sum(x => (x.Price * x.Count));
            decimal educatorExpenses = _context.GroupLessonDays.Where(x => x.GroupId == groupId).Sum(x => x.EducatorSalary.GetValueOrDefault());
            return groupExpenses + educatorExpenses;
        }
        private decimal GetGroupTotalIncomes(Guid groupId)
        {
            decimal studentIncomes = (from grupStudent in _context.Bridge_GroupStudents
                                      join ticket in _context.Tickets on grupStudent.TicketId equals ticket.Id
                                      join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                                      where grupStudent.Id == groupId
                                      select paymentDetailInfo).Sum(x => x.PaidPrice);
            return studentIncomes;
        }

        private int CalculateMinimumStudentCount(decimal difference, decimal educationPrice)
        {
            return (int)Math.Ceiling((difference / educationPrice));
        }
        #endregion
    }
}
