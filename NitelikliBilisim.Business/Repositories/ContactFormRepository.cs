using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class ContactFormRepository :BaseRepository<ContactForm,Guid>
    {
        public ContactFormRepository(NbDataContext context) : base(context)
        {

        }

    }
}
