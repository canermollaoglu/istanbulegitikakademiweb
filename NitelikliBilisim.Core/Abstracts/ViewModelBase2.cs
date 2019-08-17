namespace NitelikliBilisim.Core.Abstracts
{
    public abstract class ViewModelBase2<TKey1, TKey2> : AuditBase
    {
        public TKey1 Id { get; set; }
        public TKey2 Id2 { get; set; }
        public string CreatedUserDisplay { get; set; }
        public string UpdatedUserDisplay { get; set; }
    }
}