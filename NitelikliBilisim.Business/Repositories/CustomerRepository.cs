using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, string>
    {
        public CustomerRepository(NbDataContext context) : base(context)
        {

        }
    }
}
