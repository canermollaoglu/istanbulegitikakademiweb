using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Sales
{
    public class PayPostVm
    {
        [Required]
        public _CardInfo CardInfo { get; set; }
    }

    public class _CardInfo
    {
        public string NameOnCard { get; set; }

        public string NumberOnCard { get; set; }

        [Required, MaxLength(2)]
        public string MonthOnCard { get; set; }

        [Required, MaxLength(2)]
        public string YearOnCard { get; set; }

        [Required, MaxLength(2)]
        public string CVC { get; set; }
    }
}
