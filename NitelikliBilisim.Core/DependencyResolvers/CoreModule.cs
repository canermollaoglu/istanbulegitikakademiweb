using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NitelikliBilisim.Core.Utilities.IoC;
using System.Diagnostics;

namespace NitelikliBilisim.Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Stopwatch>();
        }
    }
}
