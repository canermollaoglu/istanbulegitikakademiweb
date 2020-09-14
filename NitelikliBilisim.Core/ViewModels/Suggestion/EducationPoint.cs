using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Suggestion
{
    public class EducationPoint
    {
        public Education Education{ get; set; }
        public Guid EducationId { get; set; }
        public double Point { get; set; }
    }
}
