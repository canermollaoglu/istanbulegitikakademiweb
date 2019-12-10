using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NitelikliBilisim.Business.PagedEntity
{
    public interface IPageableEntity<TEntity> where TEntity : class
    {
        PagedEntity<TEntity> GetPagedEntity(int page = 0, Expression<Func<TEntity, bool>> filter = null, int shownRecords = 15);
    }
}
