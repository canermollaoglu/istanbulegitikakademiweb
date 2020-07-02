using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Data;

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
        public StateRepository(NbDataContext context):base(context)
        {

        }
    }
}
