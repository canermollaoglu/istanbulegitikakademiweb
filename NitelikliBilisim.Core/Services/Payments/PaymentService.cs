using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.Core.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentOptions _option;
        public PaymentService(IConfiguration configuration)
        {
            _option = configuration.GetSection("IyzicoOptions").Get<PaymentOptions>();
        }

        public ThreedsInitialize MakePayment(PayPostVm data, ApplicationUser user, List<Education> cartItems)
        {
            var totalPrice = cartItems.Sum(x => x.NewPrice.GetValueOrDefault());
            var request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = data.ConversationId.ToString(),
                Price = totalPrice.ToString(new CultureInfo("en-US")),
                PaidPrice = totalPrice.ToString(new CultureInfo("en-US")),
                Currency = Currency.TRY.ToString(),
                Installment = data.Installments,
                BasketId = data.BasketId.ToString(),
                PaymentChannel = data.PaymentChannel.ToString(),
                PaymentGroup = data.PaymentGroup.ToString(),
                CallbackUrl = _option.ThreedsCallbackUrl
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

            var buyer = new Buyer
            {
                Id = user.Id,
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

            return ThreedsInitialize.Create(request, _option);
        }

    }
}
