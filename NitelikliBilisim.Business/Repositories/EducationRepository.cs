using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>
    {
        private readonly NbDataContext context;
        public EducationRepository(NbDataContext context) : base(context)
        {
            this.context = context;
        }

        public void Insert(Education entity, List<Guid> categoryIds, bool isSaveLater = false)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var educationId = base.Insert(entity);

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
