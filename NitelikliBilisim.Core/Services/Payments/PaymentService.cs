using System;
using Iyzipay.Model;
using Iyzipay.Request;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.ViewModels.Sales;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace NitelikliBilisim.Core.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentOptions _options;
        public PaymentService(IConfiguration configuration)
        {
            var section = configuration.GetSection(PaymentOptions.Key);
            _options = new PaymentOptions()
            {
                ApiKey = section["ApiKey"],
                SecretKey = section["SecretKey"],
                BaseUrl = section["BaseUrl"],
                ThreedsCallbackUrl = section["ThreedsCallbackUrl"],
            };
            Console.WriteLine();
        }
        /// <summary>
        /// Ortak ayarlamaların yapıldığı method!
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        private CreatePaymentRequest InitDefaultRequest(PayData data, ApplicationUser user, List<CartItem> cartItems)
        {
            var totalPrice = 0.0m;
            foreach (var item in cartItems)
                totalPrice += item.EducationGroup.NewPrice.GetValueOrDefault();
            decimal paidPrice = totalPrice - data.DiscountAmount;

            var request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = data.ConversationId.ToString(),
                Price = totalPrice.ToString(new CultureInfo("en-US")),
                PaidPrice = paidPrice.ToString(new CultureInfo("en-US")),
                Currency = Currency.TRY.ToString(),
                Installment = data.PaymentInfo.Installments,
                BasketId = data.BasketId.ToString(),
                PaymentChannel = data.PaymentInfo.PaymentChannel.ToString(),
                PaymentGroup = data.PaymentInfo.PaymentGroup.ToString()
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
                IdentityNumber = data.SpecialInfo.IdentityNumber,
                //LastLoginDate = "2015-10-05 12:43:35",
                //RegistrationDate = "2013-04-21 15:12:09",
                RegistrationAddress = data.InvoiceInfo.Address,
                Ip = data.SpecialInfo.Ip,
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
                    Id = cartItem.InvoiceDetailsId.ToString(),
                    Name = cartItem.EducationGroup.Education.Name,
                    Category1 = cartItem.EducationGroup.Education.Category.BaseCategory.Name,
                    Category2 = cartItem.EducationGroup.Education.Category.Name,
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = cartItem.EducationGroup.NewPrice.GetValueOrDefault().ToString(new CultureInfo("en-US"))
                };
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            return request;
        }

        public InstallmentInfo CheckInstallment(string conversationId, string binNumber, decimal price)
        {
            var request = new RetrieveInstallmentInfoRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = conversationId,
                BinNumber = binNumber,
                Price = price.ToString(new CultureInfo("en-US")),
            };

            return InstallmentInfo.Retrieve(request, _options);
        }
        public Payment MakePayment(PayData data, ApplicationUser user, List<CartItem> cartItems)
        {
            var request = this.InitDefaultRequest(data, user, cartItems);
            return Payment.Create(request, _options);
        }
        public Payment CheckPayment(RetrievePaymentRequest request) => Payment.Retrieve(request, _options);

        #region 3D Security

        public ThreedsInitialize Make3DsPayment(PayData data, ApplicationUser user, List<CartItem> cartItems)
        {
            var request = this.InitDefaultRequest(data, user, cartItems);
            request.CallbackUrl = _options.ThreedsCallbackUrl;

            return ThreedsInitialize.Create(request, _options);
        }

        public ThreedsPayment Confirm3DsPayment(CreateThreedsPaymentRequest request) => ThreedsPayment.Create(request, _options);


        #endregion

        public Cancel CreateCancelRequest(string conversationId, string paymentId, string ip, RefundReason reason, string description)
        {
            CreateCancelRequest request = new CreateCancelRequest
            {
                ConversationId = conversationId,
                Locale = Locale.TR.ToString(),
                PaymentId = paymentId,
                Ip = ip,
                Reason = reason.ToString(),
                Description = description
            };

            return Cancel.Create(request, _options);
        }

        public Refund CreateRefundRequest(string conversationId, string paymentTransactionId, decimal price, string ip, RefundReason reason, string description)
        {
            CreateRefundRequest request = new CreateRefundRequest
            {
                ConversationId = conversationId,
                Locale = Locale.TR.ToString(),
                PaymentTransactionId = paymentTransactionId,
                Price = price.ToString(new CultureInfo("en-US")),
                Ip = ip,
                Currency = Currency.TRY.ToString(),
                Reason = reason.ToString(),
                Description = description
            };

            return Refund.Create(request, _options);
        }

        #region BKM ödeme

        public BkmInitialize MakeBkmPayment(PayData data, ApplicationUser user, List<CartItem> cartItems)
        {
            var r1 = this.InitDefaultRequest(data, user, cartItems);
            var request = new CreateBkmInitializeRequest()
            {
                Locale = r1.Locale,
                ConversationId = r1.ConversationId,
                Price = r1.Price,
                BasketId = r1.BasketId,
                PaymentSource = r1.PaymentSource,
                PaymentGroup = r1.PaymentGroup,
                Buyer = r1.Buyer,
                BillingAddress = r1.BillingAddress,
                BasketItems = r1.BasketItems,
                CallbackUrl = _options.ThreedsCallbackUrl
            };
            return BkmInitialize.Create(request, _options);
        }

        public Bkm ConfirmBkmPayment(RetrieveBkmRequest request) => Bkm.Retrieve(request, _options);

        #endregion

        #region Hazır Ödeme Formu

        public CheckoutFormInitialize MakeCheckoutForm(PayData data, ApplicationUser user, List<CartItem> cartItems)
        {
            var r1 = this.InitDefaultRequest(data, user, cartItems);
            var request = new CreateCheckoutFormInitializeRequest()
            {
                Locale = r1.Locale,
                ConversationId = r1.ConversationId,
                Price = r1.Price,
                PaidPrice = r1.PaidPrice,
                BasketId = r1.BasketId,
                PaymentGroup = r1.PaymentGroup,
                PaymentSource = r1.PaymentSource,
                Buyer = r1.Buyer,
                BillingAddress = r1.BillingAddress,
                BasketItems = r1.BasketItems,
                CallbackUrl = _options.ThreedsCallbackUrl
            };
            var enabledInstallments = new List<int> { 2, 3, 6, 9 };
            request.EnabledInstallments = enabledInstallments;

            return CheckoutFormInitialize.Create(request, _options);
        }

        public CheckoutForm ConfirmCheckoutForm(RetrieveCheckoutFormRequest request) => CheckoutForm.Retrieve(request, _options);

        #endregion
    }
}
