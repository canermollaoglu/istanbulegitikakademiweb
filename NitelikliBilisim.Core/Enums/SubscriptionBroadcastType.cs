using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums
{
    public enum SubscriptionBroadcastType
    {
        [Description("Haber Bülteni Yayını")]
        NewsletterBroadcast = 1010,
        [Description("Blog Aboneleri Yayını")]
        BlogBroadcast = 1020
    }
}
