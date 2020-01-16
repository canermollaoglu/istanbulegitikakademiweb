using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.VmCreator;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Controllers
{
    public class SaleController : Controller
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
            var model = _vmCreator.GetCartItems(data.Items);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        public IActionResult Payment()
        {
            return View();
        }
    }

    public class GetCartItemsData
    {
        public List<Guid> Items { get; set; }
    }
}