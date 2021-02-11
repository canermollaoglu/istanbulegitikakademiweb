using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums
{
    public enum ContactFormTypes
    {
        ContactForm =1010,
        SSS=1020
    }
    public enum ContactFormSubjects
    {
        [Description("Üyelik Bilgileri")]
        MembershipInfo =1010,
        [Description("Eğitim Detayları")]
        EducationDetails = 1020,
        [Description("Eğitmen Detayları")]
        EducatorDetails = 1030,
        [Description("Ödeme İşlemleri")]
        PaymentInfo = 1040,
        [Description("Diğer İşlemler")]
        Other = 1050
    }
}
