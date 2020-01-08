using System;

namespace NitelikliBilisim.Core.RabbitMq
{
    public interface ICorrelationContext<out TPk>
    {
        Guid CorrelationId { get; }
        TPk PrimaryKey { get; }
    }
}
