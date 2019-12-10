using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.PagedEntity
{
    public class PagedEntity<TEntity>
    {
        public List<TEntity> Data { get; set; }
        public int Count { get; set; }
    }
}
