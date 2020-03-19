using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
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
    }
}
