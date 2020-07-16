using Microsoft.Extensions.DependencyInjection;
using NitelikliBilisim.Core.Utilities.IoC;

namespace NitelikliBilisim.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection services, ICoreModule[] modules)
        {
            foreach (var coreModule in modules)
            {
                coreModule.Load(services);
            }

            return ServiceTool.Create(services);
        }
    }
}
