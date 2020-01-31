using Microsoft.AspNetCore.Mvc;
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
                    errors = ModelStateUtil.GetErrors(ModelState).ToList()
                });

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