using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Enums
{
    public enum EducationCommentSortingTypes
    {
        [Description("Son Eklenenler")]
        Date =1000,
        /// <summary>
        /// Yüksekten Düşüpe Doğru Sıralama
        /// </summary>
        [Description("En Yüksek Puan")]
        PointDescending = 1010,
        /// <summary>
        /// Düşükten Yükseğe Doğru Sıralama
        /// </summary>
        [Description("En Düşük Puan")]
        Point =1020
    }
}
