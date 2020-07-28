using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, string>
    {
        public CustomerRepository(NbDataContext context) : base(context)
        {

        }


        public IQueryable<Customer> GetCustomerListQueryable()
        {
            return Context.Customers.Include(x => x.User);

        }

    }
}
