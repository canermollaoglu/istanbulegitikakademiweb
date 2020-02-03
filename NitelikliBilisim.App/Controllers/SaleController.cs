﻿using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.VmCreator;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Cart;
using System;
using System.Collections.Generic;
using NitelikliBilisim.App.Controllers.Base;
using System.Globalization;
using NitelikliBilisim.Core.ViewModels.Sales;
using NitelikliBilisim.App.Utility;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace NitelikliBilisim.App.Controllers
{
    public class SaleController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly SaleVmCreator _vmCreator;
        public SaleController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new SaleVmCreator(_unitOfWork);
        }

        [Route("sepet")]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost, IgnoreAntiforgeryToken, Route("get-cart-items")]
        public IActionResult GetCartItems(GetCartItemsData data)
        {
            if (data == null || data.Items == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    data = new
                    {
                        items = new List<CartItem>(),
                        total = 0m.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
                    }
                });

            var cartItems = _vmCreator.GetCartItems(data.Items);

            var sum = 0m;

            if (cartItems.Count > 0)
                cartItems.ForEach(x => sum += x.PriceNumeric);

            var model = new
            {
                items = cartItems,
                total = sum.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
            };

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("odeme")]
        public IActionResult Payment()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/giris-yap?returnUrl=/odeme");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("pay")]
        public IActionResult Pay(PayPostVm data)
        {
            if (!HttpContext.User.Identity.IsAuthenticated || data.CartItemsJson == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sepette ürün bulunmamaktadır" }
                });

            data.CartItems = JsonConvert.DeserializeObject<List<Guid>>(data.CartItemsJson);

            if (data.CartItems == null || data.CartItems.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Sepette ürün bulunmamaktadır" }
                });
            if (!data.IsDistantSalesAgreementConfirmed)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Mesafeli Satış sözleşmesini onaylayınız" }
                });
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });
            if (!data.InvoiceInfo.IsIndividual && data.CorporateInvoiceInfo == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "?" }
                });

            _unitOfWork.Sale.Sell(data, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }

    public class GetCartItemsData
    {
        public List<Guid> Items { get; set; }
    }
}