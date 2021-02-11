using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Core.ViewModels.areas.educator_area.group;
using NitelikliBilisim.Core.ViewModels.areas.educator_area.payment;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MUsefulMethods;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.Main.Educator;
using NitelikliBilisim.Core.ViewModels.Main.AboutUs;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducatorRepository : BaseRepository<Educator, string>
    {
        private readonly NbDataContext _context;

        public EducatorRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
        public string Insert(Educator entity, List<int> certificateIds,List<Guid> categoryIds, bool isSaveLater = false)
        {
            string educatorId = base.Insert(entity, isSaveLater);

            if (certificateIds != null && certificateIds.Count > 0)
            {
                var bridge = new List<Bridge_EducatorCertificate>();
                foreach (var certificateId in certificateIds)
                {
                    bridge.Add(new Bridge_EducatorCertificate
                    {
                        Id = educatorId,
                        Id2 = certificateId
                    });
                }
                Context.Bridge_EducatorEducatorCertificates.AddRange(bridge);
                Context.SaveChanges();
            }

            if (categoryIds != null && categoryIds.Count > 0)
            {
                var bridge = new List<Bridge_EducatorCategory>();
                foreach (var categoryId in categoryIds)
                {
                    bridge.Add(new Bridge_EducatorCategory
                    {
                        Id = educatorId,
                        Id2 = categoryId
                    });
                }
                Context.Bridge_EducatorCategories.AddRange(bridge);
                Context.SaveChanges();
            }

            return educatorId;
        }

        public IQueryable<EducatorGroupVm> GetEducatorGroupsByEducatorId(string educatorId)
        {
            var data = from educator in _context.Educators
                       join egroup in _context.EducationGroups on educator.Id equals egroup.EducatorId
                       join host in _context.EducationHosts on egroup.HostId equals host.Id
                       join education in _context.Educations on egroup.EducationId equals education.Id
                       join salaries in _context.EducatorSalaries on new { X1 = egroup.Id, X2 = educator.Id } equals new { X1 = salaries.EarnedForGroup.Value, X2 = salaries.EducatorId } into eSalaries
                       from salaries in eSalaries.DefaultIfEmpty()
                       where educator.Id == educatorId
                       select new EducatorGroupVm
                       {
                           Id = egroup.Id,
                           Name = egroup.GroupName,
                           StartDate = egroup.StartDate,
                           HostName = host.HostName,
                           EducationName = education.Name,
                           EducatorSalary = salaries!=null? salaries.Paid:0
                       };
            return data;
        }

        public int Update(Educator entity, List<int> certificateIds,List<Guid> categoryIds, bool isSaveLater = false)
        {
            var _certificates = Context.Bridge_EducatorEducatorCertificates.Where(x => x.Id == entity.Id);
            Context.Bridge_EducatorEducatorCertificates.RemoveRange(_certificates);
            var _categories = Context.Bridge_EducatorCategories.Where(x => x.Id == entity.Id);
            Context.Bridge_EducatorCategories.RemoveRange(_categories);


            if (certificateIds != null && certificateIds.Count > 0)
            {
                var newCertificates = new List<Bridge_EducatorCertificate>();
                foreach (var certificateId in certificateIds)
                {
                    newCertificates.Add(new Bridge_EducatorCertificate
                    {
                        Id = entity.Id,
                        Id2 = certificateId
                    });
                };
                Context.Bridge_EducatorEducatorCertificates.AddRange(newCertificates);
                Context.SaveChanges();
            }
            if (categoryIds != null && categoryIds.Count > 0)
            {
                var newcategories = new List<Bridge_EducatorCategory>();
                foreach (var categoryId in categoryIds)
                {
                    newcategories.Add(new Bridge_EducatorCategory
                    {
                        Id = entity.Id,
                        Id2 = categoryId
                    });
                };
                Context.Bridge_EducatorCategories.AddRange(newcategories);
                Context.SaveChanges();
            }


            return base.Update(entity, isSaveLater);
        }

        public int GetEducatorCount()
        {
            return _context.Educators.Count();
        }

        public List<EducatorListVm> GetEducatorsAboutUsPage()
        {
            var data = _context.Educators.Include(x => x.User).Select(x => new EducatorListVm
            {
                Id = x.Id,
                Name = x.User.Name,
                Surname = x.User.Surname,
                Title = x.Title,
                AvatarPath = x.User.AvatarPath
            });
            return data.ToList();
        }

        /// <summary>
        /// Admin Tarafında kullanılıyor.
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        public EducatorDetailVm GetEducatorDetailAdmin(string educatorId)
        {
            var educator = _context.Educators.Include(x => x.User).First(x => x.Id == educatorId);
            var certificates = (from bridge in _context.Bridge_EducatorEducatorCertificates
                                join certificate in _context.EducatorCertificates on bridge.Id2 equals certificate.Id
                                where bridge.Id == educatorId
                                select certificate).ToList();

            EducatorDetailVm retVal = new EducatorDetailVm()
            {
                Id = educator.Id,
                UserName = educator.User.UserName,
                Name = educator.User.Name,
                Surname = educator.User.Surname,
                AvatarPath = educator.User.AvatarPath,
                Email = educator.User.Email,
                Phone = educator.User.PhoneNumber,
                Title = educator.Title,
                ShortDescription = educator.ShortDescription,
                Bank = EnumHelpers.GetDescription<BankNames>((BankNames)educator.Bank),
                IBAN = educator.IBAN,
                Biography = educator.Biography,
                Certificates = certificates
            };
            return retVal;
        }
        /// <summary>
        /// User Tarafında kullanılıyor
        /// </summary>
        /// <returns></returns>
        public GetEducatorDetailVm GetEducatorDetailUser(string educatorId)
        {
            var educator = _context.Educators.Include(x => x.User).First(x => x.Id == educatorId);

            int educationCount = _context.Bridge_EducationEducators.Where(x => x.Id2 == educatorId).Count();

            int studentCount = _context.Bridge_GroupStudents.Include(x => x.Group).Where(x => x.Group.EducatorId == educatorId).Count();
            //(from invoiceDetail in _context.InvoiceDetails
            //                join eGroup in _context.EducationGroups on invoiceDetail.GroupId equals eGroup.Id
            //                where eGroup.EducatorId == educatorId
            //                select invoiceDetail).Count();


            var certificates = (from bridge in _context.Bridge_EducatorEducatorCertificates
                                join certificate in _context.EducatorCertificates on bridge.Id2 equals certificate.Id
                                where bridge.Id == educatorId
                                select certificate).ToList();
            var educationCategories = (from bridge in _context.Bridge_EducatorCategories
                                       join category in _context.EducationCategories on bridge.Id2 equals category.Id
                                       where bridge.Id == educatorId
                                       select category).ToList();

            var socialMedias = _context.EducatorSocialMedias.Where(x => x.EducatorId == educatorId);
            var facebook = socialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Facebook);
            var linkedIn = socialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.LinkedIn);
            var twitter = socialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Twitter);
            var google = socialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.GooglePlus);
            GetEducatorDetailVm retVal = new GetEducatorDetailVm()
            {
                Id = educator.Id,
                Name = $"{educator.User.Name} {educator.User.Surname}",
                AvatarPath = educator.User.AvatarPath,
                Email = educator.User.Email,
                Title = educator.Title,
                ShortDescription = educator.ShortDescription,
                Biography = educator.Biography,
                Certificates = certificates,
                Categories = educationCategories,
                FacebookUrl = facebook != null ? facebook.Link : "#",
                TwitterUrl = twitter != null ? twitter.Link : "#",
                LinkedInUrl = linkedIn != null ? linkedIn.Link : "#",
                GooglePlusUrl = google != null ? google.Link : "#",
                EducationCount = educationCount,
                StudentCount = studentCount
            };
            return retVal;
        }

        public List<EducationCategory> GetProfessions(string id)
        {
            var data = (from b in Context.Bridge_EducatorCategories
                        join c in Context.EducationCategories on b.Id2 equals c.Id
                        where b.Id == id
                        select c).ToList();
            return data;
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

        public IQueryable<_Educator> GetListQueryable()
        {
            return Context.Users
                  .Join(Context.Educators, l => l.Id, r => r.Id, (x, y) => new _Educator
                  {
                      Id = x.Id,
                      FullName = x.Name + " " + x.Surname,
                      Title = y.Title,
                      Phone = x.PhoneNumber,
                      Email = x.Email
                  });
        }

        public IQueryable<_Educator> GetEducatorListByCertificateId(int certificateId)
        {
            var data = from e in Context.Educators
                       join us in Context.Users on e.Id equals us.Id
                       join ec in Context.Bridge_EducatorEducatorCertificates on e.Id equals ec.Id
                       join c in Context.EducatorCertificates on ec.Id2 equals c.Id
                       where c.Id == certificateId
                       select new _Educator
                       {
                           Id = e.Id,
                           FullName = us.Name + "  " + us.Surname,
                           Title = e.Title,
                           Email = us.Email,
                           Phone = us.PhoneNumber
                       };

            return data;
        }
        public List<EducatorCertificate> GetCertificates(string userId)
        {
            var data = (from b in Context.Bridge_EducatorEducatorCertificates
                        join c in Context.EducatorCertificates on b.Id2 equals c.Id
                        where b.Id == userId
                        select c).ToList();
            return data;
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
