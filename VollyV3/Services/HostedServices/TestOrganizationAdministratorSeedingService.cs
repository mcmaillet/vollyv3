using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Users;

namespace VollyV3.Services.HostedServices
{
    public class TestOrganizationAdministratorSeedingService : IHostedService
    {
        private static readonly string Email = "test@volly.app";
        private static readonly string PhoneNumber = "4038881111";
        private static readonly string Password = "asdfasdf";

        private readonly IServiceProvider _serviceProvider;
        public TestOrganizationAdministratorSeedingService(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await EnsureTestOrganizationAdministratorExists(Email,
                PhoneNumber,
                Password);
        }
        private async Task EnsureTestOrganizationAdministratorExists(string email,
            string phoneNumber,
            string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<VollyV3User>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                    var newOrganization = new Organization()
                    {
                        IsApproved = true,
                        Name = email + "_testorg",
                        ContactEmail = email,
                        PhoneNumber = phoneNumber,
                        FullDescription = "Test organization",
                    };
                    await userManager.AddToRoleAsync(testUser, Enum.GetName(typeof(Role), Role.OrganizationAdministrator));
                    await dbContext.Organizations.AddAsync(newOrganization);
                    await dbContext.SaveChangesAsync();
                    var createdUser = await userManager.FindByEmailAsync(email);

                    dbContext.OrganizationAdministratorUsers.Add(new OrganizationAdministratorUser()
                    {
                        UserId = createdUser.Id,
                        OrganizationId = newOrganization.Id
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
