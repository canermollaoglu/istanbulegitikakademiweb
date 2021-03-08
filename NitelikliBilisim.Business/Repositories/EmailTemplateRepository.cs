using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.Repositories
{
    public class EmailTemplateRepository : BaseRepository<EmailTemplate, Guid>
    {
        public EmailTemplateRepository(NbDataContext context) : base(context)
        {
        }

    }
}
