using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Repositories;
using NitelikliBilisim.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Business.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly NbDataContext _context;
        public DbSet<TEntity> Table { get; }

        public Repository(NbDataContext context)
        {
            _context = context;
            Table = _context.Set<TEntity>();
        }

        public TEntity GetById(TKey id)
        {
            return Table.Find(id);
        }

        public TKey Add(TEntity entity, bool isSaveLater = false)
        {
            Table.Add(entity);
            if (!isSaveLater)
                Save();
            return entity.Id;
        }

        public int Update(TEntity entity, bool isSaveLater = false)
        {
            Table.Update(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Delete(TEntity entity, bool isSaveLater = false)
        {
            Table.Remove(entity);
            return isSaveLater ? 0 : Save();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? Table : Table.Where(predicate);
        }
    }
}