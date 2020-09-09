﻿using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class SaleRepository
    {
        private readonly NbDataContext _context;
        private readonly EmailSender _emailSender;

        public SaleRepository(NbDataContext context)
        {
            _context = context;
            _emailSender = new EmailSender();
        }
        public ApplicationUser GetUser(string userId)
        {
            return _context.Users.First(x => x.Id == userId);
        }
        public List<CartItem> PrepareCartItems(PayData data)
        {
            var educations = _context.Educations
               .Where(x => data.CartItems.Select(cartItem => cartItem.EducationId).Contains(x.Id))
               .Include(x => x.Category)
               .ThenInclude(x => x.BaseCategory)
               .ToList();

            var cartItems = new List<CartItem>();

            foreach (var item in educations)
                cartItems.Add(new CartItem
                {
                    Education = item,
                    InvoiceDetailsId = Guid.NewGuid()
                });

            return cartItems;
        }
        public List<InvoiceDetail> Sell(PayData data, List<CartItem> cartItems, string userId, TransactionStatus transactionStatus = TransactionStatus.TransactionAwait)
        {
            var invoiceDetails = CreateInvoiceDetails(cartItems);

            _CorporateInvoiceInfo corporateInvoiceInfo = !data.InvoiceInfo.IsIndividual ? data.CorporateInvoiceInfo : null;

            var invoice = CreateInvoice(corporateInvoiceInfo: corporateInvoiceInfo,
                paymentCount: data.PaymentInfo.Installments,
                userId: userId,
                transactionStatus: transactionStatus);

            data.BasketId = invoice.Id;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Invoices.Add(invoice);
                    _context.InvoiceAddresses.Add(new InvoiceAddress
                    {
                        Id = invoice.Id,
                        Address = data.InvoiceInfo.Address,
                        City = data.InvoiceInfo.City,
                        County = data.InvoiceInfo.Town
                    });

                    foreach (var invoiceDetail in invoiceDetails)
                        invoiceDetail.InvoiceId = invoice.Id;
                    _context.InvoiceDetails.AddRange(invoiceDetails);

                    var tickets = CreateTickets(invoiceDetails, data.CartItems.Select(x => x.HostId).ToList(), userId);
                    _context.AddRange(tickets);

                    _context.SaveChanges();
                    transaction.Commit();

                    return invoiceDetails.ToList();
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
        public void CompletePayment(PaymentCompletionModel completionModel, Guid invoiceId, List<Guid> invoiceDetailsIds)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var invoice = _context.Invoices.Find(invoiceId);
                invoice.TransactionStatus = TransactionStatus.TransactionSuccess;
                var onlinePaymentInfo = new OnlinePaymentInfo
                {
                    Id = invoice.Id,
                    PaymentId = completionModel.Invoice.PaymentId,
                    BinNumber = completionModel.Invoice.BinNumber,
                    CommissionRate = completionModel.Invoice.CommissionRate,
                    CommissonFee = completionModel.Invoice.CommissonFee,
                    HostRef = completionModel.Invoice.HostRef,
                    LastFourDigit = completionModel.Invoice.LastFourDigit,
                    PaidPrice = completionModel.Invoice.PaidPrice,
                };
                _context.OnlinePaymentInfos.Add(onlinePaymentInfo);

                var onlinePaymentDetailInfos = new List<OnlinePaymentDetailsInfo>();
                for (int i = 0; i < completionModel.InvoiceDetails.Count; i++)
                {
                    var item = completionModel.InvoiceDetails[i];
                    onlinePaymentDetailInfos.Add(new OnlinePaymentDetailsInfo
                    {
                        Id = invoiceDetailsIds[i],
                        TransactionId = item.TransactionId,
                        CommisionRate = item.CommisionRate,
                        CommissionFee = item.CommissionFee,
                        MerchantPayout = item.MerchantPayout,
                        PaidPrice = item.PaidPrice,
                        Price = item.Price,
                        BlockageResolveDate = item.BlockageDate
                    });
                }
                _context.OnlinePaymentDetailsInfos.AddRange(onlinePaymentDetailInfos);

                _context.SaveChanges();
                transaction.Commit();
                Auto__AssignTickets(invoiceDetailsIds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                transaction.Rollback();
            }
            
        }
        public bool GetIfCustomerEligibleToFullyCancel(Guid ticketId)
        {
            var groupStartDate = _context.Bridge_GroupStudents.Include(x=>x.Group).First(x => x.TicketId == ticketId).Group.StartDate;
            var ticket = _context.Tickets.First(x => x.Id == ticketId);
            var invoiceDate = _context.InvoiceDetails.Include(x => x.Invoice).First(x => x.Id == ticket.InvoiceDetailsId).Invoice.CreatedDate;
            var isGroupStarted = groupStartDate <= DateTime.Now.Date;

            return !(isGroupStarted || DateTime.Now.Date > invoiceDate.Date);
        }
        public void RefundPayment(Guid invoiceDetailId)
        {
            var onlinePaymentDetailsInfo = _context.OnlinePaymentDetailsInfos.First(x => x.Id == invoiceDetailId);
            onlinePaymentDetailsInfo.IsCancelled = true;
            onlinePaymentDetailsInfo.CancellationDate = DateTime.Now;
            _context.SaveChanges();
            Auto__UnassignTickets(new List<Guid>() { invoiceDetailId });
        }
        public void CancelPayment(Guid invoiceId)
        {
            var invoiceDetailsIds = _context.InvoiceDetails.Where(x => x.InvoiceId == invoiceId).Select(x => x.Id).ToList();
            var onlinePaymentDetails = _context.OnlinePaymentDetailsInfos.Where(x => invoiceDetailsIds.Contains(x.Id));

            foreach (var item in onlinePaymentDetails)
            {
                item.IsCancelled = true;
                item.CancellationDate = DateTime.Now;
            }
            _context.SaveChanges();
            Auto__UnassignTickets(invoiceDetailsIds);
        }
        private List<InvoiceDetail> CreateInvoiceDetails(List<CartItem> cartItems)
        {
            var invoiceDetails = new List<InvoiceDetail>();
            foreach (var cartItem in cartItems)
            {
                invoiceDetails.Add(new InvoiceDetail
                {
                    Id = cartItem.InvoiceDetailsId,
                    EducationId = cartItem.Education.Id,
                    PriceAtCurrentDate = cartItem.Education.NewPrice.GetValueOrDefault()
                });
            }

            return invoiceDetails;
        }
        private Invoice CreateInvoice(_CorporateInvoiceInfo corporateInvoiceInfo, byte paymentCount, string userId, TransactionStatus transactionStatus = TransactionStatus.TransactionAwait)
        {
            var invoice = new Invoice
            {
                BillingType = CustomerType.Individual,
                TransactionStatus = transactionStatus,

                CustomerId = userId,
                PaymentCount = paymentCount,
            };

            if (corporateInvoiceInfo != null)
            {
                invoice.BillingType = CustomerType.Corporate;
                invoice.CompanyName = corporateInvoiceInfo.CompanyName;
                invoice.TaxNo = corporateInvoiceInfo.TaxNo;
                invoice.TaxOffice = corporateInvoiceInfo.TaxOffice;
            }

            return invoice;
        }
        private List<Ticket> CreateTickets(List<InvoiceDetail> invoiceDetails, List<Guid> hostIds, string userId)
        {
            var tickets = new List<Ticket>();
            for (var i = 0; i < invoiceDetails.Count; i++)
            {
                var item = invoiceDetails[i];
                tickets.Add(new Ticket
                {
                    EducationId = item.EducationId,
                    HostId = hostIds[i],
                    InvoiceDetailsId = item.Id,
                    IsUsed = false,
                    OwnerId = userId
                });
            }

            return tickets;
        }
        private bool Auto__AssignTickets(List<Guid> invoiceDetailsIds)
        {
            try
            {
                var tickets = _context.Tickets.Include(x=>x.Education).Include(x => x.Owner).ThenInclude(x => x.User)
                    .Where(x => invoiceDetailsIds.Contains(x.InvoiceDetailsId))
                    .ToList();

                foreach (var ticket in tickets)
                {
                    var firstGroup = _context.EducationGroups
                        .Where(x => x.StartDate.Date > DateTime.Now.Date
                        && x.EducationId == ticket.EducationId
                        && x.IsGroupOpenForAssignment
                        && x.HostId == ticket.HostId)
                        .OrderBy(o => o.StartDate)
                        .FirstOrDefault();
                    if (firstGroup == null)
                    {
                        Task.Run(()=> _emailSender.SendAsync(new EmailMessage
                        {
                            Subject = "Grup Atamanız Yapılamamıştır | Nitelikli Bilişim",
                            Body = $"{ticket.Education.Name} eğitimine yönelik grupların kontenjanları dolduğu için gruba atamanız yapılamamıştır.",
                            Contacts = new string[]{ ticket.Owner.User.Email }
                        }));
                        return false;
                    }

                    _context.Bridge_GroupStudents.Add(new Bridge_GroupStudent
                    {
                        Id = firstGroup.Id,
                        Id2 = ticket.OwnerId,
                        TicketId = ticket.Id
                    });
                    //grup kontenjanının dolması durumunda IsGroupOpenForAssigment false olarak değiştiriliyor.
                    var groupStudentsCount = _context.Bridge_GroupStudents.Count(x => x.Id == firstGroup.Id) + 1;
                    ticket.IsUsed = true;
                    if (groupStudentsCount == firstGroup.Quota)
                        firstGroup.IsGroupOpenForAssignment = false;
                    _context.SaveChanges();

                    Task.Run(()=> _emailSender.SendAsync(new EmailMessage
                    {
                        Subject = "Grup Atamanız Yapılmıştır | Nitelikli Bilişim",
                        Body = $"{ticket.Education.Name} eğitimi için {firstGroup.StartDate.ToShortDateString()} tarihinde başlayacak olan {firstGroup.GroupName} grubuna atamanız yapılmıştır.",
                        Contacts = new string[] { ticket.Owner.User.Email }
                    }));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Auto__UnassignTickets(List<Guid> invoiceDetailsIds)
        {
            try
            {
                var tickets = _context.Tickets.Include(x => x.Education).Include(x => x.Owner).ThenInclude(x => x.User)
                    .Where(x => invoiceDetailsIds.Contains(x.InvoiceDetailsId))
                    .ToList();
                var bridges = _context.Bridge_GroupStudents
                    .Where(x => tickets.Select(x => x.Id).Contains(x.TicketId))
                    .ToList();
                var groups = _context.EducationGroups.Where(x => bridges.Select(s => s.Id).Contains(x.Id));

                foreach (var bridge in bridges)
                {
                    var group = groups.First(x => x.Id == bridge.Id);
                    group.IsGroupOpenForAssignment = true;
                    _context.EducationGroups.Update(group);
                }
                foreach (var ticket in tickets)
                {
                    
                    ticket.IsUsed = false;
                   Task.Run(()=> _emailSender.SendAsync(new EmailMessage
                    {
                        Subject = "Gruptan Ayrıldınız | Nitelikli Bilişim",
                        Body = $"{ticket.Education.Name} eğitimini iptal ettiğiniz için atandığınız gruptan ayrıldınız.",
                        Contacts = new string[] { ticket.Owner.User.Email }
                    }));
                }
                _context.Bridge_GroupStudents.RemoveRange(bridges);
                _context.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
