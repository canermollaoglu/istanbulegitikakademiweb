using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion;
using NitelikliBilisim.Core.ViewModels.Main.Cart;
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

            return _context.EducationPromotionCodes.Include(x => x.EducationPromotionItems)
                 .Select(promotionCode => new EducationPromotionCodeListVm
                 {
                     Id = promotionCode.Id,
                     Name = promotionCode.Name,
                     StartDate = promotionCode.StartDate,
                     EndDate = promotionCode.EndDate,
                     PromotionCode = promotionCode.PromotionCode,
                     Description = promotionCode.Description,
                     MaxUsageLimit = promotionCode.MaxUsageLimit,
                     DiscountAmount = promotionCode.DiscountAmount,
                     MinBasketAmount = promotionCode.MinBasketAmount,
                     IsActive = DateTime.Now.Date < promotionCode.EndDate && DateTime.Now >= promotionCode.StartDate ? "Aktif" : "Pasif",
                     CountOfUses = promotionCode.EducationPromotionItems.Count
                 });
        }

        public bool CheckThePromotionItem(Guid promotionId)
        {
            if (_context.EducationPromotionItems.Any(x => x.EducationPromotionCodeId == promotionId))
            {
                return true;
            }
            return false;
        }

        public PromotionCodeVm GetPromotionInfo(string promotionCode)
        {
            var code = _context.EducationPromotionCodes.FirstOrDefault(x => x.PromotionCode == promotionCode);
            if (code != null)
            {
                return new PromotionCodeVm
                {
                    Name = code.Name,
                    PromotionCode = code.PromotionCode,
                    DiscountAmount = code.DiscountAmount
                };
            }
            else
            {
                throw new Exception();
            }

        }

        public EducationPromotionCode GetPromotionbyPromotionCode(string promotionCode,string userId)
        {
            try
            {
                var promotion = _context.EducationPromotionCodes.FirstOrDefault(x => x.PromotionCode == promotionCode);
                if (promotion==null)
                {
                    throw new Exception("Geçersiz promosyon kodu girdiniz.");
                }
                var promotionItemCount = _context.EducationPromotionItems.Count(x=>x.EducationPromotionCodeId ==promotion.Id);
                var userBasedPromotionItemCount = _context.EducationPromotionItems.Count(x => x.UserId == userId && x.EducationPromotionCodeId == promotion.Id);
                if (promotion.MaxUsageLimit<promotionItemCount+1)
                {
                    throw new Exception("Bu kupon maksimum kullanım sınırına gelmiştir.");
                }
                if (promotion.UserBasedUsageLimit< userBasedPromotionItemCount + 1)
                {
                    throw new Exception("Bu kupon için kullanım hakkınız kalmamıştır.");
                }
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
