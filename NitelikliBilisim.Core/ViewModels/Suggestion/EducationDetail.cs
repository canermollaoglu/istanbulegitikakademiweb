using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.Core.ViewModels.Suggestion
{
    public class EducationDetail
    {
        public Guid Id { get; set; }
        public Education Education { get; set; }
        public double Point { get; set; }
    }
}
