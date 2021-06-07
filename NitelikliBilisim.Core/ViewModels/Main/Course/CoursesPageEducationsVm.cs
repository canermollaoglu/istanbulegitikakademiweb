using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class CoursesPageEducationsVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public byte EducationDays { get; set; }
        public int EducationHours { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CategoryId { get; set; }
        public string Seo { get; set; }
        public string CSeo { get; set; }
        public EducationLevel Level { get; set; }
        public string Price { get; set; }
        public Guid CategoryId2 { get; set; }
        public byte Order { get; set; }
        public string LevelString { get; set; }
    }
}
