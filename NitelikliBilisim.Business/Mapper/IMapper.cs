using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Mapper
{
    public interface IMapper<TEntity, TDto> where TEntity : new() where TDto : new()
    {
        TDto Map(TEntity entity);
    }
}
