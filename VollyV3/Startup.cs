using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VollyV3.Controllers.Databind.Serialization;
using VollyV3.Models;
using VollyV3.Services.EmailSender;
using VollyV3.Services.HostedServices;
using VollyV3.Services.ImageManager;
using VollyV3.Services.SendGrid;

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

            services.AddMemoryCache();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddSignalR();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LoginPath = "/Identity/Account/Login";
            });

            services.AddControllersWithViews()
                .AddJsonOptions(options=>
                {
                    options.JsonSerializerOptions.Converters.Add(new LineBreakingDateTimeConverter());
                });

            services.AddRazorPages();

            services.AddSingleton<IImageManager, ImageManagerImpl>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsConfigured", policy =>
                 policy.RequireRole("IsConfigured"));
            });

            services.AddHttpClient("sendgrid", c =>
            {
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                     "Bearer",
                     Environment.GetEnvironmentVariable("sendgrid_api_key")
                     );
                c.BaseAddress = new Uri("https://api.sendgrid.com/v3/");
            });

            AddCustomTransients(services);
        }

        private void AddCustomTransients(IServiceCollection services)
        {
            services.AddTransient<IEmailSenderExtended, EmailSender>();
            services.AddTransient<SendGridClientImpl>();
        }
        private void AddHostedSeedingServices(IServiceCollection services)
        {
            services.AddHostedService<RoleSeedingService>();
            services.AddHostedService<PlatformAdministratorSeedingService>();
            services.AddHostedService<TestOrganizationAdministratorSeedingService>();
            services.AddHostedService<TestVolunteerSeedingService>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
