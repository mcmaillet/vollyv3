using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Areas.Identity;

namespace VollyV3.Authorization.Policies.Requirements
{
    public class IsConfiguredHandler : AuthorizationHandler<IsConfiguredRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsConfiguredRequirement requirement)
        {
            if (context.User.IsInRole(Enum.GetName(typeof(Role), Role.IsConfigured)))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
