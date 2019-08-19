using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace NitelikliBilisim.App.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            #region Dependency Injections
            services.AddScoped<IRepository<Kategori, Guid>, Repository<Kategori, Guid>>();
            services.AddScoped<IRepository<Egitim, Guid>, Repository<Egitim, Guid>>();
            services.AddScoped<IRepository<EgitimKategori, Guid>, Repository<EgitimKategori, Guid>>();
            services.AddScoped<IRepository<EgitimDetay, Guid>, Repository<EgitimDetay, Guid>>();
            services.AddScoped<IRepository<MusteriYorum, Guid>, Repository<MusteriYorum, Guid>>();
            services.AddScoped<IRepository<EgitimKazanim, Guid>, Repository<EgitimKazanim, Guid>>();
            services.AddScoped<IRepository<Egitici, string>, Repository<Egitici, string>>();
            services.AddScoped<IRepository<Sepet, Guid>, Repository<Sepet, Guid>>();
            services.AddScoped<IRepository<Satis, Guid>, Repository<Satis, Guid>>();
            services.AddScoped<IRepository<SatisDetay, Guid>, Repository<SatisDetay, Guid>>();
            #endregion

            #region IdentityConfig

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
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

            #region OAuth

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = configuration.GetSection("Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.CallbackPath = new PathString("/signin-google-token");
                    options.AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
                    options.CallbackPath = new PathString("/signin-google-token");
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.SaveTokens = true;
                    options.Events.OnCreatingTicket = ctx =>
                    {
                        List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();
                        tokens.Add(new AuthenticationToken()
                            {Name = "TicketCreated", Value = DateTime.UtcNow.ToString()});
                        ctx.Properties.StoreTokens(tokens);
                        return Task.CompletedTask;
                    };
                });

            #endregion

            return services;
        }
    }
}
