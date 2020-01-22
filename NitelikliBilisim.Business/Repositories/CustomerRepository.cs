using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, string>
    {
        public CustomerRepository(NbDataContext context) : base(context)
        {

        }
    }
}
