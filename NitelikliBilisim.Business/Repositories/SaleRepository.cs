﻿using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.Data;
using System.Collections.Generic;
using System.Linq;
using NitelikliBilisim.Core.Services.Payment;

namespace NitelikliBilisim.Business.Repositories
{
    public class SaleRepository
    {
        private readonly NbDataContext _context;
        public SaleRepository(NbDataContext context)
        {
            _context = context;
        }

        public void Sell(PayPostVm data, IPaymentService paymentService, string userId)
        {
            var invoiceDetails = CreateInvoiceDetails(
                _context.Educations
                .Where(x => data.CartItems.Contains(x.Id))
                .ToList());

            _CorporateInvoiceInfo corporateInvoiceInfo = !data.InvoiceInfo.IsIndividual ? data.CorporateInvoiceInfo : null;

            var invoice = CreateInvoice(corporateInvoiceInfo: corporateInvoiceInfo,
                paymentCount: 1,
                isCash: true,
                userId: userId);

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
                    PriceAtCurrentDate = education.NewPrice.Value
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
