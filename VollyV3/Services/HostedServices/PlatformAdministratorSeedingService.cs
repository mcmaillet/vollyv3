using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using VollyV3.Areas.Identity;
using VollyV3.Models;

namespace VollyV3.Services.HostedServices
{
    public class PlatformAdministratorSeedingService : IHostedService
    {
        private static readonly string Email = "email@gmailss.com";
        private static readonly string PhoneNumber = "4038092222";
        private static readonly string Password = "somepwd";

        private readonly IServiceProvider _serviceProvider;
        public PlatformAdministratorSeedingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (true)
            {
                return;
            }
            await EnsurePlatformAdministratorExists(Email,
                PhoneNumber,
                Password);
        }
        private async Task EnsurePlatformAdministratorExists(string email,
            string phoneNumber,
            string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<VollyV3User>>();

            var poweruser = new VollyV3User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };
            var _user = await userManager.FindByEmailAsync(email);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(poweruser, password);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(poweruser, Enum.GetName(typeof(Role), Role.PlatformAdministrator));

                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
