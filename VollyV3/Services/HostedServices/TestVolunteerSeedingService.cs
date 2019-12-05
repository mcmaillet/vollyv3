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
    public class TestVolunteerSeedingService : IHostedService
    {
        private static readonly string Email = "user@volly.app";
        private static readonly string PhoneNumber = "4038881111";
        private static readonly string Password = "asdfasdf";

        private readonly IServiceProvider _serviceProvider;
        public TestVolunteerSeedingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await EnsureTestVolunteerExists(Email,
                PhoneNumber,
                Password);
        }
        private async Task EnsureTestVolunteerExists(string email,
            string phoneNumber,
            string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<VollyV3User>>();

            var testUser = new VollyV3User
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
                var createTestUser = await userManager.CreateAsync(testUser, password);
                if (createTestUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, Enum.GetName(typeof(Role), Role.Volunteer));

                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
