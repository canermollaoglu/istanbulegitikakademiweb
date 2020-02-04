using Iyzipay.Model;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
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

        public ThreedsInitialize Sell(PayPostVm data, string userId, IPaymentService paymentService, out PayPostVm dataResult)
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

            data.BasketId = invoice.Id;

            #region OnlinePaymentService

            var user = _context.Users.First(x => x.Id == userId);

            var paymentResult = paymentService.Make3DsPayment(data, user, cartItems);
            dataResult = data;
            #endregion

            if (IsValidConversation(data.ConversationId, paymentResult))
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
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }

            return paymentResult;
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
