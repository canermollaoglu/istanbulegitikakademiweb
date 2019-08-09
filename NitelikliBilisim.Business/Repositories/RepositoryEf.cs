using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Repositories;

namespace NitelikliBilisim.Business.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {


        public TEntity GetById(TKey id)
        {
            throw new NotImplementedException();
        }

        public TKey Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public int Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> @where = null)
        {
            throw new NotImplementedException();
        }
    }
}
