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
        [Description("Eğitim")]
        Education = 1020,
        [Description("Kategori")]
        Category = 1030
    }
}
