using NitelikliBilisim.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Mapper
{
    public class Mapper<TEntity, TDto> : IMapper<TEntity, TDto> where TEntity : new() where TDto : new()
    {
        public TDto Map(TEntity entity)
        {
            var dto = new TDto();
            var entityProps = entity.GetType().GetProperties();
            var dtoProps = dto.GetType().GetProperties();

            foreach (var entityProp in entityProps)
            {
                foreach (var dtoProp in dtoProps)
                {
                    if (entityProp.Name == dtoProp.Name)
                    {
                        if (!entityProp.PropertyType.IsEnum)
                        {
                            var type = dtoProp.PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                type = Nullable.GetUnderlyingType(type);

                            dtoProp.SetValue(dto, Convert.ChangeType(entityProp.GetValue(entity), type));
                        }

                        break;
                    }
                }
            }

            return dto;
        }
    }
}
