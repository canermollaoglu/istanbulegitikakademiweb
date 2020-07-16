using Microsoft.Extensions.DependencyInjection;

namespace NitelikliBilisim.Core.Utilities.IoC
{
    public interface ICoreModule
    {
        void Load(IServiceCollection collection);
    }
}
