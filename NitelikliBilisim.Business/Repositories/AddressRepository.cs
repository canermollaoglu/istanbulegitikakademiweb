
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class AddressRepository:BaseRepository<Address,int>
    {
        public AddressRepository(NbDataContext context):base(context)
        {

        }
    }
}
