using Newtonsoft.Json;
using System;

namespace NitelikliBilisim.Core.RabbitMq
{
    public class CorrelationContext<TPk> : ICorrelationContext<TPk>
    {
        public Guid CorrelationId { get; }
        public TPk PrimaryKey { get; }
        public CorrelationContext()
        {
        }

        [JsonConstructor]
        private CorrelationContext(Guid correlationId, TPk primaryKey)
        {
            CorrelationId = correlationId;
            PrimaryKey = primaryKey;
        }

        public static ICorrelationContext<TPk> Create(Guid id, TPk primaryKey)
        {
            return new CorrelationContext<TPk>(id, primaryKey);
        }
    }
}
