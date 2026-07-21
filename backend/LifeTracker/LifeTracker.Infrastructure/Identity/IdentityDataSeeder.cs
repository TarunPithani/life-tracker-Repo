using System.Security.Claims;
using LifeTracker.Domain.Common.Constants;
using LifeTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LifeTracker.Infrastructure.Identity;

public static class IdentityDataSeeder
{
    private static readonly Dictionary<string, string[]> RolePermissions = new()
    {
        [Roles.Admin] = Permissions.All.ToArray(),
        [Roles.Manager] =
        [
            Permissions.Todo.Read,
            Permissions.Todo.Create,
            Permissions.Todo.Update,
            Permissions.Todo.Delete,
            Permissions.Category.Read,
            Permissions.Category.Create,
            Permissions.Category.Update,
            Permissions.Category.Delete,
            Permissions.User.Read
        ],
        [Roles.User] =
        [
            Permissions.Todo.Read,
            Permissions.Todo.Create,
            Permissions.Todo.Update
        ]
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var logger = provider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentityDataSeeder");
        var dbContext = provider.GetRequiredService<AppDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedRolePermissionsAsync(roleManager);
        await SeedAdminUserAsync(userManager, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var descriptions = new Dictionary<string, string>
        {
            [Roles.Admin] = "Full system access",
            [Roles.Manager] = "Manage todos, categories, and users",
            [Roles.User] = "Standard application user"
        };

        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            await roleManager.CreateAsync(new ApplicationRole(roleName)
            {
                Description = descriptions[roleName]
            });
        }
    }

    private static async Task SeedRolePermissionsAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var (roleName, permissions) in RolePermissions)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var existingClaims = await roleManager.GetClaimsAsync(role);
            var existingPermissions = existingClaims
                .Where(claim => claim.Type == Permissions.ClaimType)
                .Select(claim => claim.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in permissions)
            {
                if (existingPermissions.Contains(permission))
                {
                    continue;
                }

                await roleManager.AddClaimAsync(
                    role,
                    new Claim(Permissions.ClaimType, permission));
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        var admin = await userManager.FindByEmailAsync(AuthConstants.Seed.AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = AuthConstants.Seed.AdminUserName,
                Email = AuthConstants.Seed.AdminEmail,
                FirstName = AuthConstants.Seed.AdminFirstName,
                LastName = AuthConstants.Seed.AdminLastName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(admin, AuthConstants.Seed.AdminPassword);
            if (!result.Succeeded)
            {
                logger.LogError(
                    "Failed to seed admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(error => error.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        logger.LogInformation(
            "Admin user ready: {Email}",
            AuthConstants.Seed.AdminEmail);
    }
}
