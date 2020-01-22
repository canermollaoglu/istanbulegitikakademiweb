using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Repositories;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Business.Repositories
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly NbDataContext Context;
        protected DbSet<TEntity> Table { get; }

        public BaseRepository(NbDataContext context)
        {
            Context = context;
            Table = Context.Set<TEntity>();
        }

        public TEntity GetById(TKey id)
        {
            return Table.Find(id);
        }

        public virtual TKey Insert(TEntity entity, bool isSaveLater = false)
        {
            Table.Add(entity);
            if (!isSaveLater)
                Save();
            return entity.Id;
        }

        public virtual int Update(TEntity entity, bool isSaveLater = false)
        {
            Table.Update(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Delete(TKey id, bool isSaveLater = false)
        {
            var entity = Table.Find(id);
            Table.Remove(entity);
            return isSaveLater ? 0 : Save();
        }
        public int Delete(TEntity entity, bool isSaveLater = false)
        {
            Table.Remove(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Save()
        {
            Context.EnsureAutoHistory();
            return Context.SaveChanges();
        }
        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? Table : Table.Where(predicate);
        }

        public List<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Table;

            if (filter != null)
                query = query.Where(filter);

            if (includes != null && includes.Length > 0)
                foreach (var include in includes)
                    query = query.Include(include);

            if (order != null)
                return order(query).AsNoTracking().ToList();

            return query.AsNoTracking().ToList();
        }
    }
}