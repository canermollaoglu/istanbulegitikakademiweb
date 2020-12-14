using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Cart;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.App.VmCreator
{
    public class SaleVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public SaleVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // gereksiz request sayısı
        public List<CartItemVm> GetCartItems(List<_CartItem> items)
        {
            List<CartItemVm> model = _unitOfWork.EducationGroup.GetGroupCartItems(items);
            return model;
        }
    }
}
