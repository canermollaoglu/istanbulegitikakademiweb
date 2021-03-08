using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums
{
    public enum EmailTemplateType
    {
        [Description("Genel")]
        General = 1000,
        [Description("Haber Bülteni Gönderisi")]
        NewsletterSubscribersPostTemplate = 1010,
        [Description("Blog Aboneleri Gönderisi")]
        BlogSubscribersPostTemplate = 1020,

    }
}
