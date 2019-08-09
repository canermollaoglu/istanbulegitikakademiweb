using NitelikliBilisim.Core.Abstracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Core.Repositories
{
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        TEntity GetById(TKey id);
        TKey Add(TEntity entity);
        int Update(TEntity entity);
        int Delete(TEntity entity);

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> where = null);
    }
}
