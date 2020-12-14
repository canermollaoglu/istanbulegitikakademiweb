using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.promotion;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.promotion;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class AppUserRepository
    {
        private readonly NbDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public AppUserRepository(NbDataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            return model;
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
               .Join(_context.EducationMedias.Where(x => x.MediaType == EducationMediaType.PreviewPhoto), l => l.Id, r => r.EducationId, (x, y) => new
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
                Job = customer.Job,
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

        public ForYouPageGetVm GetForYouPageData(string userId)
        {
            var infos = _context.Customers
                .Include(x=>x.StudentEducationInfos)
                .ThenInclude(x=>x.Category)
                .Include(x => x.StudentEducationInfos)
                .ThenInclude(x => x.EducationDays)
                .First(x=>x.Id == userId);
            var nbuyInfo = infos.StudentEducationInfos[0];
            var totalEducationWeeks = (int)Math.Ceiling((nbuyInfo.EducationDays.OrderBy(x=>x.Day).Last().Date - nbuyInfo.StartedAt.Date).TotalDays / (double)7);
            var currentEducationWeek = (int)Math.Ceiling((DateTime.Now.Date - nbuyInfo.StartedAt).TotalDays / (double)7); //GetEducationWeek(userId);
            ForYouPageGetVm model = new();
            model.EducationCategory = nbuyInfo.Category.Name;
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
                         where eImage.MediaType == EducationMediaType.PreviewPhoto && comment.CommentatorId == userId
                         select new MyCommentVm
                         {
                             Id = comment.Id,
                             EducationId = education.Id,
                             Content = comment.Content,
                             Point = comment.Points,
                             Date = comment.CreatedDate.ToString("dd MMMM yyyy"),
                             EducationName = education.Name,
                             EducationFeaturedImage = eImage.FileUrl,
                             CategoryName = category.Name,
                             ApprovalStatus = comment.ApprovalStatus,
                         }).ToList();
            return model;
        }

        public MyCourseDetailVm GetPurschasedEducationDetail(string userId, Guid groupId)
        {
            var model = (from eGroup in _context.EducationGroups
                         join education in _context.Educations on eGroup.EducationId equals education.Id
                         join educator in _context.Educators on eGroup.EducatorId equals educator.Id
                         join host in _context.EducationHosts on eGroup.HostId equals host.Id
                         join educatorUser in _context.Users on educator.Id equals educatorUser.Id
                         join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                         where eImage.MediaType == EducationMediaType.PreviewPhoto
                         && eGroup.Id == groupId
                         select new MyCourseDetailVm
                         {
                             EducationId = education.Id,
                             EducationName = education.Name,
                             EducationShortDescription = education.Description,
                             EducationFeaturedImage = eImage.FileUrl,
                             EducatorId = educator.Id,
                             EducatorName = $"{educatorUser.Name} {educatorUser.Surname}",
                             EducatorTitle = educator.Title,
                             EducatorAvatarPath = educatorUser.AvatarPath,
                             PriceText = eGroup.NewPrice,
                             Days = education.Days.ToString(),
                             Hours = (education.Days * education.HoursPerDay).ToString(),
                             Host = host.HostName
                         }).FirstOrDefault();
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
            var purchasedEducations = _context.Bridge_GroupStudents.Where(x => x.Id2 == userId);
            var wishListItems = _context.Wishlist.Where(x => x.Id == userId).Select(x => x.Id2).ToList();

            List<Guid> ids = purchasedEducations.Select(x => x.Id).ToList();
            var educationList = (from eGroup in _context.EducationGroups
                                 join education in _context.Educations on eGroup.EducationId equals education.Id
                                 join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                                 where eImage.MediaType == EducationMediaType.PreviewPhoto
                                  && ids.Contains(eGroup.Id)
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
            var student = _context.Customers.Include(x => x.User).First(x => x.Id == userId);
            var favoriteEducations = GetUserFavoriteEducationsByUserId(userId);
            var purchasedEducations = GetPurschasedEducationsByUserId(userId);

            model.FavoriteEducations = favoriteEducations;
            model.FavoriteEducationCount = favoriteEducations.Count;
            model.PurchasedEducations = purchasedEducations;
            model.PurchasedEducationCount = purchasedEducations.Count;
            if (student.IsNbuyStudent)
            {
                model.EducationWeek = GetEducationWeek(userId);
            }
            return model;

        }

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
                                 where eImage.MediaType == EducationMediaType.PreviewPhoto
                                 && ids.Contains(eGroup.Id)
                                 select new PurchasedEducationVm
                                 {
                                     EducationId = education.Id,
                                     GroupId = eGroup.Id,
                                     Name = education.Name,
                                     CategoryName = category.Name,
                                     City = EnumHelpers.GetDescription(host.City),
                                     FeaturedImageUrl = eImage.FileUrl,
                                     EducatorImageUrl = educatorUser.AvatarPath,
                                 }
                ).ToList();
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
            var wishListItems = _context.Wishlist.Where(x => x.Id == userId).ToList();
            List<Guid> wishListEducationIds = wishListItems.Select(x => x.Id2).ToList();
            var educationsList = (from education in _context.Educations
                                  join featuredImage in _context.EducationMedias on education.Id equals featuredImage.EducationId
                                  join category in _context.EducationCategories on education.CategoryId equals category.Id
                                  where featuredImage.MediaType == EducationMediaType.PreviewPhoto
                                  && wishListEducationIds.Contains(education.Id)
                                  select new FavoriteEducationVm
                                  {
                                      Id = education.Id,
                                      Name = education.Name,
                                      CategoryName = category.Name,
                                      HoursText = (education.HoursPerDay * education.Days).ToString(),
                                      DaysText = education.Days.ToString(),
                                      FeaturedImageUrl = featuredImage.FileUrl
                                  }).ToList();
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
        #region Test
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
        #endregion
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
    }


}
