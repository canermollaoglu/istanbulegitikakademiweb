using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums.educations
{
    public enum CriterionType
    {
        [Description("Eğitim Günü")]
        EducationDay = 1010,
        [Description("Favori Eğitimler")]
        WishListEducations = 1020,
        [Description("Satın Alınan Eğitimler")]
        PurchasedEducations=1030
    }
}
