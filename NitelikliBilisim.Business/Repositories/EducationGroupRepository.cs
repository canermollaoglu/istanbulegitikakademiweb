using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.group;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.areas.admin.customer;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_expense;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using NitelikliBilisim.Core.ViewModels.Cart;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationGroupRepository : BaseRepository<EducationGroup, Guid>
    {
        private readonly NbDataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;


        public EducationGroupRepository(NbDataContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _storageService = new StorageService();
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

        public GroupDetailVm GetDetailByGroupId(Guid groupId)
        {
            var expectedProfitRate = _configuration.GetValue<int>("ApplicationSettings:ExpectedProfitRate");
            var posCommissionRate = _configuration.GetValue<decimal>("ApplicationSettings:PosCommissionRate");
            var group = _context.EducationGroups
                .Include(x => x.Education)
                .Include(x => x.Host)
                .Include(x => x.GroupStudents)
                .Include(x => x.GroupExpenses)
                .Include(x => x.GroupLessonDays).First(x => x.Id == groupId);
            var educatorIds = _context.Bridge_EducationEducators.Where(x => x.Id == group.EducationId)
                .Select(x => x.Id2)
                .ToList();
            #region Satın alım ve iptal sayısı 
            var purchasesItems = (from onlinePaymentDetail in _context.OnlinePaymentDetailsInfos
                                  join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetail.Id equals invoiceDetail.Id
                                  where invoiceDetail.GroupId == groupId
                                  select onlinePaymentDetail).ToList();
            #endregion


            var weekDays = _context.WeekDaysOfGroups.First(x => x.GroupId == groupId);
            List<string> weekDaysNames = new List<string>();
            int[] days = JsonConvert.DeserializeObject<int[]>(weekDays.DaysJson);
            foreach (var item in days)
            {
                weekDaysNames.Add(EnumHelpers.GetDescription((Weekdays)item));
            }


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
            newTotal = newTotal + (newTotal * posCommissionRate / 100);
            newTotal = newTotal + (newTotal * 8 / 100);
            decimal educationPrice = group.NewPrice.GetValueOrDefault();
            var minimumStudent = CalculateMinimumStudentCount(newTotal - totalIncomes, educationPrice);
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
                EndDate = group.GroupLessonDays != null ? group.GroupLessonDays.OrderBy(x => x.DateOfLesson).Last().DateOfLesson.ToShortDateString() : group.StartDate.ToShortDateString(),
                ClassRoomName = classRoom != null ? classRoom.Name : "Sınıf bilgisi girilmemiş.",
                EducatorId = educator.Id,
                EducatorName = $"{educator.Name} {educator.Surname}",
                GroupExpenseTypes = _context.GroupExpenseTypes.ToList(),
                SelectEducators = educators,
                MinimumStudentCount = minimumStudent,
                OldPrice = group.OldPrice,
                NewPrice = group.NewPrice,
                ExpectedProfitRate = expectedProfitRate,
                WeekdayNames = weekDaysNames,
                PurchasesCount = purchasesItems.Count(),
                CancellationCount = purchasesItems.Count(x => x.IsCancelled)

            };
            return model;
        }

        public CalculateSalesPriceGetVm GetCalculateSalesPriceInformation(Guid groupId, int expectedProfitRate)
        {
            var group = _context.EducationGroups.First(x => x.Id == groupId);
            var quota = group.Quota;
            decimal expectedStudentCount = Math.Ceiling(Convert.ToDecimal(quota * 70.0 / 100.0));
            return new CalculateSalesPriceGetVm
            {
                ExpectedProfitRate = expectedProfitRate,
                TotalExpenses = GetGroupTotalExpenses(groupId),
                ExpectedStudentCount = (int)expectedStudentCount
            };
        }

        public Dictionary<Guid, string> GetAllGroupsDictionary()
        {
            return _context.EducationGroups.OrderByDescending(x => x.StartDate).ToDictionary(x => x.Id, x => x.GroupName + " - " + x.StartDate.ToShortDateString());
        }

        public List<GroupLessonDayGetListVm> GetLessonDaysByGroupId(Guid groupId)
        {
            var lessonDays = (from lessonDay in _context.GroupLessonDays
                              join eGroup in _context.EducationGroups.Include(x => x.Education) on lessonDay.GroupId equals eGroup.Id
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
                                  EducatorSalary = lessonDay.EducatorSalary.GetValueOrDefault() * eGroup.Education.HoursPerDay,
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
                    Id = x.Customer.Id,
                    TicketId = x.TicketId,
                    CustomerFullName = $"{x.Customer.User.Name} {x.Customer.User.Surname}",
                    CustomerId = x.Customer.User.Id,
                    Email = x.Customer.User.Email,
                    Job = EnumHelpers.GetDescription(x.Customer.Job),
                    PhoneNumber = x.Customer.User.PhoneNumber,
                    NonAttendance = groupAttendances.Count(c => c.CustomerId == x.Customer.Id),
                    IsNbuyStudent = x.Customer.IsNbuyStudent
                })
                .ToList();
            return assignedTickets;
        }

        public IQueryable<EducationGroupListVm> GetListQueryable()
        {
            return (from egroup in _context.EducationGroups
                    join education in _context.Educations on egroup.EducationId equals education.Id
                    join host in _context.EducationHosts on egroup.HostId equals host.Id
                    select new EducationGroupListVm
                    {
                        Id = egroup.Id,
                        CreatedDate = egroup.CreatedDate,
                        StartDate = egroup.StartDate,
                        NewPrice = egroup.NewPrice.HasValue ? egroup.NewPrice : 0,
                        EducationName = education.Name,
                        GroupName = egroup.GroupName,
                        HostName = host.HostName,
                        HostCity = (int)host.City
                    });
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
                catch
                {
                    transation.Rollback();
                    return Guid.Empty;
                }
            }
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
            decimal totalPosCommissionAmount = GetGroupTotalPosCommission(groupId);

            decimal totalEducatorExpense = (totalHours * educatorExpensesAverage) * (decimal)1.45;
            decimal totalExpense = groupExpenses + totalEducatorExpense;
            decimal kdv = totalExpense * 8 / 100;
            totalExpense = totalExpense + kdv;
            totalExpense += totalPosCommissionAmount;
            decimal profitRate = studentIncomes > 0 && totalExpense > 0 ? Math.Round(studentIncomes / totalExpense, 2) : 0;
            var culture = CultureInfo.CreateSpecificCulture("tr-TR");

            return new GroupExpenseAndIncomeVm
            {
                TotalEducationHours = totalHours,
                EducatorExpensesAverage = educatorExpensesAverage,
                GroupExpenses = groupExpenses.ToString("c", culture),
                EducatorExpenses = totalEducatorExpense.ToString("c", culture),
                TotalExpenses = totalExpense.ToString("c", culture),
                TotalStudentIncomes = studentIncomes.ToString("c", culture),
                GrandTotal = (studentIncomes - totalExpense).ToString("c", culture),
                ProfitRate = (profitRate * 100) - 100,
                KDV = kdv.ToString("c", culture),
                TotalPosCommissionAmount = totalPosCommissionAmount.ToString("c", culture)
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

        public EducationGroup GetByIdWithEducation(Guid groupId)
        {
            return _context.EducationGroups.Include(x => x.Education).First(x => x.Id == groupId);
        }

        /// <summary>
        /// İlgili eğitim için tarih ve kontenjan uygunluğunu kontrol ederek eğitim verilebileceği şehiler listesini döner.
        /// </summary>
        /// <param name="educationId"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetAvailableCities(Guid educationId)
        {
            var groups = _context.EducationGroups.Include(x => x.Host).Where(x => x.StartDate.Date > DateTime.Now.Date && x.EducationId == educationId && x.IsGroupOpenForAssignment);
            Dictionary<int, string> cities = new Dictionary<int, string>();
            foreach (var group in groups)
            {
                if (!cities.ContainsKey((int)group.Host.City))
                {
                    cities.Add((int)group.Host.City, EnumHelpers.GetDescription(group.Host.City));
                }
            }

            return cities;

        }
        public List<GroupVm> GetFirstAvailableGroups(Guid educationId, int? cityId = null)
        {
            var groups = _context.EducationGroups
                .Include(x => x.Host)
                .Where(x => x.StartDate.Date > DateTime.Now.Date && x.EducationId == educationId && x.IsGroupOpenForAssignment)
                .OrderBy(o => o.StartDate)
                .ToList();
            if (cityId.HasValue)
            {
                groups = groups.Where(x => x.Host.City == (HostCity)cityId).ToList();
            }
            var educators = _context.Educators.Include(x => x.User).ToList();

            var model = new List<GroupVm>();
            var hostIds = new List<Guid>();
            var storage = new StorageService();
            foreach (var item in groups)
            {
                var educator = educators.First(x => x.Id == item.EducatorId);
                var url = storage.BlobUrl + educator.User.AvatarPath;
                var culture = CultureInfo.CreateSpecificCulture("tr-TR");
                var discountRate = 100-(item.NewPrice*100/item.OldPrice);

                if (!hostIds.Contains(item.HostId))
                {
                    hostIds.Add(item.HostId);
                    model.Add(new GroupVm
                    {
                        GroupId = item.Id,
                        StartDate = item.StartDate,
                        StartDateText = item.StartDate.ToString("dd MMMM yyyy"),
                        Joined = _context.Bridge_GroupStudents.Count(x => x.Id == item.Id),
                        Quota = item.Quota,
                        Host = new HostVm
                        {
                            HostId = item.Host.Id,
                            Address = item.Host.Address,
                            City = EnumHelpers.GetDescription(item.Host.City),
                            HostName = item.Host.HostName,
                            Latitude = item.Host.Latitude,
                            Longitude = item.Host.Longitude,
                            LocationUrl = item.Host.GoogleMapUrl
                        },
                        Educator = new EducatorVm
                        {
                            EducatorId = educator.Id,
                            Name = educator.User.Name,
                            Surname = educator.User.Surname,
                            Title = educator.Title,
                            ProfilePhoto = url,
                            Point = GetEducatorPoint(item.EducatorId)
                        },
                        OldPrice = item.OldPrice.GetValueOrDefault(),
                        OldPriceText = item.OldPrice.GetValueOrDefault().ToString(culture),
                        NewPrice = item.NewPrice,
                        NewPriceText = item.NewPrice.GetValueOrDefault().ToString(culture),
                        DiscountRate= Math.Round(discountRate.GetValueOrDefault())
                    });
                }
            }
            return model;
        }

        private double GetEducatorPoint(string educatorId)
        {
            var data = (from eComment in _context.EducationComments
                        join education in _context.Educations on eComment.EducationId equals education.Id
                        join eGroup in _context.EducationGroups on education.Id equals eGroup.EducationId
                        where eGroup.EducatorId == educatorId
                        select (int)eComment.Points).ToList();
            return Math.Round(data.Average(),2);
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
                Location = $"{group.Host.HostName} ({EnumHelpers.GetDescription(group.Host.City)})"
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
            var educatorExpenses = (educatorExpensesAverage * (decimal)1.45) * totalHours;
            return groupExpenses + educatorExpenses;
        }
        /// <summary>
        /// İade edilen eğitim ücretleri toplamını döner
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private decimal GetGroupTotalRefundAmount(Guid groupId)
        {
            return (from onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos
                    join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetailInfo.Id equals invoiceDetail.Id
                    join educationGroup in _context.EducationGroups on invoiceDetail.GroupId equals educationGroup.Id
                    where educationGroup.Id == groupId && onlinePaymentDetailInfo.IsCancelled
                    select onlinePaymentDetailInfo).Sum(x => x.RefundPrice);
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
                    select onlinePaymentDetailInfo).Sum(x => x.PaidPrice);
        }
        /// <summary>
        /// Grup ödemelerindeki İyzico tarafından kesilen işlem ücreti ve komisyon tutarı toplamını döner.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private decimal GetGroupTotalPosCommission(Guid groupId)
        {
            return (from onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos
                    join invoiceDetail in _context.InvoiceDetails on onlinePaymentDetailInfo.Id equals invoiceDetail.Id
                    join educationGroup in _context.EducationGroups on invoiceDetail.GroupId equals educationGroup.Id
                    where educationGroup.Id == groupId && !onlinePaymentDetailInfo.IsCancelled
                    select onlinePaymentDetailInfo).Sum(x => x.CommisionRate + x.CommissionFee);
        }

        private int CalculateMinimumStudentCount(decimal difference, decimal educationPrice)
        {
            return educationPrice == 0 ? 0 : (int)Math.Ceiling((difference / educationPrice));
        }

        public List<CartItemVm> GetGroupCartItems(List<_CartItem> items)
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture("tr-TR");
            var groups = _context.EducationGroups.Include(x => x.Education).ThenInclude(x=>x.Category).ThenInclude(x=>x.BaseCategory).Where(x => items.Select(x => x.GroupId).Contains(x.Id)).OrderBy(x => x.Education.Name).ToList();
            //var educations = _unitOfWork.Education.Get(x => items.Select(x => x.EducationId).Contains(x.Id), x => x.OrderBy(o => o.Name));
            var model = new List<CartItemVm>();
            foreach (var group in groups)
            {
                var imagePath = _context.EducationMedias.First(x => x.EducationId == group.EducationId && x.MediaType == EducationMediaType.Square).FileUrl;
                var imageUrl = _storageService.BlobUrl + imagePath;
                model.Add(new CartItemVm
                {
                    EducationId = group.EducationId,
                    EducationName = group.Education.Name,
                    CategorySeoUrl = group.Education.Category.SeoUrl,
                    SeoUrl = group.Education.SeoUrl,
                    PreviewPhoto = imageUrl,
                    PriceNumeric = group.NewPrice.GetValueOrDefault(),
                    PriceText = group.NewPrice.GetValueOrDefault().ToString(cultureInfo),
                    OldPriceNumeric = group.OldPrice.GetValueOrDefault(),
                    OldPriceText = group.OldPrice.GetValueOrDefault().ToString()
                });
            }
            return model;
        }



        #endregion
    }
}
