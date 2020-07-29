using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums.educations
{
    public enum CriterionType
    {
        [Description("Bulunduğu Eğitim Günü")]
        EducationDay = 1010,
        [Description("Favori Eğitimleri")]
        WishListEducations = 1020,
        [Description("Satın Alınan Eğitimler")]
        PurchasedEducations=1030
    }
}
