using LowCarbLife.Api.Data;
using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Features.Recipes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LowCarbLife.Api.Tests.Data;

public class DataSeederTests
{
    private const string AdminEmail = "admin@tests.local";
    private const string AdminPassword = "Admin123!";

    private static ServiceProvider BuildServices(
        string databaseName,
        string? adminEmail = AdminEmail,
        string? adminPassword = AdminPassword)
    {
        var settings = new Dictionary<string, string?>();
        if (adminEmail is not null)
        {
            settings["Seed:AdminEmail"] = adminEmail;
        }

        if (adminPassword is not null)
        {
            settings["Seed:AdminPassword"] = adminPassword;
        }

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        return services.BuildServiceProvider();
    }

    private static async Task SeedAsync(ServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        await DataSeeder.SeedAsync(scope.ServiceProvider);
    }

    [Fact]
    public async Task SeedAsync_CreatesAdminAndUserRoles()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        Assert.True(await roles.RoleExistsAsync(Roles.Admin));
        Assert.True(await roles.RoleExistsAsync(Roles.User));
    }

    [Fact]
    public async Task SeedAsync_CreatesAdminAccountFromConfiguration()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await users.FindByEmailAsync(AdminEmail);
        Assert.NotNull(admin);
        Assert.Equal("Admin", admin.DisplayName);
        Assert.True(admin.EmailConfirmed);
        Assert.True(await users.IsInRoleAsync(admin, Roles.Admin));
        Assert.True(await users.CheckPasswordAsync(admin, AdminPassword));
    }

    [Fact]
    public async Task SeedAsync_WithoutConfiguration_UsesDefaultAdminCredentials()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString(), adminEmail: null, adminPassword: null);

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await users.FindByEmailAsync("admin@lowcarblife.local");
        Assert.NotNull(admin);
        Assert.True(await users.IsInRoleAsync(admin, Roles.Admin));
    }

    [Fact]
    public async Task SeedAsync_AddsSampleBlogPostAuthoredByAdmin()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await users.FindByEmailAsync(AdminEmail);
        var post = await db.BlogPosts.SingleAsync();
        Assert.Equal(admin!.Id, post.AuthorId);
        Assert.False(string.IsNullOrWhiteSpace(post.Content));
    }

    [Fact]
    public async Task SeedAsync_AddsRecipesForBothDiets()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.True(await db.Recipes.AnyAsync(r => r.DietType == DietType.Keto));
        Assert.True(await db.Recipes.AnyAsync(r => r.DietType == DietType.LowCarb));
        Assert.True(await db.Recipes.AnyAsync(r => r.MealCategory == MealCategory.Salatki));
    }

    [Fact]
    public async Task SeedAsync_NeverAssignsKetoOnlyMealsToLowCarbRecipes()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var lowCarbMeals = await db.Recipes
            .Where(r => r.DietType == DietType.LowCarb)
            .Select(r => r.MealCategory)
            .Distinct()
            .ToListAsync();
        Assert.All(lowCarbMeals, meal =>
            Assert.Contains(meal, new[] { MealCategory.Sniadanie, MealCategory.Obiad, MealCategory.Kolacja }));
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_IsIdempotent()
    {
        await using var provider = BuildServices(Guid.NewGuid().ToString());

        await SeedAsync(provider);
        int recipes, posts, users;
        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            recipes = await db.Recipes.CountAsync();
            posts = await db.BlogPosts.CountAsync();
            users = await db.Users.CountAsync();
        }

        await SeedAsync(provider);

        await using var verify = provider.CreateAsyncScope();
        var verifyDb = verify.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(recipes, await verifyDb.Recipes.CountAsync());
        Assert.Equal(posts, await verifyDb.BlogPosts.CountAsync());
        Assert.Equal(users, await verifyDb.Users.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_ExistingUserWithoutAdminRole_IsPromotedToAdmin()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var provider = BuildServices(databaseName);
        await using (var scope = provider.CreateAsyncScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var created = await users.CreateAsync(
                new ApplicationUser { UserName = AdminEmail, Email = AdminEmail },
                AdminPassword);
            Assert.True(created.Succeeded);
        }

        await SeedAsync(provider);

        await using var verify = provider.CreateAsyncScope();
        var verifyUsers = verify.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await verifyUsers.FindByEmailAsync(AdminEmail);
        Assert.True(await verifyUsers.IsInRoleAsync(admin!, Roles.Admin));
    }
}
