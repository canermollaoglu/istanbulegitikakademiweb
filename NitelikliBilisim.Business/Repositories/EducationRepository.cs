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

        public bool Insert(Education entity, List<Guid> categoryIds, bool isSaveLater = false)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return false;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    entity.IsActive = false;
                    var educationId = base.Insert(entity);

                    var bridge = new List<Bridge_EducationCategory>();
                    foreach (var categoryId in categoryIds)
                        bridge.Add(new Bridge_EducationCategory
                        {
                            Id = categoryId,
                            Id2 = educationId
                        });

                    context.Bridge_EducationCategories.AddRange(bridge);
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
