using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Course
{
    public class UserCommentPostVm
    {
        
        public byte Point { get; set; }

        [Required]
        [MaxLength(512)]
        public string Content { get; set; }
        [Required]
        public Guid EducationId { get; set; }
    }
}
