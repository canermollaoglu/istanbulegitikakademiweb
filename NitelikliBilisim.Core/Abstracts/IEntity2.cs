﻿namespace NitelikliBilisim.Core.Abstracts
{
    public interface IEntity2<TKey1, TKey2> : IEntity<TKey1>
    {
        TKey1 Id { get; set; }
        TKey2 Id2 { get; set; }
    }
}
