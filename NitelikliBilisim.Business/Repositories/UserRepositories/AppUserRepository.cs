using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
                    Level = EnumSupport.GetDescription(x.Education.Level),
                    //PriceText = x.Education.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    HoursPerDayText = x.Education.HoursPerDay.ToString(),
                    DaysText = x.Education.Days.ToString(),
                    DaysNumeric = x.Education.Days,
                    HoursPerDayNumeric = x.Education.HoursPerDay
                },
                Medias = new List<EducationMediaVm> { new EducationMediaVm { EducationId = x.Education.Id, FileUrl = x.EducationPreviewMedia.FileUrl } },
            }).ToList();

            #endregion
            if (customer == null)
                return null;

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
                var educationDay = educationDays.Where(x => x.StudentEducationInfoId == studentEducationInfo.Id && x.Date <= DateTime.Now).OrderByDescending(c => c.Date).First();

                _educationInfo = new _EducationInfo
                {
                    EducationCategory = _context.EducationCategories.First(x => x.Id == studentEducationInfo.CategoryId).Name,
                    EducationCenter = EnumSupport.GetDescription(studentEducationInfo.EducationCenter),
                    StartedAt = studentEducationInfo.StartedAt,
                    NBUYCurrentEducationDay = educationDay.Day
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
                    HostCity = EnumSupport.GetDescription(ticket.Host.City),
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
                        BillingType = EnumSupport.GetDescription(invoice.BillingType),
                        CompanyInfo = invoice.BillingType == CustomerType.Corporate ? new _CompanyInfo { CompanyName = invoice.CompanyName } : null,
                        CreatedDate = invoice.CreatedDate,
                        InvoiceId = invoice.Id,
                        IsIndividual = invoice.BillingType == CustomerType.Individual,
                        PaymentCount = invoice.PaymentCount,
                        TransactionStatus = EnumSupport.GetDescription(invoice.TransactionStatus),
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
