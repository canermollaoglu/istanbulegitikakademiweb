namespace NitelikliBilisim.Core.Abstracts
{
    public abstract class ViewModelBase<TKey> : AuditBase
    {
        public TKey Id { get; set; }
        public string CreatedUserDisplay { get; set; }
        public string UpdatedUserDisplay { get; set; }
    }
}