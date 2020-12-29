using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums
{
    public enum ContactFormTypes
    {
        ContactForm =1010,
        SSS=1020
    }
    public enum ContactFormSubjects
    {
        [Description("Destek")]
        Support =1010
    }
}
