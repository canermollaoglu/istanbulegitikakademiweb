using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion;
using NitelikliBilisim.Core.ViewModels.Main.Cart;
using NitelikliBilisim.Core.ViewModels.Sales;
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

        public IQueryable<EducationPromotionCodeListVm> GetCouponCodeBasedPromotionList()
        {

            return _context.EducationPromotionCodes.Include(x => x.EducationPromotionItems)
                .Where(x=>x.PromotionType == Core.Enums.promotion.PromotionType.CouponCode)
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
                     CountOfUses = promotionCode.EducationPromotionItems.Count,
                     UserBasedUsageLimit = promotionCode.UserBasedUsageLimit,
                     PromotionType=promotionCode.PromotionType

                 });
        }

        public IQueryable<EducationBasketBasedPromotionListVm> GetBasketBasedPromotionList()
        {
            return _context.EducationPromotionCodes.Include(x => x.EducationPromotionItems)
                .Where(x => x.PromotionType == Core.Enums.promotion.PromotionType.BasketBased)
                 .Select(promotionCode => new EducationBasketBasedPromotionListVm
                 {
                     Id = promotionCode.Id,
                     Name = promotionCode.Name,
                     StartDate = promotionCode.StartDate,
                     EndDate = promotionCode.EndDate,
                     Description = promotionCode.Description,
                     MaxUsageLimit = promotionCode.MaxUsageLimit,
                     DiscountAmount = promotionCode.DiscountAmount,
                     MinBasketAmount = promotionCode.MinBasketAmount,
                     IsActive = DateTime.Now.Date < promotionCode.EndDate && DateTime.Now >= promotionCode.StartDate ? "Aktif" : "Pasif",
                     CountOfUses = promotionCode.EducationPromotionItems.Count,
                     UserBasedUsageLimit = promotionCode.UserBasedUsageLimit,
                     PromotionType = promotionCode.PromotionType
                 });
        }

        public IQueryable<UsagePromotionListVm> GetUsagePromotionList(Guid promotionCodeId)
        {
            return (from user in _context.Users
                    join promotionUsageItem in _context.EducationPromotionItems on user.Id equals promotionUsageItem.UserId
                    where promotionUsageItem.EducationPromotionCodeId == promotionCodeId
                    select new UsagePromotionListVm
                   {
                       Id = promotionUsageItem.Id,
                       DateOfUse = promotionUsageItem.CreatedDate,
                       StudentId = promotionUsageItem.UserId,
                       Name = user.Name,
                       Surname = user.Surname,
                       InvoiceId = promotionUsageItem.InvoiceId
                   });
        }

        public IQueryable<EducationPromotionCode> GetBasketBasedPromotions()
        {
           return _context.EducationPromotionCodes.Include(x => x.EducationPromotionConditions).Where(x => x.PromotionType == Core.Enums.promotion.PromotionType.BasketBased);
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
        public int GetEducationPromotionItemCountByUserId(Guid promotionCodeId,string userId)
        {
            return _context.EducationPromotionItems.Count(x => x.UserId == userId && x.EducationPromotionCodeId == promotionCodeId);
        }
        public int GetEducationPromotionItemByPromotionCodeId(Guid promotionCodeId)
        {
            return _context.EducationPromotionItems.Count(x => x.EducationPromotionCodeId == promotionCodeId);
        }



        public EducationPromotionCode GetPromotionbyPromotionCode(string promotionCode)
        {
                return _context.EducationPromotionCodes.Include(x=>x.EducationPromotionConditions).FirstOrDefault(x => x.PromotionCode == promotionCode);
        }

    }
}
