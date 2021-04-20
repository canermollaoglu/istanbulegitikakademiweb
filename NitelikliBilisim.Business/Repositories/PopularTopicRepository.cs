using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class PopularTopicRepository : BaseRepository<PopularTopic,Guid>
    {
        private readonly NbDataContext _context;
        public PopularTopicRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
