using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System;

namespace NitelikliBilisim.App.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Kategori, Guid>, Repository<Kategori, Guid>>();
            services.AddScoped<IRepository<Egitim, Guid>, Repository<Egitim, Guid>>();
            services.AddScoped<IRepository<EgitimKategori, Guid>, Repository<EgitimKategori, Guid>>();
            services.AddScoped<IRepository<EgitimDetay, Guid>, Repository<EgitimDetay, Guid>>();
            services.AddScoped<IRepository<MusteriYorum, Guid>, Repository<MusteriYorum, Guid>>();
            services.AddScoped<IRepository<EgitimKazanim, Guid>, Repository<EgitimKazanim, Guid>>();
            services.AddScoped<IRepository<Egitici, string>, Repository<Egitici, string>>();

            #region IdentityConfig

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = false;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            #endregion

            return services;
        }
    }
}
