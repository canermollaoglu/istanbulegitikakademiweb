using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.Main.Sales;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class InvoiceDetailRepository : BaseRepository<InvoiceDetail,Guid>
    {
        private readonly NbDataContext _context;
        public InvoiceDetailRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public InvoiceDetail GetByIdWithOnlinePaymentDetailInfo(Guid invoiceDetailId)
        {
            return _context.InvoiceDetails.Include(x=>x.Invoice).Include(x => x.OnlinePaymentDetailInfo).First(x => x.Id == invoiceDetailId);
        }

        public PaymentSuccessDetailVm GetSuccessPaymentDetails(List<Guid> invoiceDetailIds, Guid? promotionId)
        {
            PaymentSuccessDetailVm model = new();
            decimal promotionDiscountAmount = 0;
            if (promotionId.HasValue)
            {
              var promotion =  _context.EducationPromotionCodes.FirstOrDefault(x => x.Id == promotionId);
              promotionDiscountAmount = promotion!=null? promotion.DiscountAmount:0;
            }
            var cultureInfo = CultureInfo.CreateSpecificCulture("tr-TR");
            var details = _context.InvoiceDetails.Include(x => x.Invoice).Where(x => invoiceDetailIds.Contains(x.Id));
            var groups = _context.EducationGroups.Include(x => x.Education).Where(x => details.Any(y => y.GroupId == x.Id)).ToList();
            var totalOldPrice = groups.Sum(x => x.OldPrice.Value);
            var totalNewPrice = groups.Sum(x => x.NewPrice.Value);
            var invoice = details.ToList()[0].Invoice;
            model.Installment = invoice.PaymentCount==1?"Tek Çekim":$"{invoice.PaymentCount} Taksit";
            model.TotalOldPrice = totalOldPrice.ToString(cultureInfo);
            model.TotalDiscount = (promotionDiscountAmount+(totalOldPrice-totalNewPrice)).ToString(cultureInfo);
            model.TotalNewPrice = (details.Sum(x => x.PriceAtCurrentDate)-promotionDiscountAmount).ToString(cultureInfo);

            var iDetails = (from eGroup in _context.EducationGroups
                           join education in _context.Educations on eGroup.EducationId equals education.Id
                           join category in _context.EducationCategories on education.CategoryId equals category.Id
                           join eImage in _context.EducationMedias on education.Id equals eImage.EducationId
                           where details.Any(x => eGroup.Id == x.GroupId) && eImage.MediaType == Core.Enums.EducationMediaType.Square
                           select new PaymentSuccessGroupDetailVm
                           {
                               ImagePath = eImage.FileUrl,
                               EducationName = education.Name,
                               Price = eGroup.OldPrice.GetValueOrDefault().ToString(cultureInfo),
                               SeoUrl = education.SeoUrl,
                               CategorySeoUrl = category.SeoUrl
                           }).ToList();

            model.InvoiceDetails = iDetails;
            return model;
        }
    }
}
