using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NitelikliBilisim.Core.Enums.promotion
{
    public enum PromotionType
    {
        [Description("Kupon Kodu")]
        CouponCode = 1010,
        [Description("Sepet'te Geçerli")]
        BasketBased = 1020
    }

    public enum ConditionType
    {
        [Description("Kullanıcı")]
        User = 1010,
        [Description("Sepetteki Eğitim")]
        Education = 1020,
        [Description("Sepetteki Kategori")]
        Category = 1030,
        [Description("Satın Alınmış Eğitim")]
        PurchasedEducation = 1040,
        [Description("Satın Alınmış Eğitim Kategorisi")]
        PurchasedCategory = 1050
    }

    public enum PromotionValidityType
    {
        [Description("Sepet'te Geçerli")]
        Basket = 1010,
        [Description("Belirli Eğitimler İçin Geçerli")]
        Education = 1020,
        [Description("Belirli Kategoriler İçin Geçerli")]
        Category = 1030

    }
}
