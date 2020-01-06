namespace NitelikliBilisim.Business.Mapper
{
    public interface IMapper<TEntity, TDto> where TEntity : new() where TDto : new()
    {
        TDto Map(TEntity entity);
    }
}
