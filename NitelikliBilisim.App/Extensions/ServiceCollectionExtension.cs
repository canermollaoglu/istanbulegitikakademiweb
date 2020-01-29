using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Facebook;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.Services.Abstracts;

namespace NitelikliBilisim.App.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            #region Dependency Injections
            services.AddSingleton<IMessageService, EmailService>();
            services.AddSingleton<IStorageService, StorageService>();
            #endregion

            #region IdentityConfig

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = false;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
                options.User.RequireUniqueEmail = true;


            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                options.LoginPath = "/giris-yap";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            #endregion

            #region OAuth

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        var googleAuthNSection = configuration.GetSection("Google");
            //        options.ClientId = googleAuthNSection["ClientId"];
            //        options.ClientSecret = googleAuthNSection["ClientSecret"];
            //        options.AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
            //        options.CallbackPath = new PathString("/signin-google-token");
            //        options.Scope.Add("openid");
            //        options.Scope.Add("profile");
            //        options.Scope.Add("email");
            //        options.SaveTokens = true;
            //        options.Events.OnCreatingTicket = ctx =>
            //        {
            //            var tokens = ctx.Properties.GetTokens().ToList();
            //            ctx.Identity.AddClaim(new Claim("photo", ctx.User.Value<string>("picture")));
            //            tokens.Add(new AuthenticationToken()
            //            { Name = "TicketCreated", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) });
            //            ctx.Properties.StoreTokens(tokens);
            //            return Task.CompletedTask;
            //        };
            //    })
            //    .AddFacebook(options =>
            //    {
            //        var facebookAuthSection = configuration.GetSection("Facebook");
            //        options.AppId = facebookAuthSection["AppId"];
            //        options.AppSecret = facebookAuthSection["AppSecret"];
            //        options.CallbackPath = new PathString("/signin-facebook");
            //        options.AuthorizationEndpoint = FacebookDefaults.AuthorizationEndpoint;
            //        options.Fields.Add("picture");
            //        options.Events.OnCreatingTicket = ctx =>
            //        {
            //            var tokens = ctx.Properties.GetTokens().ToList();
            //            var profileImg = ctx.User["picture"]["data"].Value<string>("url");
            //            //ctx.Identity.AddClaim(new Claim("photo", profileImg));

            //            tokens.Add(new AuthenticationToken()
            //            { Name = "TicketCreated", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) });
            //            ctx.Properties.StoreTokens(tokens);
            //            return Task.CompletedTask;
            //        };
            //    })
            //    .AddGitHub(options =>
            //    {
            //        var githubAuthNSection = configuration.GetSection("GitHub");
            //        options.ClientId = githubAuthNSection["ClientId"];
            //        options.ClientSecret = githubAuthNSection["ClientSecret"];
            //        options.AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint;
            //        options.Scope.Add("user:email");
            //        options.Events.OnCreatingTicket = ctx =>
            //        {
            //            var tokens = ctx.Properties.GetTokens().ToList();
            //            ctx.Identity.AddClaim(new Claim("photo", ctx.User.Value<string>("avatar_url")));
            //            tokens.Add(new AuthenticationToken()
            //            { Name = "TicketCreated", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) });
            //            ctx.Properties.StoreTokens(tokens);
            //            return Task.CompletedTask;
            //        };
            //    });

            #endregion

            return services;
        }
    }
}
