namespace NitelikliBilisim.Core.Abstracts
{
    public interface IEntity
    {
    }

    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
