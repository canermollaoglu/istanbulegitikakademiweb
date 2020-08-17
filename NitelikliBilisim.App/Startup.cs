using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Hubs;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Globalization;
using System.IO;

namespace NitelikliBilisim.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<NbDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<NbDataContext>()
                //.AddUserStore<UserStore<ApplicationUser, ApplicationRole, NbDataContext>>()
                //.AddRoleStore<RoleStore<ApplicationRole, NbDataContext>>()
                //.AddUserManager<UserManager<ApplicationUser>>()
                //.AddRoleManager<RoleManager<ApplicationRole>>()
                .AddDefaultTokenProviders();

            services.AddScoped<UnitOfWork>();
            services.AddScoped<ComingSoonActionFilter>();
            services.AddApplicationServices(this.Configuration);

            //services.AddControllers(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });
            services.AddCors(options=>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            services.AddMvc();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultureInfo = new CultureInfo("tr-TR") { NumberFormat = { NumberDecimalSeparator = "." } };

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/yakinda");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
                RequestPath = new PathString("/vendor")
            });
            app.UseAzureSignalR(config =>
            {
                config.MapHub<MessageHub>("/messages");
            });
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseCors("AllowAll");
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapAreaControllerRoute("admin", "admin", "admin/{controller=Manage}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    "default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
