using NitelikliBilisim.Core.Abstracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Core.Repositories
{
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        TEntity GetById(TKey id);
        TKey Add(TEntity entity, bool isSaveLater = false);
        int Update(TEntity entity, bool isSaveLater = false);
        int Delete(TEntity entity, bool isSaveLater = false);
        int Save();
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null);
    }
}