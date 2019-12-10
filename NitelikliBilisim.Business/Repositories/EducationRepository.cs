using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>, IPageableEntity<Education>
    {
        public EducationRepository(NbDataContext context) : base(context)
        {
        }

        public PagedEntity<Education> GetPagedEntity(int page = 0, Expression<Func<Education, bool>> filter = null, int shownRecords = 15)
        {
            IQueryable<Education> dbSet = base._table;
            if (filter != null)
                dbSet = dbSet.Where(filter);

            return new PagedEntity<Education>
            {
                Data = dbSet.OrderBy(o => o.Name)
                    .Skip(page * shownRecords)
                    .Take(shownRecords)
                    .AsNoTracking()
                    .ToList(),
                Count = dbSet.Count()
            };
        }

        public Guid? Insert(Education entity, List<Guid> categoryIds, List<EducationMedia> medias, bool isSaveLater = false)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return null;

            using (var transaction = _context.Database.BeginTransaction())
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
                    foreach (var item in medias)
                        item.EducationId = educationId;

                    _context.EducationMedias.AddRange(medias);
                    _context.Bridge_EducationCategories.AddRange(bridge);
                    _context.SaveChanges();
                    transaction.Commit();
                    return educationId;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }
}
