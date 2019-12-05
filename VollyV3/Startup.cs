using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VollyV3.Models;
using VollyV3.Services;
using VollyV3.Services.HostedServices;
using VollyV3.Services.ImageManager;

namespace VollyV3
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("vollyprod")));

            services.AddIdentity<VollyV3User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddAuthentication()
            //    .AddCookie(cfg => cfg.SlidingExpiration = true)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
            //    {
            //        jwtBearerOptions.SaveToken = true;
            //        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = false,
            //            ValidateAudience = false,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(VollyConstants.BearerSecret)),
            //            ClockSkew = TimeSpan.Zero
            //        };
            //    });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMemoryCache();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            //services.AddProgressiveWebApp();

            services.AddSignalR();

            //services.AddScoped<IAuthorizationHandler, OpportunityAuthorizationHandler>();

            //services.AddCors(o =>
            //    o.AddPolicy("MyPolicy", builder => { builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); }));

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHostedService<RoleSeedingService>();
            services.AddHostedService<PlatformAdministratorSeedingService>();
            services.AddHostedService<TestOrganizationAdministratorSeedingService>();
            services.AddHostedService<TestVolunteerSeedingService>();

            services.AddSingleton<IImageManager, ImageManagerImpl>();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "Home",
            //    //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //       name: "default",
            //       template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //endpoints.MapGet("/", async context =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            //    endpoints.MapControllers();
            //    endpoints.MapRazorPages();
            //});

        }
    }
}
