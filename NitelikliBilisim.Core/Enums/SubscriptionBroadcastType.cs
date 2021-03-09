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
    public enum SubscriberType
    {
        [Description("Haber Bülteni Aboneliği")]
        NewsletterSubscriber =1010,
        [Description("Blog Aboneliği")]
        BlogSubscriber =1020
    }
}
