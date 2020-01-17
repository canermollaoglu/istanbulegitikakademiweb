using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
        public List<CartItem> GetCartItems(List<Guid> itemIds)
        {
            var educations = _unitOfWork.Education.Get(x => itemIds.Contains(x.Id), x => x.OrderBy(o => o.Name));
            var model = educations.Select(x => new CartItem
            {
                EducationId = x.Id,
                EducationName = x.Name,
                PreviewPhoto = _unitOfWork.EducationMedia.Get(y => y.EducationId == x.Id && y.MediaType == EducationMediaType.PreviewPhoto).FirstOrDefault().FileUrl,
                PriceNumeric = x.NewPrice.GetValueOrDefault(0),
                PriceText = x.NewPrice.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
            }).ToList();

            return model;
        }
    }
}
