using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationPromotionCodeRepository : BaseRepository<EducationPromotionCode, Guid>
    {
        private readonly NbDataContext _context;
        public EducationPromotionCodeRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<EducationPromotionCodeListVm> GetPromotionCodeList()
        {
            var data = (from promotionCode in _context.EducationPromotionCodes
                        select new EducationPromotionCodeListVm
                        {
                            Id= promotionCode.Id,
                            StartDate = promotionCode.StartDate,
                            EndDate = promotionCode.EndDate,
                            PromotionCode = promotionCode.PromotionCode,
                            Description = promotionCode.Description,
                            MaxUsageLimit = promotionCode.MaxUsageLimit,
                            DiscountAmount = promotionCode.DiscountAmount,
                            MinBasketAmount = promotionCode.MinBasketAmount,
                            IsActive = DateTime.Now.Date < promotionCode.EndDate && DateTime.Now >= promotionCode.StartDate ? true : false
                        });

            return data;
        }
    }
}
