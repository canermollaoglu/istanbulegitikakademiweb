using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.groups;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.customer;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_expense;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using NitelikliBilisim.Core.ViewModels.Cart;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
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


        public List<GroupBasedSalesReportStudentsVm> GetGroupBasedSalesReportStudentsToList(Guid groupId)
        {
            return (from gs in _context.Bridge_GroupStudents
                    join student in _context.Users on gs.Id2 equals student.Id
                    join ticket in _context.Tickets on gs.TicketId equals ticket.Id
                    join paymentDetailInfo in _context.OnlinePaymentDetailsInfos on ticket.InvoiceDetailsId equals paymentDetailInfo.Id
                    where !paymentDetailInfo.IsCancelled
                    orderby gs.CreatedDate
                    where gs.Id == groupId
                    select new GroupBasedSalesReportStudentsVm
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Surname = student.Surname,
                        PaymentDate = paymentDetailInfo.CreatedDate,
                        PaidPrice = paymentDetailInfo.PaidPrice,
                        CommissionFee = paymentDetailInfo.CommissionFee,
                        CommissionRate = paymentDetailInfo.CommisionRate,
                        MerchantPayout = paymentDetailInfo.MerchantPayout
                    }).ToList();
        }

        public GroupDetailVm GetDetailByGroupId(Guid groupId, int expectedProfitRate)
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
            var educators = new Dictionary<string, string>();
            if (educatorIds != null && educatorIds.Count > 0)
            {
                educators = _context.Users.Where(x => educatorIds.Contains(x.Id)).ToDictionary(x => x.Id, x => $"{x.Name} {x.Surname}");
            }

            var firstDay = group.GroupLessonDays.FirstOrDefault();
            Classroom classRoom = null;
            if (firstDay != null)
            {
                classRoom = _context.Classrooms.FirstOrDefault(x => x.Id == firstDay.ClassroomId);
            }
            var educator = _context.Users.First(x => x.Id == group.EducatorId);

            #region Belirlenen kar için minimum kayıt olması gereken öğrenci sayısı

            decimal totalExpenses = GetGroupTotalExpenses(groupId);
            decimal totalIncomes = GetGroupTotalIncomes(groupId);
            decimal newTotal = totalExpenses + (totalExpenses * expectedProfitRate / 100);
            decimal educationPrice = group.NewPrice.GetValueOrDefault();
            var minimumStudent = CalculateMinimumStudentCount(newTotal - totalIncomes, educationPrice);
            var expectedSellingPrice = newTotal / group.Quota;
            #endregion


            var model = new GroupDetailVm
            {
                GroupId = group.Id,
                GroupName = group.GroupName,
                Host = group.Host,
                Quota = group.Quota,
                EducationDays = group.Education.Days.ToString(),
                EducationHoursPerDay = group.Education.HoursPerDay.ToString(),
                AssignedStudentsCount = group.GroupStudents.Count,
                Education = new _Education
                {
                    Id = group.Education.Id,
                    Name = group.Education.Name
                },
                StartDate = group.StartDate.ToShortDateString(),
                EndDate = group.GroupLessonDays!=null? group.GroupLessonDays.OrderBy(x=>x.DateOfLesson).Last().DateOfLesson.ToShortDateString():group.StartDate.ToShortDateString(),
                ClassRoomName = classRoom != null ? classRoom.Name : "Sınıf bilgisi girilmemiş.",
                EducatorName = $"{educator.Name} {educator.Surname}",
                GroupExpenseTypes = _context.GroupExpenseTypes.ToList(),
                SelectEducators = educators,
                MinimumStudentCount = minimumStudent,
                OldPrice = group.OldPrice,
                NewPrice = group.NewPrice,
                ExpectedProfitRate = expectedProfitRate,
                ExpectedSellingPrice = expectedSellingPrice>0? Math.Ceiling(expectedSellingPrice):0
            };
            return model;
        }
        public GroupGeneralInformationVm GetGroupGeneralInformation(Guid groupId)
        {
            var group = _context.EducationGroups
                .Include(x => x.GroupStudents)
                .Include(x => x.Education)
                .Include(x => x.Host).First(x => x.Id == groupId);
            string classRoomName = string.Empty;
            var firstLessonDay = _context.GroupLessonDays.FirstOrDefault(x => x.GroupId == groupId);
            if (firstLessonDay != null && firstLessonDay.ClassroomId != null)
            {
                classRoomName = _context.Classrooms.First(x => x.Id == firstLessonDay.ClassroomId).Name;
            }
            var educator = _context.Users.First(x => x.Id == group.EducatorId);

            var model = new GroupGeneralInformationVm
            {
                GroupId = group.Id,
                GroupName = group.GroupName,
                Quota = group.Quota,
                StartDate = group.StartDate.ToShortDateString(),
                EducationHost = group.Host.HostName,
                Classroom = classRoomName,
                EducationName = group.Education.Name,
                EducatorName = $"{educator.Name} {educator.Surname}",
                AssignedStudentsCount = group.GroupStudents.Count,
                EducationDays = group.Education.Days.ToString(),
                EducationHoursPerDay = group.Education.HoursPerDay.ToString()
            };
            return model;
        }
        public Dictionary<Guid, string> GetAllGroupsDictionary()
        {
            return _context.EducationGroups.OrderByDescending(x => x.StartDate).ToDictionary(x => x.Id, x => x.GroupName + " - " + x.StartDate.ToShortDateString());
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
                    NewPrice = x.NewPrice.HasValue ? x.NewPrice : 0,
                    EducationName = x.Education.Name,
                    GroupName = x.GroupName,
                    HostName = x.Host.HostName,
                    HostCity = EnumSupport.GetDescription(x.Host.City),
                    StartDate = x.StartDate
                });

            return groups;
        }

        public Guid Insert(EducationGroup entity, List<int> days, Guid? classRoomId, decimal educatorSalary)
        {
            using (var transation = _context.Database.BeginTransaction())
            {
                try
                {
                    var daysJson = SerializeDays(days);
                    if (daysJson == null)
                        throw new Exception("Eğitim günlerini giriniz!");

                    _context.EducationGroups.Add(entity);
                    _context.SaveChanges();
                    _context.WeekDaysOfGroups.Add(new WeekDaysOfGroup
                    {
                        DaysJson = daysJson,
                        GroupId = entity.Id
                    });
                    _context.SaveChanges();
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
                    transation.Commit();

                    return entity.Id;
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    throw ex;
                }
            }
        }

        public CalculateExpectedProfitabilityReturnVm CalculateExpectedRateOfProfitability(CalculateExpectedProfitabilityVm data)
        {
            decimal totalExpenses = GetGroupTotalExpenses(data.GroupId);
            decimal totalIncomes = GetGroupTotalIncomes(data.GroupId);
            decimal newTotal = totalExpenses + (totalExpenses * data.ExpectedRateOfProfitability / 100);
            var group = _context.EducationGroups.Include(x => x.Education).First(x => x.Id == data.GroupId);
            decimal educationPrice = group.NewPrice.GetValueOrDefault();


            return new CalculateExpectedProfitabilityReturnVm
            {
                ExpectedRateOfProfitability = data.ExpectedRateOfProfitability,
                PlannedAmount = newTotal,
                MinStudentCount = CalculateMinimumStudentCount(newTotal - totalIncomes, educationPrice)
            };

        }

        public GroupExpenseAndIncomeVm CalculateGroupExpenseAndIncome(Guid groupId)
        {
            var group = _context.EducationGroups.Include(x => x.GroupLessonDays).Include(x => x.Education).Include(x => x.GroupExpenses).First(x => x.Id == groupId);
            var education = group.Education;
            var totalHours = education.HoursPerDay * education.Days;
            var lessonDays = group.GroupLessonDays.ToList();

            decimal groupExpenses = group.GroupExpenses.Sum(x => (x.Price * x.Count));
            decimal educatorExpensesAverage = lessonDays != null && lessonDays.Count > 0 ? lessonDays.Average(x => x.EducatorSalary.GetValueOrDefault()) : 0;
            decimal studentIncomes = GetGroupTotalIncomes(groupId);

            decimal totalEducatorExpense = (totalHours * educatorExpensesAverage) * (decimal)1.45;
            decimal profitRate = studentIncomes > 0 && (groupExpenses + totalEducatorExpense) > 0 ? Math.Round(studentIncomes / (groupExpenses + totalEducatorExpense), 2) : 0;


            return new GroupExpenseAndIncomeVm
            {
                TotalEducationHours = totalHours,
                EducatorExpensesAverage = educatorExpensesAverage,
                GroupExpenses = groupExpenses,
                EducatorExpenses = totalEducatorExpense,
                TotalExpenses = groupExpenses+totalEducatorExpense,
                TotalStudentIncomes = studentIncomes,
                ProfitRate = (profitRate * 100) - 100
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
                        },
                        OldPrice = item.OldPrice,
                        NewPrice = item.NewPrice
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
        /// <summary>
        /// Grup giderleri ve eğitmen ücreti gideri toplamını döner
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private decimal GetGroupTotalExpenses(Guid groupId)
        {
            var group = _context.EducationGroups.Include(x => x.Education).Include(x => x.GroupLessonDays).Include(x => x.GroupExpenses).First(x => x.Id == groupId);

            //Grup Giderleri
            decimal groupExpenses = group.GroupExpenses.Sum(x => (x.Price * x.Count));
            var totalHours = group.Education.HoursPerDay * group.Education.Days;
            var lessonDays = group.GroupLessonDays.ToList();
            decimal educatorExpensesAverage = lessonDays != null && lessonDays.Count > 0 ? lessonDays.Average(x => x.EducatorSalary.GetValueOrDefault()) : 0;
            var educatorExpenses = (educatorExpensesAverage* (decimal)1.45) * totalHours;
            return groupExpenses + educatorExpenses;
        }

        /// <summary>
        /// Grup gelirleri yalnızca öğrenci ödemeleri olduğu için öğrenci ödemeleri toplamını döner.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private decimal GetGroupTotalIncomes(Guid groupId)
        {
            return (from onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos
                    join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetailInfo.Id equals invoiceDetail.Id
                    join educationGroup in _context.EducationGroups on invoiceDetail.GroupId equals educationGroup.Id
                    where educationGroup.Id == groupId && !onlinePaymentDetailInfo.IsCancelled
                    select onlinePaymentDetailInfo).Sum(x => x.MerchantPayout);
        }

        private int CalculateMinimumStudentCount(decimal difference, decimal educationPrice)
        {
            return educationPrice == 0 ? 0 : (int)Math.Ceiling((difference / educationPrice));
        }

        public List<CartItemVm> GetGroupCartItems(List<_CartItem> items)
        {
            var groups = _context.EducationGroups.Include(x => x.Education).Where(x => items.Select(x => x.GroupId).Contains(x.Id)).OrderBy(x => x.Education.Name);
            //var educations = _unitOfWork.Education.Get(x => items.Select(x => x.EducationId).Contains(x.Id), x => x.OrderBy(o => o.Name));
            var model = groups.Select(x => new CartItemVm
            {
                EducationId = x.Education.Id,
                EducationName = x.Education.Name,
                PreviewPhoto = _context.EducationMedias.First(y => y.EducationId == x.Id && y.MediaType == EducationMediaType.PreviewPhoto).FileUrl,
                PriceNumeric = x.NewPrice.GetValueOrDefault(0),
                PriceText = x.NewPrice.GetValueOrDefault(0).ToString()
            }).ToList();
            return model;
        }
        #endregion
    }
}
