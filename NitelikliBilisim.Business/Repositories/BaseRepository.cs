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
        protected readonly NbDataContext _context;
        private DbSet<TEntity> _table { get; }

        public BaseRepository(NbDataContext context)
        {
            _context = context;
            _table = _context.Set<TEntity>();
        }

        public TEntity GetById(TKey id)
        {
            return _table.Find(id);
        }

        public virtual TKey Insert(TEntity entity, bool isSaveLater = false)
        {
            _table.Add(entity);
            if (!isSaveLater)
                Save();
            return entity.Id;
        }

        public int Update(TEntity entity, bool isSaveLater = false)
        {
            _table.Update(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Delete(TKey id, bool isSaveLater = false)
        {
            var entity = _table.Find(id);
            _table.Remove(entity);
            return isSaveLater ? 0 : Save();
        }
        public int Delete(TEntity entity, bool isSaveLater = false)
        {
            _table.Remove(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _table : _table.Where(predicate);
        }

        public List<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table;

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