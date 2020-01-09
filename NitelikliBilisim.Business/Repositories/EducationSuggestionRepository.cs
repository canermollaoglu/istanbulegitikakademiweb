using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationSuggestionRepository : BaseRepository<Suggestion, Guid>
    {
        public EducationSuggestionRepository(NbDataContext context) : base(context)
        {

        }
    }
}
