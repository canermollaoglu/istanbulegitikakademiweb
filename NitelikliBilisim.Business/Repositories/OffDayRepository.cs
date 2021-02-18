using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.helper;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class OffDayRepository : BaseRepository<OffDay, int>
    {
        private readonly NbDataContext _context;
        public OffDayRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public int Insert(OffDay entity)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int id = base.Insert(entity);
                    var customers = _context.Customers
                        .Include(x => x.StudentEducationInfos)
                        .ThenInclude(x => x.EducationDays)
                        .Where(x => x.StudentEducationInfos != null && x.StudentEducationInfos.Count > 0).ToList();
                    var reCalculateCustomers = customers.Where(x => x.StudentEducationInfos[0].EducationDays.Any(y => y.Date.Date == entity.Date.Date)).ToList();
                    var reCalculateEducationGroups = _context.EducationGroups.Include(x => x.Education).Include(x => x.GroupLessonDays).Where(x => x.GroupLessonDays.Any(y => y.DateOfLesson.Date == entity.Date.Date)).ToList();
                    ReCalculateGroupLessonDays(reCalculateEducationGroups);
                    ReCalculateEducationDays(reCalculateCustomers);
                    transaction.Commit();
                    return id;


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

            }
        }

        public void Delete(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    OffDay relatedOffDay = _context.OffDays.First(x => x.Id == id);
                    int entityId = base.Delete(id);
                    var customers = _context.Customers
                        .Include(x => x.StudentEducationInfos)
                        .ThenInclude(x => x.EducationDays)
                        .Where(x => x.StudentEducationInfos != null && x.StudentEducationInfos.Count > 0).ToList();
                    var reCalculateCustomers = customers.Where(x => x.StudentEducationInfos[0].EducationDays.Any(y => y.Date.Year == relatedOffDay.Date.Year && y.Date.Month == relatedOffDay.Month)).ToList();
                    var reCalculateEducationGroups = _context.EducationGroups.Include(x => x.Education).Include(x => x.GroupLessonDays).Where(x => x.GroupLessonDays.Any(y => y.DateOfLesson.Year == relatedOffDay.Date.Year && y.DateOfLesson.Month == relatedOffDay.Month)).ToList();
                    ReCalculateGroupLessonDays(reCalculateEducationGroups);
                    ReCalculateEducationDays(reCalculateCustomers);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

            }
        }

        public int Update(OffDay entity)
        {
            var oldDay = _context.OffDays.AsNoTracking().First(x => x.Id == entity.Id).Date;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int id = base.Update(entity);
                    var customers = _context.Customers
                        .Include(x => x.StudentEducationInfos)
                        .ThenInclude(x => x.EducationDays)
                        .Where(x => x.StudentEducationInfos != null && x.StudentEducationInfos.Count > 0).ToList();
                    var reCalculateEducationGroups = _context.EducationGroups.Include(x=>x.Education).Include(x => x.GroupLessonDays).Where(x => x.GroupLessonDays.Any(y => y.DateOfLesson.Date == entity.Date.Date || y.DateOfLesson.Date == oldDay.Date)).ToList();


                    var reCalculateCustomers = customers.Where(x => x.StudentEducationInfos[0].EducationDays.Any(y => y.Date.Date == entity.Date.Date || y.Date.Date == oldDay.Date)).ToList();
                    ReCalculateGroupLessonDays(reCalculateEducationGroups);
                    ReCalculateEducationDays(reCalculateCustomers);
                    transaction.Commit();
                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

            }



        }

        #region Helper Methods
        /// <summary>
        /// Eğitim günlerini tekrar hesaplayıp db ye ekliyor.
        /// </summary>
        /// <param name="customers"></param>
        private void ReCalculateEducationDays(List<Customer> customers)
        {
            var offDays = _context.OffDays.Where(x => x.Year == DateTime.Now.Year || x.Year == DateTime.Now.Year - 1 || x.Year == DateTime.Now.Year + 1).ToList();
            foreach (var customer in customers)
            {
                foreach (var studentEducationInfo in customer.StudentEducationInfos)
                {
                    var nbuyCategory = _context.EducationCategories.FirstOrDefault(x => x.Id == studentEducationInfo.CategoryId);
                    var educationDayCount = nbuyCategory.EducationDayCount.HasValue ? nbuyCategory.EducationDayCount.Value : 0;

                    if (studentEducationInfo.EducationDays != null && studentEducationInfo.EducationDays.Count > 0)
                    {
                        //Tekrar hesaplanacak olan günleri yenisini yazmak için db den siliyoruz
                        var deletedItems = _context.EducationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id).ToList();
                        if (deletedItems != null && deletedItems.Count > 0)
                        {
                            _context.EducationDays.RemoveRange(deletedItems);
                        }
                        //Yeni günleri oluşturuyoruz.
                        DateTime activeDate = studentEducationInfo.StartedAt.Date;
                        for (int i = 0; i < educationDayCount; i++)
                        {
                            activeDate = activeDate.AddDays(1);
                            if (CheckWeekdays(activeDate) && CheckNotHoliday(activeDate, offDays))
                            {
                                _context.EducationDays.Add(new EducationDay
                                {
                                    Date = activeDate,
                                    Day = i + 1,
                                    StudentEducationInfoId = studentEducationInfo.Id
                                });
                            }
                            else
                            {
                                i--;
                            }
                        }
                        _context.SaveChanges();
                    }
                }
            }
        }


        private void ReCalculateGroupLessonDays(List<EducationGroup> groups)
        {
            var offDays = _context.OffDays.Where(x => x.Year == DateTime.Now.Year || x.Year == DateTime.Now.Year - 1 || x.Year == DateTime.Now.Year + 1).Select(x=>x.Date).ToList();

            foreach (var group in groups)
            {
                var weekDays = _context.WeekDaysOfGroups.First(x => x.GroupId == group.Id);
                var days = JsonConvert.DeserializeObject<List<int>>(weekDays.DaysJson);
                var oldDates = _context.GroupLessonDays.Where(x => x.GroupId == group.Id).ToList();
                 
               var newDates = CreateGroupLessonDays(group, days, offDays);
                group.StartDate = newDates[0];
                _context.GroupLessonDays.RemoveRange(oldDates);
                var newGroupLessonDays = new List<GroupLessonDay>();
                foreach (var newdate in newDates)
                    newGroupLessonDays.Add(new GroupLessonDay
                    {
                        DateOfLesson = newdate,
                        GroupId = group.Id,
                        ClassroomId = group.GroupLessonDays[0].ClassroomId,
                        HasAttendanceRecord = false,
                        IsImmuneToAutoChange = false,
                        EducatorId = group.GroupLessonDays[0].EducatorId,
                        EducatorSalary = group.GroupLessonDays[0].EducatorSalary
                    });

                _context.GroupLessonDays.AddRange(newGroupLessonDays);
                _context.SaveChanges();
            }


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


        /// <summary>
        /// Parametre olarak gönderilen tarihin hafta içi olması durumunu kontrol eder.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Hafta içi ise true döner.</returns>
        public bool CheckWeekdays(DateTime date)
        {
            if (!(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Parametre olarak gönderilen aktif günün tatil olup olmaması durumunu kontrol eder.
        /// </summary>
        /// <param name="date">Geçerli gün</param>
        /// <param name="offDays">Tatil günleri listesi</param>
        /// <returns>Tatil değil ise true döner.</returns>
        public bool CheckNotHoliday(DateTime date, List<OffDay> offDays)
        {
            foreach (var offDay in offDays)
            {
                if (offDay.Date.Date == date.Date)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Kayıt tekrarı varsa True döner.
        /// </summary>
        /// <param name="date">Tatil günü</param>
        /// <returns></returns>
        public bool IsDuplicate(int day, int month, int year)
        {
            var temp = _context.OffDays.FirstOrDefault(x => x.Day == day && x.Month == month && x.Year == year);
            if (temp != null)
            {
                return true;
            }
            return false;
        }
        #endregion


    }
}



