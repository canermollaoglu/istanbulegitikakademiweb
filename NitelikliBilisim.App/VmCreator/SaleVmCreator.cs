﻿using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.Cart;
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
        public List<CartItemVm> GetCartItems(List<Guid> itemIds)
        {
            var educations = _unitOfWork.Education.Get(x => itemIds.Contains(x.Id), x => x.OrderBy(o => o.Name));
            var model = educations.Select(x => new CartItemVm
            {
                EducationId = x.Id,
                EducationName = x.Name,
                PreviewPhoto = _unitOfWork.EducationMedia.Get(y => y.EducationId == x.Id && y.MediaType == EducationMediaType.PreviewPhoto).First().FileUrl,
                PriceNumeric = x.NewPrice.GetValueOrDefault(0),
                PriceText = x.NewPrice.GetValueOrDefault(0).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"))
            }).ToList();

            return model;
        }
    }
}
