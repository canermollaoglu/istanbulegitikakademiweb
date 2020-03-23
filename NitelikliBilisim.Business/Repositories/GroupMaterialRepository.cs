using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_material;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class GroupMaterialRepository : BaseRepository<GroupMaterial, Guid>
    {
        private readonly NbDataContext _context;
        public GroupMaterialRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public GetMaterialVm GetMaterials(Guid groupId)
        {
            var model = new GetMaterialVm
            {
                Materials = new List<_Material>()
            };
            var materials = _context.GroupMaterials
                .Where(x => x.GroupId == groupId)
                .ToList();

            model.Materials = materials.Select(x => new _Material
                {
                    MaterialId = x.Id,
                    GroupId = x.GroupId,
                    MaterialName = x.MaterialName,
                    MaterialPrice = x.Price.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR")),
                    MaterialCount = $"{x.Count} adet"
                }).ToList();
            return model;
        }
        public bool AddMaterial(AddMaterialPostVm data)
        {
            if (string.IsNullOrWhiteSpace(data.MaterialName) || data.Count < 1 || data.Price < 0)
                return false;

            _context.GroupMaterials.Add(new GroupMaterial
            {
                Count = data.Count,
                GroupId = data.GroupId,
                MaterialName = data.MaterialName,
                Price = data.Price
            });

            _context.SaveChanges();
            return true;
        }
    }
}
