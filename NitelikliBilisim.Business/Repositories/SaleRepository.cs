using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class SaleRepository
    {
        private readonly NbDataContext _context;
        private readonly IMessageService _emailSender;
        private readonly IConfiguration _configuration;

        public SaleRepository(NbDataContext context, IMessageService emailSender,IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        public ApplicationUser GetUser(string userId)
        {
            return _context.Users.First(x => x.Id == userId);
        }
        public List<CartItem> PrepareCartItems(PayData data)
        {
            var groups = _context.EducationGroups
               .Include(x=>x.Education)
               .ThenInclude(x=>x.Category)
               .ThenInclude(x=>x.BaseCategory)
               .Where(x => data.CartItems.Select(cartItem => cartItem.GroupId).Contains(x.Id))
               .ToList();

            var cartItems = new List<CartItem>();

            foreach (var item in groups)
                cartItems.Add(new CartItem
                {
                    EducationGroup = item,
                    InvoiceDetailsId = Guid.NewGuid()
                });

            return cartItems;
        }
        public List<InvoiceDetail> Sell(PayData data, List<CartItem> cartItems, string userId, TransactionStatus transactionStatus = TransactionStatus.TransactionAwait)
        {
            var invoiceDetails = CreateInvoiceDetails(cartItems);

            Address corporateInvoiceInfo = data.InvoiceAddress.AddressType == AddressTypes.Corporate ? data.InvoiceAddress : null;

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
                        Address = data.InvoiceAddress.Content,
                        City = data.InvoiceAddress.City.Name,
                        County = data.InvoiceAddress.State.Name
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
                        BlockageResolveDate = CalculateTransferDate(DateTime.Now.Date)
                    });
                }
                _context.OnlinePaymentDetailsInfos.AddRange(onlinePaymentDetailInfos);

                _context.SaveChanges();
                Auto__AssignTickets(invoiceDetailsIds);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                transaction.Rollback();
            }
            
        }

        private DateTime CalculateTransferDate(DateTime paymentDate)
        {
            switch (paymentDate.DayOfWeek)
            {
                case DayOfWeek.Monday: return paymentDate.AddDays(9).AddHours(17);
                case DayOfWeek.Tuesday: return paymentDate.AddDays(8).AddHours(17);
                case DayOfWeek.Wednesday: return paymentDate.AddDays(7).AddHours(17);
                case DayOfWeek.Thursday: return paymentDate.AddDays(6).AddHours(17);
                case DayOfWeek.Friday: return paymentDate.AddDays(5).AddHours(17);
                case DayOfWeek.Saturday: return paymentDate.AddDays(4).AddHours(17);
                case DayOfWeek.Sunday: return paymentDate.AddDays(3).AddHours(17);
                default: return paymentDate.AddDays(9).AddHours(17);
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
        public void RefundPayment(Guid invoiceDetailId,decimal refundPrice)
        {
            var onlinePaymentDetailsInfo = _context.OnlinePaymentDetailsInfos.First(x => x.Id == invoiceDetailId);
            onlinePaymentDetailsInfo.IsCancelled = true;
            onlinePaymentDetailsInfo.CancellationDate = DateTime.Now;
            onlinePaymentDetailsInfo.RefundPrice = refundPrice;
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
                item.RefundPrice = item.PaidPrice;
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
                    EducationId = cartItem.EducationGroup.Education.Id,
                    GroupId = cartItem.EducationGroup.Id,
                    PriceAtCurrentDate = cartItem.EducationGroup.NewPrice.GetValueOrDefault()
                });
            }

            return invoiceDetails;
        }
        private Invoice CreateInvoice(Address corporateInvoiceInfo, byte paymentCount, string userId, TransactionStatus transactionStatus = TransactionStatus.TransactionAwait)
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
                invoice.TaxNo = corporateInvoiceInfo.TaxNumber;
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
                    OwnerId = userId,
                    GroupId = item.GroupId
                });
            }

            return tickets;
        }
        private void Auto__AssignTickets(List<Guid> invoiceDetailsIds)
        {
                string[] adminEmails = GetAdminEmails();
                var tickets = _context.Tickets.Include(x=>x.Education).Include(x => x.Owner).ThenInclude(x => x.User)
                    .Where(x => invoiceDetailsIds.Contains(x.InvoiceDetailsId))
                    .ToList();
                var user = _context.Users.First(x => x.Id == tickets[0].OwnerId);
            try
            {
                foreach (var ticket in tickets)
                {
                    var firstGroup = _context.EducationGroups
                        .Where(x => x.StartDate.Date > DateTime.Now.Date
                        && x.Id == ticket.GroupId
                        && x.IsGroupOpenForAssignment)
                        .FirstOrDefault();

                    if (firstGroup == null)
                    {
                        var message = new EmailMessage
                        {
                            Subject = $"{user.Name} {user.Surname} Grup Atamanız Yapılamamıştır | Nitelikli Bilişim",
                            Body = $"{user.Name} {user.Surname} kişisinin satınalımı sonucu {ticket.Education.Name} eğitimine ait grupların kontenjanları dolduğu için gruba ataması yapılamamıştır.",
                            Contacts = adminEmails
                        };
                        Task.Run(() => _emailSender.SendAsync(JsonConvert.SerializeObject(message)));
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
                    var emailMessage = new EmailMessage
                    {
                        Subject = $"{user.Name} {user.Surname} Grup Ataması Yapılmıştır | Nitelikli Bilişim",
                        Body = $"{user.Name} {user.Surname} kişisinin {ticket.Education.Name} eğitimi için {firstGroup.StartDate.ToShortDateString()} tarihinde başlayacak olan {firstGroup.GroupName} grubuna ataması yapılmıştır.",
                        Contacts = adminEmails
                    };
                    Task.Run(() => _emailSender.SendAsync(JsonConvert.SerializeObject(emailMessage)));
                    if (groupStudentsCount >= firstGroup.Quota - 3)
                    {
                        var infoEmailMessage = new EmailMessage
                        {
                            Subject = "Grup Kontenjan Bilgisi | Nitelikli Bilişim",
                            Body = $"{firstGroup.GroupName} Grup kontenjanının dolması için {firstGroup.Quota - groupStudentsCount} kayıt kalmıştır.",
                            Contacts = adminEmails
                        };
                        Task.Run(() => _emailSender.SendAsync(JsonConvert.SerializeObject(infoEmailMessage)));
                    }
                }
            }
            catch
            {
                var errorEmailMessage = new EmailMessage
                {
                    Subject = $"{user.Name} {user.Surname} Grup Ataması Yapılamamıştır!! | Nitelikli Bilişim",
                    Body = $"{user.Name} {user.Surname} kişisinin satın aldığı eğitim/ler için grup ataması yapılamamıştır.",
                    Contacts = adminEmails
                };
                Task.Run(() => _emailSender.SendAsync(JsonConvert.SerializeObject(errorEmailMessage)));
            }
                
        }
        
        private bool Auto__UnassignTickets(List<Guid> invoiceDetailsIds)
        {
            try
            {
                var adminEmails = GetAdminEmails();
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
                    var unassignEmailMessage = new EmailMessage
                    {
                        Subject = "Gruptan Ayrılma İşlemi | Nitelikli Bilişim",
                        Body = $"{ticket.Owner.User.Name} {ticket.Owner.User.Name} ({ticket.Owner.User.Email}) {ticket.Education.Name} eğitimi {groups.First(x => x.Id == ticket.GroupId).GroupName} grubundan ayrılmıştır.",
                        Contacts = adminEmails
                    };
                   Task.Run(()=> _emailSender.SendAsync(JsonConvert.SerializeObject(unassignEmailMessage)));
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
        public string[] GetAdminEmails()
        {
            return _configuration.GetSection("SiteGeneralOptions").GetSection("AdminEmails").Value.Split(";");
        }
    }
}
