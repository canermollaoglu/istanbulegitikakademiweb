using Microsoft.EntityFrameworkCore;
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

                    ReCalculateEducationDays(customers);
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

                    ReCalculateEducationDays(customers);
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int id = base.Update(entity);
                    var customers = _context.Customers
                        .Include(x => x.StudentEducationInfos)
                        .ThenInclude(x => x.EducationDays)
                        .Where(x => x.StudentEducationInfos != null && x.StudentEducationInfos.Count > 0).ToList();

                    ReCalculateEducationDays(customers);
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
                    var nbuyCategory = _context.EducationCategories.FirstOrDefault(x => x.Id == studentEducationInfo.CategoryId.GetValueOrDefault());
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



