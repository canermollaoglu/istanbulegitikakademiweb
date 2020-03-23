using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.group_material
{
    public class GetMaterialVm
    {
        public List<_Material> Materials { get; set; }
    }

    public class _Material
    {
        public Guid MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialPrice { get; set; }
        public string MaterialCount { get; set; }
        public Guid GroupId { get; set; }
    }
}
