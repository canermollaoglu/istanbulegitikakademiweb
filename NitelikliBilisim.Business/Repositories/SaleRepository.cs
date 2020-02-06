using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Iyzipay.Model;
using NitelikliBilisim.Core.PaymentModels;

namespace NitelikliBilisim.Business.Repositories
{
    public class SaleRepository
    {
        private readonly NbDataContext _context;
        public SaleRepository(NbDataContext context)
        {
            _context = context;
        }
        public ApplicationUser GetUser(string userId)
        {
            return _context.Users.First(x => x.Id == userId);
        }
        public List<Guid> Sell(PayData data, List<CartItem> cartItems, string userId)
        {
            var invoiceDetails = CreateInvoiceDetails(cartItems);

            //var cartItems = new List<CartItem>();
            //for (int i = 0; i < educations.Count; i++)
            //    cartItems.Add(new CartItem
            //    {
            //        InvoiceDetailsId = invoiceDetails[i].Id,
            //        Education = educations[i]
            //    });

            _CorporateInvoiceInfo corporateInvoiceInfo = !data.InvoiceInfo.IsIndividual ? data.CorporateInvoiceInfo : null;

            var invoice = CreateInvoice(corporateInvoiceInfo: corporateInvoiceInfo,
                paymentCount: data.PaymentInfo.Installments,
                isCash: true,
                userId: userId);

            data.BasketId = invoice.Id;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    invoice.ConversationId = data.ConversationId;
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

                    return invoiceDetails.Select(x => x.Id).ToList();
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public List<CartItem> PrepareCartItems(PayData data)
        {
            var educations = _context.Educations
               .Where(x => data.CartItems.Contains(x.Id))
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

        public bool IsValidConversation(Guid determinedConversationId, ThreedsInitialize paymentResult)
        {
            if (paymentResult.Status == "success"
                && determinedConversationId.ToString() == paymentResult.ConversationId
                && paymentResult.ErrorCode == null
                && paymentResult.ErrorMessage == null
                && paymentResult.ErrorGroup == null
                && paymentResult.HtmlContent != null)
                return true;
            return false;
        }
    }
}
