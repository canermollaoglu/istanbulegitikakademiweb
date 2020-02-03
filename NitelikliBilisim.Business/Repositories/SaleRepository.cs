using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class SaleRepository
    {
        private readonly NbDataContext _context;
        public SaleRepository(NbDataContext context)
        {
            _context = context;
        }

        public void Sell(PayPostVm data, string userId, Options options)
        {
            var cartItems = _context.Educations
                .Where(x => data.CartItems.Contains(x.Id))
                .Include(x => x.Category)
                .ThenInclude(x => x.BaseCategory)
                .ToList();

            var invoiceDetails = CreateInvoiceDetails(cartItems);

            _CorporateInvoiceInfo corporateInvoiceInfo = !data.InvoiceInfo.IsIndividual ? data.CorporateInvoiceInfo : null;

            var invoice = CreateInvoice(corporateInvoiceInfo: corporateInvoiceInfo,
                paymentCount: data.Installments,
                isCash: true,
                userId: userId);

            #region OnlinePaymentService

            var conversationId = Guid.NewGuid();
            var totalPrice = invoiceDetails.Sum(x => x.PriceAtCurrentDate);
            var request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = conversationId.ToString(),
                Price = totalPrice.ToString(new CultureInfo("en-US")),
                PaidPrice = totalPrice.ToString(new CultureInfo("en-US")),
                Currency = Currency.TRY.ToString(),
                Installment = data.Installments,
                BasketId = invoice.Id.ToString(),
                PaymentChannel = data.PaymentChannel.ToString(),
                PaymentGroup = data.PaymentGroup.ToString()
            };

            var paymentCard = new PaymentCard
            {
                CardHolderName = data.CardInfo.NameOnCard,
                CardNumber = data.CardInfo.NumberOnCard,
                ExpireMonth = data.CardInfo.MonthOnCard,
                ExpireYear = data.CardInfo.YearOnCard,
                Cvc = data.CardInfo.CVC,
                RegisterCard = 0
            };
            request.PaymentCard = paymentCard;

            var user = _context.Users.First(x => x.Id == userId);
            var buyer = new Buyer
            {
                Id = userId,
                Name = user.Name,
                Surname = user.Surname,
                GsmNumber = user.PhoneNumber,
                Email = user.Email,
                IdentityNumber = data.IdentityNumber,
                //LastLoginDate = "2015-10-05 12:43:35",
                //RegistrationDate = "2013-04-21 15:12:09",
                RegistrationAddress = data.InvoiceInfo.Address,
                Ip = data.Ip,
                City = data.InvoiceInfo.City,
                Country = "Turkey",
                //ZipCode = "34732"
            };
            request.Buyer = buyer;

            var billingAddress = new Address
            {
                ContactName = data.InvoiceInfo.IsIndividual ? data.CardInfo.NameOnCard : data.CorporateInvoiceInfo.CompanyName,
                City = data.InvoiceInfo.City,
                Country = "Turkey",
                Description = data.InvoiceInfo.IsIndividual ? data.InvoiceInfo.Address : $"{data.CorporateInvoiceInfo.CompanyName} {data.CorporateInvoiceInfo.TaxNo} {data.CorporateInvoiceInfo.TaxOffice} - {data.InvoiceInfo.City}",
                //ZipCode = "34742"
            };
            request.BillingAddress = billingAddress;

            var basketItems = new List<BasketItem>();
            foreach (var cartItem in cartItems)
            {
                var basketItem = new BasketItem
                {
                    Id = cartItem.Id.ToString(),
                    Name = cartItem.Name,
                    Category1 = cartItem.Category.BaseCategory.Name,
                    Category2 = cartItem.Category.Name,
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = cartItem.NewPrice.GetValueOrDefault().ToString(new CultureInfo("en-US"))
                };
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            var paymentResult = Payment.Create(request, options);

            #endregion

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

                    var tickets = CreateTickets(invoiceDetails, new Guid("E24DA16A-269A-4C91-B9FF-C64E4ABCC031"), userId);
                    _context.AddRange(tickets);

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        private List<InvoiceDetail> CreateInvoiceDetails(List<Education> educations)
        {
            var invoiceDetails = new List<InvoiceDetail>();
            foreach (var education in educations)
            {
                invoiceDetails.Add(new InvoiceDetail
                {
                    EducationId = education.Id,
                    PriceAtCurrentDate = education.NewPrice.GetValueOrDefault()
                });
            }

            return invoiceDetails;
        }

        private Invoice CreateInvoice(_CorporateInvoiceInfo corporateInvoiceInfo, byte paymentCount, bool isCash, string userId)
        {
            var invoice = new Invoice
            {
                BillingType = CustomerType.Individual,
                CustomerId = userId,
                IsCash = isCash,
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

        private List<Ticket> CreateTickets(List<InvoiceDetail> invoiceDetails, Guid hostId, string userId)
        {
            var tickets = new List<Ticket>();
            foreach (var item in invoiceDetails)
                tickets.Add(new Ticket
                {
                    EducationId = item.EducationId,
                    HostId = hostId,
                    InvoiceDetailsId = item.Id,
                    IsUsed = false,
                    OwnerId = userId
                });

            return tickets;
        }
    }
}
