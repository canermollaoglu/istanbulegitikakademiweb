using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CityRepository : BaseRepository<City,int>
    {
        public CityRepository(NbDataContext context):base(context)
        {

        }
    }

    public class StateRepository : BaseRepository<State, int>
    {
        private readonly NbDataContext _context;
        public StateRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        public List<State> GetStateByCityId(int cityId)
        {
            return _context.States.Where(x => x.CityId == cityId).ToList();
        }
    }
}
