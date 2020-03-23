using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_material
{
    public class AddMaterialPostVm
    {
        public string MaterialName { get; set; }
        public decimal Price { get; set; }
        public byte Count { get; set; }
        public Guid GroupId { get; set; }
    }
}
