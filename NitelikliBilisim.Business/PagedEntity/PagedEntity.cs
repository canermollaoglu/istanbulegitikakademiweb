using System.Collections.Generic;

namespace NitelikliBilisim.Business.PagedEntity
{
    public class PagedEntity<TEntity>
    {
        public List<TEntity> Data { get; set; }
        public int Count { get; set; }
    }
}
