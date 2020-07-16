using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using NitelikliBilisim.Core.Utilities.Interceptors;
using NitelikliBilisim.Data;
using System;

namespace NitelikliBilisim.Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<BaseRepository<Education, Guid>>().As<IRepository<Education, Guid>>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();

        }
    }
}
