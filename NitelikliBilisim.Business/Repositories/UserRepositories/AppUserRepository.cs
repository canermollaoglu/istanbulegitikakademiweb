using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.helper;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.promotion;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Account;
using NitelikliBilisim.Core.ViewModels.Main;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NitelikliBilisim.Business.Repositories
{
    public class AppUserRepository
    {
        private readonly NbDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AppUserRepository(NbDataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }
        public MyAccountSidebarVm GetMyAccountSidebarInfo(string userId, string currentPageName)
        {
            MyAccountSidebarVm model = new MyAccountSidebarVm();
            var student = _context.Customers.Include(x => x.User).Include(x => x.StudentEducationInfos).ThenInclude(x => x.Category).First(x => x.Id == userId);
            University university = null;
            StudentEducationInfo nBuyInfo = null;
            if (student.LastGraduatedSchoolId.HasValue)
            {
                university = _context.Universities.FirstOrDefault(x => x.Id == student.LastGraduatedSchoolId);
            }
            if (student.IsNbuyStudent && student.StudentEducationInfos[0] != null)
            {
                nBuyInfo = student.StudentEducationInfos[0];
            }
            model.Name = student.User.Name;
            model.Surname = student.User.Surname;
            model.IsNBUY = student.IsNbuyStudent;
            model.University = university != null ? university.Name : "Girilmemiş";
            model.NbuyCategory = nBuyInfo != null && nBuyInfo.Category != null ? nBuyInfo.Category.Name : "Girilmemiş";
            model.AvatarPath = student.User.AvatarPath;
            model.PageName = currentPageName;
            model.CertificateCount = GetUserCertificateCount(userId);
            return model;
        }

        public async Task<ResponseData> RegisterUser(RegisterPostVm model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Surname = model.Surname,
                    Email = model.Email,
                    PhoneNumber = model.Phone
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result == IdentityResult.Success)
                {
                    try
                    {
                        result = await _userManager.AddToRoleAsync(user,
                            _userManager.Users.Count() == 1
                                ? IdentityRoleList.Admin.ToString()
                                : IdentityRoleList.User.ToString());
                        var customer = new Customer
                        {
                            Id = user.Id,
                            CustomerType = CustomerType.Individual,
                            IsNbuyStudent = model.IsNbuyStudent
                        };
                        _context.Customers.Add(customer);
                        if (model.IsNbuyStudent)
                        {
                            var studentEducationInformation = new StudentEducationInfo
                            {
                                CustomerId = user.Id,
                                StartedAt = Convert.ToDateTime(model.StartedAt),
                                EducationCenter = (EducationCenter)model.EducationCenter.Value,
                                CategoryId = model.EducationCategory.Value
                            };
                            var studentEducationInfo = _context.StudentEducationInfos.Add(studentEducationInformation);
                            if (model.EducationCategory.HasValue && !string.IsNullOrEmpty(model.StartedAt))
                                CreateEducationDays(studentEducationInfo.Entity.Id, model.EducationCategory.Value, Convert.ToDateTime(model.StartedAt));
                        }
                        transaction.Commit();
                        return new ResponseData
                        {
                            Success = true,
                            Data = user
                        };

                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return new ResponseData
                        {
                            Success = false,
                            Message = e.Message
                        };
                    }
                }
                return new ResponseData
                {
                    Success = false,
                    Data = result.Errors
                };
            }

        }

        public UserInfoVm GetCustomerInfo(string userId)
        {
            var customer = _context.Customers
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == userId);

            #region Favori eklenen eğitimler
            var wishListItems = _context.Wishlist.Where(x => x.Id == userId).ToList();
            List<Guid> wishListEducationIds = wishListItems.Select(x => x.Id2).ToList();
            List<EducationVm> _wishList = new List<EducationVm>();
            var educationsList = _context.Educations.Where(x => wishListEducationIds.Contains(x.Id) && x.IsActive)
               .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.Card), l => l.Id, r => r.EducationId, (x, y) => new
               {
                   Education = x,
                   EducationPreviewMedia = y
               })
               .Join(_context.EducationCategories, l => l.Education.CategoryId, r => r.Id, (x, y) => new
               {
                   Education = x.Education,
                   EducationPreviewMedia = x.EducationPreviewMedia,
                   CategoryName = y.Name
               }).ToList();

            _wishList = educationsList.Select(x => new EducationVm
            {
                Base = new EducationBaseVm
                {
                    Id = x.Education.Id,
                    Name = x.Education.Name,
                    Description = x.Education.Description,
                    CategoryName = x.CategoryName,
                    Level = EnumHelpers.GetDescription(x.Education.Level),
                    //PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
            }).ToList();

            #endregion

            var _personalAndAccount = new _PersonalAccountInfo
            {
                FirstName = customer.User.Name,
                LastName = customer.User.Surname,
                PhoneNumber = customer.User.PhoneNumber,
                Email = customer.User.Email,
                UserName = customer.User.UserName,
                FilePath = customer.User.AvatarPath,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender,
                Job = EnumHelpers.GetDescription(customer.Job),
                LinkedInProfileUrl = customer.LinkedInProfileUrl,
                WebSiteUrl = customer.WebSiteUrl
            };
            _EducationInfo _educationInfo = null;
            if (customer.IsNbuyStudent)
            {
                var studentEducationInfo = _context.StudentEducationInfos.First(x => x.CustomerId == customer.Id);
                //Öğrencinin yalnız bir nbuy eğitimi aldığı varsayıldığı için first ile ilk eğitim çekildi.
                var educationDays = _context.EducationDays.ToList();
                var educationDay = educationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).FirstOrDefault();

                _educationInfo = new _EducationInfo
                {
                    EducationCategory = _context.EducationCategories.First(x => x.Id == studentEducationInfo.CategoryId).Name,
                    EducationCenter = EnumHelpers.GetDescription(studentEducationInfo.EducationCenter),
                    StartedAt = studentEducationInfo.StartedAt,
                    NBUYCurrentEducationDay = educationDay != null ? educationDay.Day : 0
                };
            }

            var tickets = _context.Tickets
                .Include(x => x.Host)
                .Include(x => x.Education)
                .Where(x => x.OwnerId == customer.Id)
                .ToList();

            var _tickets = new List<_Ticket>();
            foreach (var ticket in tickets)
                _tickets.Add(new _Ticket
                {
                    TicketId = ticket.Id,
                    EducationId = ticket.EducationId,
                    EducationName = ticket.Education.Name,
                    HostId = ticket.HostId,
                    HostName = ticket.Host.HostName,
                    HostCity = EnumHelpers.GetDescription(ticket.Host.City),
                    IsUsed = ticket.IsUsed
                });

            var model = new UserInfoVm
            {
                PersonalAndAccountInfo = _personalAndAccount,
                EducationInfo = _educationInfo,
                Tickets = _tickets,
                WishList = _wishList
            };

            return model;
        }


        public List<MyCertificateVm> GetUserAvailableCertificates(string userId)
        {
            var certificates = (from certificate in _context.CustomerCertificates
                                join student in _context.Customers.Include(x => x.User) on certificate.CustomerId equals student.Id
                                join eGroup in _context.EducationGroups on certificate.GroupId equals eGroup.Id
                                join education in _context.Educations.Include(x => x.Category).ThenInclude(x => x.BaseCategory) on eGroup.EducationId equals education.Id
                                join bridge in _context.Bridge_GroupStudents on new { Id = student.Id, Id2 = eGroup.Id } equals new { Id = bridge.Id2, Id2 = bridge.Id } into b
                                from bridge in b.DefaultIfEmpty()
                                where certificate.CustomerId == userId
                                select new MyCertificateVm
                                {
                                    Id = certificate.Id,
                                    EducationName = education.Name,
                                    EducationDate = eGroup.StartDate,
                                    EducationSeoUrl = education.SeoUrl,
                                    CategorySeoUrl = education.Category.SeoUrl,
                                    CategoryName = education.Category.BaseCategory.Name,
                                    EducationDateText = eGroup.StartDate.ToString("dd MMMM yyyy")
                                }).ToList();
            return certificates;
        }


        public List<MyCertificateVm> GetUserCertificates(string userId)
        {
            var certificates = (from bridge in _context.Bridge_GroupStudents
                                join eGroup in _context.EducationGroups on bridge.Id equals eGroup.Id
                                join student in _context.Customers on bridge.Id2 equals student.Id
                                join education in _context.Educations on eGroup.EducationId equals education.Id
                                join educationImage in _context.EducationMedias on new {id=education.Id,mType =EducationMediaType.Square } equals new { id=educationImage.EducationId,mType=educationImage.MediaType}
                                join educationCardImage in _context.EducationMedias on new { id = education.Id, mType = EducationMediaType.Card } equals new { id = educationCardImage.EducationId, mType = educationCardImage.MediaType }
                                where bridge.Id2 == userId && eGroup.StartDate.AddDays(education.Days) < DateTime.Now
                                select new MyCertificateVm
                                {
                                    GroupId = eGroup.Id,
                                    EducationName = education.Name,
                                    EducationImageUrl = educationImage.FileUrl,
                                    EducationCardImageUrl = educationCardImage.FileUrl,
                                    EducationDate = eGroup.StartDate,
                                    EducationSeoUrl = education.SeoUrl,
                                    CategorySeoUrl = education.Category.SeoUrl,
                                    CategoryName = education.Category.BaseCategory.Name,
                                    EducationDateText = eGroup.StartDate.ToString("dd MMMM yyyy")
                                }).ToList();
            var availableCertificates = new List<MyCertificateVm>();

            foreach (var certificate in certificates)
            {
                var attendance = _context.GroupAttendances.Where(x => x.GroupId == certificate.GroupId && x.CustomerId == userId).ToList();
                if (attendance.Count == 0)
                {
                    availableCertificates.Add(certificate);
                }
            }

            return availableCertificates;
        }

        public InvoiceDetailsVm GetCustomerInvoiceDetails(Guid invoiceId)
        {
            InvoiceDetailsVm model = new();
            var totalPrice = 0m;
            var cultureInfo = CultureInfo.CreateSpecificCulture("tr-TR");
            var invoice = _context.Invoices.Include(x => x.OnlinePaymentInfo).Include(x => x.InvoiceDetails).First(x => x.Id == invoiceId);
            var groupIds = invoice.InvoiceDetails.Select(x => x.GroupId).ToList();
            var groups = _context.EducationGroups.Where(x => groupIds.Contains(x.Id)).ToList();
            totalPrice = groups != null && groups.Count > 0 ? groups.Sum(x => x.OldPrice.GetValueOrDefault()) : 0m;

            model.TotalPrice = totalPrice.ToString(cultureInfo);
            model.PaidPrice = invoice.OnlinePaymentInfo.PaidPrice.ToString(cultureInfo);
            model.DiscountAmount = (totalPrice - invoice.OnlinePaymentInfo.PaidPrice).ToString(cultureInfo);
            model.PaymentId = invoice.OnlinePaymentInfo.PaymentId;
            model.Date = invoice.CreatedDate;
            model.InstallmentInfo = invoice.PaymentCount;
            var details = (from invoiceDetail in _context.InvoiceDetails
                           join onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos on invoiceDetail.Id equals onlinePaymentDetailInfo.Id
                           join education in _context.Educations on invoiceDetail.EducationId equals education.Id
                           join category in _context.EducationCategories on education.CategoryId equals category.Id
                           join educationImage in _context.EducationMedias on education.Id equals educationImage.EducationId
                           where educationImage.MediaType == EducationMediaType.Square && invoiceDetail.InvoiceId == invoiceId
                           select new InvoiceDetailListVm
                           {
                               Id = invoiceDetail.Id,
                               PaidPrice = onlinePaymentDetailInfo.PaidPrice.ToString(cultureInfo),
                               Education = education.Name,
                               EducationImage = educationImage.FileUrl,
                               CategorySeoUrl = category.SeoUrl,
                               EducationSeoUrl = education.SeoUrl
                           }).ToList();
            model.Details = details;
            return model;
        }
        public bool UpdateNbuyEducationInfo(UpdateNBUYEducationInfoVm model)
        {
            var info = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == model.UserId);
            if (info.StartedAt != model.StartAt || info.CategoryId != model.EducationCategoryId)
            {
                var oldDates = _context.EducationDays.Where(x => x.StudentEducationInfoId == info.Id);
                _context.EducationDays.RemoveRange(oldDates);
                CreateEducationDays(info.Id, model.EducationCategoryId, model.StartAt);
            }
            info.CategoryId = model.EducationCategoryId;
            info.EducationCenter = model.EducationCenter;
            info.StartedAt = model.StartAt;

            return _context.SaveChanges() > 0 ? true : false;
        }
        public bool UpdateStudentInfo(UpdateStudentInfoVm model)
        {
            var user = _context.Customers.Include(x => x.User).First(x => x.Id == model.UserId);
            user.DateOfBirth = model.BirthDate;
            user.LastGraduatedSchoolId = model.LastGraduatedSchoolId;
            user.Gender = model.Gender;
            user.Job = model.Job;
            return _context.SaveChanges() > 0 ? true : false;
        }


        public bool UpdateUserContactInfo(UpdateUserContactInfoVm model)
        {

            var user = _context.Customers.Include(x => x.User).First(x => x.Id == model.UserId);
            user.User.PhoneNumber = model.PhoneNumber;
            user.LinkedInProfileUrl = model.LinkedIn;
            user.WebSiteUrl = model.Website;
            user.CityId = model.CityId;
            return _context.SaveChanges() > 0 ? true : false;
        }

        public List<CustomerInvoiceListVm> GetCustomerInvoices(string userId)
        {
            var culture = CultureInfo.CreateSpecificCulture("tr-TR");

            var data = (from invoice in _context.Invoices
                        join onlinePayment in _context.OnlinePaymentInfos on invoice.Id equals onlinePayment.Id
                        where invoice.CustomerId == userId
                        select new CustomerInvoiceListVm
                        {
                            Id = invoice.Id,
                            PaymentId = onlinePayment.PaymentId,
                            Date = invoice.CreatedDate,
                            PaidPrice = onlinePayment.PaidPrice.ToString(culture),
                            FileUrl = invoice.InvoicePdfUrl
                        }).ToList();
            return data;
        }

        public MyAccountSettingsGetVm GetAccoutSettingsPageData(string userId)
        {
            MyAccountSettingsGetVm model = new();
            var data = (from user in _context.Users
                        where user.Id == userId
                        join student in _context.Customers on user.Id equals student.Id
                        select new MyAccountSettingsGeneralInformationVm
                        {
                            AvatarPath = user.AvatarPath,
                            Email = user.Email,
                            Name = user.Name,
                            Surname = user.Surname,
                            DateOfBirth = student.DateOfBirth,
                            Job = student.Job,
                            LastGraduatedSchoolId = student.LastGraduatedSchoolId,
                            Phone = user.PhoneNumber,
                            LinkedIn = student.LinkedInProfileUrl,
                            WebSite = student.WebSiteUrl,
                            CityId = student.CityId,
                            Gender = student.Gender,
                            IsNbuyStudent = student.IsNbuyStudent
                        }).First();

            if (data.IsNbuyStudent)
            {
                var nbuyInfo = _context.StudentEducationInfos.FirstOrDefault(x => x.CustomerId == userId);
                if (nbuyInfo != null)
                {
                    data.NbuyInformation = new NbuyInformationVm
                    {
                        Id = nbuyInfo.Id,
                        CategoryId = nbuyInfo.CategoryId,
                        StartAt = nbuyInfo.StartedAt,
                        EducationCenter = nbuyInfo.EducationCenter
                    };
                }
            }


            var addresses = _context.Addresses.Include(x => x.City).ThenInclude(x => x.States).Include(x => x.State).Where(x => x.CustomerId == userId).ToList();
            var defaultaddress = addresses.FirstOrDefault(x => x.IsDefaultAddress);
            model.GeneralInformation = data;
            model.Addresses = addresses;
            model.DefaultAddressId = defaultaddress != null ? defaultaddress.Id : 0;
            model.Universities = _context.Universities.ToList();
            model.Cities = _context.Cities.OrderBy(x => x.Order).ToList();
            model.Genders = EnumHelpers.ToKeyValuePair<Genders>();
            model.Jobs = EnumHelpers.ToKeyValuePair<Jobs>();
            model.EducationCenters = EnumHelpers.ToKeyValuePair<EducationCenter>();
            model.EducationCategories = _context.EducationCategories.Where(x => x.BaseCategoryId == null).ToDictionary(x => x.Id, x => x.Name);
            return model;
        }



        public ForYouPageGetVm GetForYouPageData(string userId)
        {
            var infos = _context.Customers
                .Include(x => x.StudentEducationInfos)
                .ThenInclude(x => x.Category)
                .Include(x => x.StudentEducationInfos)
                .ThenInclude(x => x.EducationDays)
                .First(x => x.Id == userId);
            var nbuyInfo = infos.StudentEducationInfos[0];
            var totalEducationWeeks = (int)Math.Ceiling((nbuyInfo.EducationDays.OrderBy(x => x.Day).Last().Date - nbuyInfo.StartedAt.Date).TotalDays / (double)7);
            var currentEducationWeek = (int)Math.Ceiling((DateTime.Now.Date - nbuyInfo.StartedAt).TotalDays / (double)7); //GetEducationWeek(userId);
            var totalEducationMonths = (int)Math.Ceiling(totalEducationWeeks / (double)4);
            ForYouPageGetVm model = new();
            int week = 1;
            for (int i = 1; i <= totalEducationMonths; i++)
            {
                var eMonth = new EducationMonth();
                eMonth.Order = i;
                for (int j = 1; j <= 4; j++)
                {
                    if (week <= totalEducationWeeks)
                    {
                        eMonth.Weeks.Add(new EducationWeek
                        {
                            IsCurrentWeek = currentEducationWeek == week,
                            Order = week++
                        });
                    }
                }
                model.EducationMonths.Add(eMonth);
            }
            model.EducationCategory = nbuyInfo.Category.Name;
            model.EducationCategoryId = nbuyInfo.CategoryId;
            model.EducationWeek = currentEducationWeek;
            model.LeftWeeks = totalEducationWeeks - currentEducationWeek;
            model.NbuyStartDate = nbuyInfo.StartedAt.ToString("dd MMMM yyyy");
            model.NbuyEndDate = nbuyInfo.EducationDays.OrderBy(x => x.Date).Last().Date.ToString("dd MMMM yyyy");
            return model;
        }

        public List<MyCommentVm> GetCustomerComments(string userId)
        {
            var model = (from comment in _context.EducationComments
                         join education in _context.Educations on comment.EducationId equals education.Id
                         join category in _context.EducationCategories on education.CategoryId equals category.Id
                         join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                         where eImage.MediaType == EducationMediaType.Square && comment.CommentatorId == userId
                         select new MyCommentVm
                         {
                             Id = comment.Id,
                             SeoUrl = education.SeoUrl,
                             CategorySeoUrl = category.SeoUrl,
                             EducationId = education.Id,
                             Content = comment.Content,
                             Point = comment.Points,
                             Date = comment.CreatedDate.ToString("dd MMMM yyyy"),
                             EducationName = education.Name,
                             EducationFeaturedImage = eImage.FileUrl,
                             CategoryName = category.Name,
                             ApprovalStatus = comment.ApprovalStatus,
                         }).ToList();

            foreach (var comment in model)
            {
                comment.EducatorPictureUrls = _context.Bridge_EducationEducators.Include(x => x.Educator).ThenInclude(x => x.User).Where(x => x.Id == comment.EducationId).Select(x => x.Educator.User.AvatarPath).ToList();
            }

            return model;
        }

        public MyCourseDetailVm GetPurschasedEducationDetail(string userId, Guid groupId)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("tr-TR");
            var model = (from eGroup in _context.EducationGroups
                         where eGroup.Id == groupId
                         join education in _context.Educations on eGroup.EducationId equals education.Id
                         join category in _context.EducationCategories on education.CategoryId equals category.Id
                         join educator in _context.Educators on eGroup.EducatorId equals educator.Id
                         join host in _context.EducationHosts on eGroup.HostId equals host.Id
                         join educatorUser in _context.Users on educator.Id equals educatorUser.Id
                         join eImage in _context.EducationMedias on new { Id = education.Id, MediaType = EducationMediaType.Card } equals new { Id = eImage.EducationId, MediaType = eImage.MediaType }
                         join invoiceDetail in _context.InvoiceDetails.Include(x => x.Invoice) on new { GroupId = eGroup.Id, UserId = userId } equals new { invoiceDetail.GroupId, UserId = invoiceDetail.Invoice.CustomerId }
                         join onlinePaymentDetailInfo in _context.OnlinePaymentDetailsInfos on invoiceDetail.Id equals onlinePaymentDetailInfo.Id
                         where !onlinePaymentDetailInfo.IsCancelled
                         select new MyCourseDetailVm
                         {
                             GroupId = eGroup.Id,
                             InvoiceDetailId = invoiceDetail.Id,
                             EducationDate = eGroup.StartDate,
                             EducationEndDate = eGroup.StartDate.AddDays(education.Days),
                             CategoryName = category.Name,
                             EducationId = education.Id,
                             SeoUrl = education.SeoUrl,
                             CategorySeoUrl = category.SeoUrl,
                             EducationName = education.Name,
                             EducationShortDescription = education.Description,
                             EducationFeaturedImage = eImage.FileUrl,
                             EducatorId = educator.Id,
                             EducatorName = $"{educatorUser.Name} {educatorUser.Surname}",
                             EducatorTitle = educator.Title,
                             EducatorAvatarPath = educatorUser.AvatarPath,
                             PriceText = onlinePaymentDetailInfo.PaidPrice.ToString(cultureInfo),
                             Days = education.Days.ToString(),
                             Hours = (education.Days * education.HoursPerDay).ToString(),
                             Host = host.HostName,
                             IsCancelled = onlinePaymentDetailInfo.IsCancelled
                         }).First();
            model.EducatorPoint = GetEducatorPoint(model.EducatorId);
            model.EducatorStudentCount = _context.Bridge_GroupStudents.Include(x => x.Group).Where(x => x.Group.EducatorId == model.EducatorId).Count();
            var group = _context.EducationGroups.Include(x => x.GroupLessonDays).First(x => x.Id == groupId);
            model.IsRefundable = model.IsCancelled || group.GroupLessonDays.OrderByDescending(x => x.DateOfLesson).First().DateOfLesson.Date < DateTime.Now.Date ? false : true;
            #region test 
            var attendance = _context.GroupAttendances.Where(x => x.GroupId == model.GroupId && x.CustomerId == userId).ToList();
            if (attendance.Count == 0 && model.EducationEndDate < DateTime.Now)
            {
                model.IsCertificateAvailable = true;
            }
            #endregion

            var bridgeEducatorCertificates = _context.Bridge_EducatorEducatorCertificates.Include(x => x.EducatorCertificate).Where(x => x.Id == model.EducatorId).ToList();
            var educatorCertificates = new List<EducatorCertificate>();
            foreach (var bridge in bridgeEducatorCertificates)
            {
                educatorCertificates.Add(bridge.EducatorCertificate);
            }
            model.EducatorCertificates = educatorCertificates;

            return model;

        }

        public bool CheckPurschasedEducation(string userId, Guid groupId)
        {
            var purchasedEducations = _context.Bridge_GroupStudents.FirstOrDefault(x => x.Id2 == userId && x.Id == groupId);
            return purchasedEducations == null ? false : true;
        }

        public List<MyCourseVm> GetPurschasedEducationsByUserIdMyCoursesPage(string userId)
        {
            //var purchasedEducations = _context.Bridge_GroupStudents.Where(x => x.Id2 == userId);
            var wishListItems = _context.Wishlist.Where(x => x.Id == userId).Select(x => x.Id2).ToList();

            //List<Guid> ids = purchasedEducations.Select(x => x.Id).ToList();
            var educationList = (from bridge in _context.Bridge_GroupStudents where bridge.Id2 == userId
                                 join ticket in _context.Tickets on bridge.TicketId equals ticket.Id
                                 join eGroup in _context.EducationGroups on bridge.Id equals eGroup.Id
                                 join education in _context.Educations on eGroup.EducationId equals education.Id
                                 join eImage in _context.EducationMedias on new { Id = education.Id, IType = EducationMediaType.Square } equals new { Id = eImage.EducationId, IType = eImage.MediaType }
                                 where ticket.IsUsed
                                 select new MyCourseVm
                                 {
                                     Id = eGroup.Id,
                                     EducationId = education.Id,
                                     Name = education.Name,
                                     Date = eGroup.StartDate.ToString("dd MMMM yyyy"),
                                     Days = education.Days.ToString(),
                                     Hours = (education.Days * education.HoursPerDay).ToString(),
                                     FeaturedImageUrl = eImage.FileUrl,
                                 }).ToList();
            foreach (var education in educationList)
            {
                education.IsFavorite = wishListItems.Contains(education.Id) ? true : false;
            }
            return educationList;
        }

        public MyPanelVm GetPanelInfo(string userId)
        {
            MyPanelVm model = new MyPanelVm();
            var student = _context.Customers.Include(x => x.User)
                .Include(x => x.StudentEducationInfos).ThenInclude(x => x.Category)
                .Include(x => x.StudentEducationInfos).ThenInclude(x => x.EducationDays).First(x => x.Id == userId);
            var favoriteEducations = GetUserFavoriteEducationsByUserId(userId);
            var purchasedEducations = GetPurschasedEducationsByUserId(userId);

            var certificates = GetUserCertificates(userId);
            model.FavoriteEducations = favoriteEducations;
            model.FavoriteEducationCount = favoriteEducations.Count;
            model.PurchasedEducations = purchasedEducations;
            model.Certificates = certificates;
            model.PurchasedEducationCount = purchasedEducations.Count;
            model.IsNBUY = student.IsNbuyStudent;
            if (student.IsNbuyStudent)
            {
                var nbuyInfo = student.StudentEducationInfos[0];
                var totalEducationWeek = (int)Math.Ceiling((nbuyInfo.EducationDays.OrderBy(x => x.Day).Last().Date - nbuyInfo.StartedAt.Date).TotalDays / (double)7);
                var currentEducationWeek = (int)Math.Ceiling((DateTime.Now.Date - nbuyInfo.StartedAt).TotalDays / (double)7);
                model.NbuyCategory = nbuyInfo.Category.Name;
                model.NbuyStartDateText = nbuyInfo.StartedAt.ToString("dd MMM yyyy");
                model.NbuyStartDate = nbuyInfo.StartedAt;
                model.EducationWeek = currentEducationWeek;
                model.TotalEducationWeek = totalEducationWeek;
            }
            return model;

        }


        #region Counts

        public int GetUserCertificateCount(string userId)
        {
            return _context.Bridge_GroupStudents.Include(x => x.Group).ThenInclude(x => x.Education).Where(x => x.Id2 == userId && x.Group.StartDate.AddDays(x.Group.Education.Days) < DateTime.Now).Count();

        }
        #endregion
        private int GetEducationWeek(string userId)
        {
            var retVal = 1;
            var student = _context.Customers.Include(x => x.StudentEducationInfos).ThenInclude(x => x.EducationDays).FirstOrDefault(x => x.Id == userId);
            if (student != null)
            {
                var currentDate = student.StudentEducationInfos[0].EducationDays.OrderBy(x => x.Date).Last(x => x.Date <= DateTime.Now.Date);
                retVal = (int)Math.Ceiling(currentDate.Day / (double)7);
            }
            return retVal;
        }

        public List<PurchasedEducationVm> GetPurschasedEducationsByUserId(string userId)
        {
            var purchasedEducations = _context.Bridge_GroupStudents.Where(x => x.Id2 == userId);
            List<Guid> ids = purchasedEducations.Select(x => x.Id).ToList();
            var educationList = (from eGroup in _context.EducationGroups
                                 join education in _context.Educations on eGroup.EducationId equals education.Id
                                 join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                                 join educator in _context.Educators on eGroup.EducatorId equals educator.Id
                                 join educatorUser in _context.Users on educator.Id equals educatorUser.Id
                                 join host in _context.EducationHosts on eGroup.HostId equals host.Id
                                 join category in _context.EducationCategories on education.CategoryId equals category.Id
                                 where eImage.MediaType == EducationMediaType.Card
                                 && ids.Contains(eGroup.Id)
                                 select new PurchasedEducationVm
                                 {
                                     GroupId = eGroup.Id,
                                     CreatedDate = eGroup.StartDate,
                                     EducationId = education.Id,
                                     Name = education.Name,
                                     CategoryName = category.Name,
                                     City = EnumHelpers.GetDescription(host.City),
                                     FeaturedImageUrl = eImage.FileUrl,
                                     EducatorImageUrl = educatorUser.AvatarPath,
                                 }
                ).OrderByDescending(x => x.CreatedDate).ToList();
            foreach (var data in educationList)
            {
                data.CompletionRate = GetEducationCompletionRate(data.GroupId);
            }
            return educationList;

        }

        private int GetEducationCompletionRate(Guid id)
        {
            var group = _context.EducationGroups.Include(x => x.Education).Include(x => x.GroupLessonDays).FirstOrDefault(x => x.Id == id);
            var rate = 0;
            if (group != null && DateTime.Now.Date > group.StartDate)
            {
                var completedDays = group.GroupLessonDays.Where(x => x.DateOfLesson.Date < DateTime.Now.Date).Count();
                var totalDays = group.Education.Days;
                rate = completedDays * 100 / totalDays;
            }

            return rate;
        }

        public List<FavoriteEducationVm> GetUserFavoriteEducationsByUserId(string userId)
        {
            var hostId = Guid.Parse(_configuration.GetSection("SiteGeneralOptions").GetSection("PriceLocationId").Value);
            var currentCulture = CultureInfo.CreateSpecificCulture("tr-TR");
            var educationsList = (from wishListItem in _context.Wishlist
                                  join education in _context.Educations on wishListItem.Id2 equals education.Id
                                  join featuredImage in _context.EducationMedias on education.Id equals featuredImage.EducationId
                                  join category in _context.EducationCategories on education.CategoryId equals category.Id
                                  where featuredImage.MediaType == EducationMediaType.Card && wishListItem.Id == userId && education.IsActive
                                  select new FavoriteEducationVm
                                  {
                                      Id = education.Id,
                                      CreatedDate = wishListItem.CreatedDate,
                                      SeoUrl = education.SeoUrl,
                                      Name = education.Name,
                                      CategoryName = category.Name,
                                      CategorySeoUrl = category.SeoUrl,
                                      Price = _context.EducationGroups.OrderByDescending(x => x.CreatedDate).FirstOrDefault(y => y.HostId == hostId && y.EducationId == education.Id).NewPrice.GetValueOrDefault().ToString(currentCulture),
                                      HoursText = (education.HoursPerDay * education.Days).ToString(),
                                      DaysText = education.Days.ToString(),
                                      FeaturedImageUrl = featuredImage.FileUrl
                                  }).OrderByDescending(x => x.CreatedDate).ToList();
            return educationsList;
        }

        public List<MyCouponVm> GetCustomerPromotions(string userId)
        {
            var conditions = _context.EducationPromotionConditions.Where(x => x.ConditionType == Core.Enums.promotion.ConditionType.User);
            var currentUserConditions = new List<Guid>();
            foreach (var condition in conditions)
            {
                var userIds = JsonConvert.DeserializeObject<string[]>(condition.ConditionValue);
                if (userIds.Contains(userId))
                {
                    currentUserConditions.Add(condition.EducationPromotionCodeId);
                }
            }

            var codes = _context.EducationPromotionCodes.Where(x => x.PromotionType == Core.Enums.promotion.PromotionType.CouponCode && currentUserConditions.Contains(x.Id));
            var uses = _context.EducationPromotionItems.Where(x => x.UserId == userId).ToList();
            var retVal = new List<MyCouponVm>();
            foreach (var promotionCode in codes)
            {
                MyCouponVm promotion = new MyCouponVm();
                promotion.PromotionName = promotionCode.Name;
                promotion.Code = promotionCode.PromotionCode;
                promotion.PromotionId = promotionCode.Id;
                promotion.RemainingTime = "0";
                if (uses.Any(x => x.EducationPromotionCodeId == promotionCode.Id))
                {
                    promotion.Status = PromotionStatus.Used;
                }
                else if (promotionCode.EndDate.Date < DateTime.Now.Date)
                {
                    promotion.Status = PromotionStatus.Expired;
                }
                else
                {
                    TimeSpan remainingTime = promotionCode.EndDate - DateTime.Now.Date;
                    promotion.RemainingTime = remainingTime.TotalDays.ToString();
                    promotion.Status = PromotionStatus.Active;
                }
                retVal.Add(promotion);
            }


            return retVal;
        }


        #region Helper Functions
        /// <summary>
        /// Eğitmen Puanını döner
        /// </summary>
        /// <param name="educatorId"></param>
        /// <returns></returns>
        private double GetEducatorPoint(string educatorId)
        {
            var data = (from eComment in _context.EducationComments
                        join education in _context.Educations on eComment.EducationId equals education.Id
                        join eGroup in _context.EducationGroups on education.Id equals eGroup.EducationId
                        where eGroup.EducatorId == educatorId
                        select (int)eComment.Points).ToList();
            return Math.Round(data.Average(), 2);
        }

        /// <summary>
        /// Girilen parametrelere göre NBUY eğitimi alan öğrencinin eğitim günlerini db ye ekler.
        /// </summary>
        /// <param name="studentEducationInfoId">Öğrencinin aldığı NBUY eğitiminin tutulduğu tablo.</param>
        /// <param name="categoryId">NBUY eğitimi alan öğrencinin aldığı eğitim kategorisi.</param>
        /// <param name="startDate">NBUY eğitim başlangıç tarihi.</param>
        private void CreateEducationDays(Guid studentEducationInfoId, Guid categoryId, DateTime startDate)
        {
            //Tatil Günleri
            var offDays = _context.OffDays.Where(x => x.Year == DateTime.Now.Year || x.Year == DateTime.Now.Year - 1 || x.Year == DateTime.Now.Year + 1).ToList();
            //Kullanıcının aldığı Nbuy Eğitimi ve eğitimin süresi
            var nbuyCategory = _context.EducationCategories.First(x => x.Id == categoryId);
            var educationDayCount = nbuyCategory.EducationDayCount.HasValue ? nbuyCategory.EducationDayCount.Value : 0;
            //Kullanıcının eğitime başlangıç tarihi 
            var activeDate = startDate;
            for (int i = 0; i < educationDayCount; i++)
            {
                activeDate = activeDate.AddDays(1);
                if (checkWeekdays(activeDate) && checkNotHoliday(activeDate, offDays))
                {
                    _context.EducationDays.Add(new EducationDay
                    {
                        Date = activeDate,
                        Day = i + 1,
                        StudentEducationInfoId = studentEducationInfoId
                    });
                }
                else
                {
                    i--;
                }
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Parametre olarak gönderilen tarihin hafta içi olması durumunu kontrol eder.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Hafta içi ise true döner.</returns>
        private bool checkWeekdays(DateTime date)
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
        private bool checkNotHoliday(DateTime date, List<OffDay> offDays)
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
        #endregion

        public List<MyInvoicesVm> GetUserInvoices(string userId)
        {
            List<MyInvoicesVm> retVal = new List<MyInvoicesVm>();
            var userAllTickets = _context.Tickets.Where(x => x.OwnerId == userId).ToList();
            var userGroups = _context.Bridge_GroupStudents.Include(x => x.Group).Where(x => x.Id2 == userId).ToList();
            var invoices = _context.Invoices
                .Include(x => x.InvoiceDetails)
                .ThenInclude(x => x.OnlinePaymentDetailInfo)
                .Include(x => x.OnlinePaymentInfo)
                .Where(x => x.CustomerId == userId);
            foreach (var invoice in invoices)
            {
                retVal.Add(new MyInvoicesVm
                {
                    Invoice = new _Invoice
                    {
                        BillingType = EnumHelpers.GetDescription(invoice.BillingType),
                        CompanyInfo = invoice.BillingType == CustomerType.Corporate ? new _CompanyInfo { CompanyName = invoice.CompanyName } : null,
                        CreatedDate = invoice.CreatedDate,
                        InvoiceId = invoice.Id,
                        IsIndividual = invoice.BillingType == CustomerType.Individual,
                        PaymentCount = invoice.PaymentCount,
                        TransactionStatus = EnumHelpers.GetDescription(invoice.TransactionStatus),
                        IsEligibleToFullyCancel = true
                    },
                    InvoiceDetails = invoice.InvoiceDetails.Select(x => new _InvoiceDetail
                    {
                        InvoiceDetailsId = x.Id,
                        IsCancelled = x.OnlinePaymentDetailInfo.IsCancelled,
                        Education = _context.Educations.First(e => e.Id == x.EducationId).Name,
                        PaidPriceNumeric = x.OnlinePaymentDetailInfo.PaidPrice,
                        PaidPriceText = x.OnlinePaymentDetailInfo.PaidPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
                    }).ToList()
                });
            }

            foreach (var val in retVal)
            {
                foreach (var item in val.InvoiceDetails)
                {
                    var ticket = userAllTickets.First(x => x.InvoiceDetailsId == item.InvoiceDetailsId);
                    var userGroup = userGroups.FirstOrDefault(x => x.TicketId == ticket.Id);
                    if (userGroup != null)
                    {
                        item.Group = new _CorrespondingGroup
                        {
                            GroupName = userGroup.Group.GroupName,
                            IsGroupStarted = userGroup.Group.StartDate <= DateTime.Now.Date,
                            StartDate = userGroup.Group.StartDate,
                            StartDateText = userGroup.Group.StartDate.ToShortDateString(),
                            TicketId = ticket.Id
                        };
                    }

                }
            }

            return retVal;


        }

        public HeaderLoggedInUserInfoVm GetHeaderLoggedInUserInfo(string userId)
        {
            var data = _context.Users.First(x => x.Id == userId);
            var studentEducationInfo = _context.StudentEducationInfos.Any(x => x.CustomerId == userId);
            var retVal = new HeaderLoggedInUserInfoVm
            {
                AvatarPath = data.AvatarPath,
                Name = data.Name,
                Surname = data.Surname,
                IsNbuyStudent = studentEducationInfo
            };
            return retVal;
        }

        //public List<MyInvoicesVm> GetUserInvoices(string userId)
        //{
        //    var invoices = _context.OnlinePaymentDetailsInfos
        //        .Include(x => x.InvoiceDetail)
        //        .ThenInclude(x => x.Invoice)
        //        .Where(x => x.InvoiceDetail.Invoice.CustomerId == userId)
        //        .Join(_context.Tickets, l => l.InvoiceDetail.Id, r => r.InvoiceDetailsId, (x, y) => new
        //        {
        //            Base = x,
        //            Ticket = y
        //        }).ToList()
        //        .GroupBy(x => x.Base)
        //        .Select(x => new
        //        {
        //            Base = x.Key,
        //            Data = x.ToList()
        //        })
        //        .ToList();

        //    var model = new List<MyInvoicesVm>();
        //    foreach (var item in invoices)
        //    {
        //        model.Add(new MyInvoicesVm
        //        {
        //            Invoice = new _Invoice
        //            {
        //                InvoiceId = item.Base.InvoiceDetail.Invoice.Id,
        //                BillingType = EnumSupport.GetDescription(item.Base.InvoiceDetail.Invoice.BillingType),
        //                CompanyInfo = item.Base.InvoiceDetail.Invoice.BillingType == CustomerType.Corporate ? new _CompanyInfo
        //                {
        //                    CompanyName = item.Base.InvoiceDetail.Invoice.CompanyName
        //                } : null,
        //                IsIndividual = item.Base.InvoiceDetail.Invoice.BillingType == CustomerType.Individual,
        //                PaymentCount = item.Base.InvoiceDetail.Invoice.PaymentCount,
        //                TransactionStatus = EnumSupport.GetDescription(item.Base.InvoiceDetail.Invoice.TransactionStatus),
        //                CreatedDate = item.Base.CreatedDate,
        //                IsEligibleToFullyCancel = true
        //            },
        //            InvoiceDetails = item.Data.Select(x => x.Base).Select(y => new _InvoiceDetail
        //            {
        //                InvoiceDetailsId = y.InvoiceDetail.Id,
        //                Education = _context.Educations.First(x => x.Id == y.InvoiceDetail.EducationId).Name,
        //                IsCancelled = y.IsCancelled,
        //                PaidPriceNumeric = y.PaidPrice,
        //                PaidPriceText = y.PaidPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
        //            }).ToList()
        //        });
        //    }

        //    var invoiceDetailsIds = new List<Guid>();
        //    foreach (var item in model)
        //        foreach (var i in item.InvoiceDetails)
        //            if (!invoiceDetailsIds.Contains(i.InvoiceDetailsId))
        //                invoiceDetailsIds.Add(i.InvoiceDetailsId);

        //    var tickets = _context.Tickets
        //        .Where(x => invoiceDetailsIds.Contains(x.InvoiceDetailsId))
        //        .ToList();

        //    var bridgeGroups = _context.Bridge_GroupStudents
        //        .Include(x => x.Group)
        //        .Where(x => tickets.Select(x => x.Id).Contains(x.TicketId))
        //        .ToList();

        //    foreach (var item in model)
        //    {
        //        foreach (var i in item.InvoiceDetails)
        //        {
        //            if (tickets.Select(x => x.InvoiceDetailsId).Contains(i.InvoiceDetailsId))
        //            {
        //                var ticket = tickets.FirstOrDefault(x => x.InvoiceDetailsId == i.InvoiceDetailsId);
        //                var bridgeGroup = bridgeGroups.FirstOrDefault(x => x.TicketId == ticket.Id);
        //                if (bridgeGroup != null)
        //                {
        //                    i.Group = new _CorrespondingGroup
        //                    {
        //                        GroupName = bridgeGroup.Group.GroupName,
        //                        IsGroupStarted = bridgeGroup.Group.StartDate.Date <= DateTime.Now.Date,
        //                        StartDate = bridgeGroup.Group.StartDate,
        //                        StartDateText = bridgeGroup.Group.StartDate.ToLongDateString(),
        //                        TicketId = bridgeGroup.TicketId
        //                    };
        //                    if (i.Group.IsGroupStarted || DateTime.Now.Date > item.Invoice.CreatedDate.Date)
        //                        item.Invoice.IsEligibleToFullyCancel = false;
        //                }
        //            }
        //        }
        //    }

        //    return model;
        //}
        public HeaderLoggedInUserDropDownVm GetHeaderLoggedInUserDropDownInfo(string userId)
        {
            var data = _context.Users.First(x => x.Id == userId);
            var customer = _context.Customers.FirstOrDefault(x => x.Id == userId);
            var studentEducationInfo = _context.StudentEducationInfos.Include(x=>x.Category).FirstOrDefault(x => x.CustomerId == userId);
            var retVal = new HeaderLoggedInUserDropDownVm()
            {
                Name = data.Name,
                Surname = data.Surname,
                EducationInfo = studentEducationInfo != null ? studentEducationInfo.Category.Name : ""
            };
            return retVal;
        }
    }


}
