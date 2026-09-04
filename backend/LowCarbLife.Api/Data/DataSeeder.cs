using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Features.Blog;
using LowCarbLife.Api.Features.Recipes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<AppDbContext>();
        var config = services.GetRequiredService<IConfiguration>();

        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = config["Seed:AdminEmail"] ?? "admin@lowcarblife.local";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin123!";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Admin",
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(admin, adminPassword);
            if (created.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
        else if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        if (!await db.BlogPosts.AnyAsync())
        {
            db.BlogPosts.Add(new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = "Pierwsze tygodnie keto i bieganie",
                Content = """
                Ruszyłem z niskowęglowodanowym stylem życia równolegle z regularnym bieganiem. Pierwsze dni to mieszanka euforii i lekkiego spowolnienia — organizm przestawiał się z cukru na tłuszcz.

                Na treningach trzymałem się spokojnego tempa. Zamiast walczyć o życiówki, słuchałem ciała: więcej snu, więcej elektrolitów, mniej „sportowych” żeli.

                Po około dwóch tygodniach wróciła energia. Bieganie stało się lżejsze, a wieczory bez podjadania — zaskakująco proste. Ten blog ma być dziennikiem takich odczuć, bez cudownych obietnic.
                """,
                ImageUrl = "https://images.unsplash.com/photo-1476480862126-209bfaa8edc8?auto=format&fit=crop&w=1200&q=80",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                AuthorId = admin!.Id
            });
        }

        if (!await db.Recipes.AnyAsync())
        {
            db.Recipes.AddRange(
                new Recipe
                {
                    Id = Guid.NewGuid(),
                    Title = "Jajecznica ze szpinakiem i awokado",
                    Description = "Szybkie keto śniadanie, które syci przed porannym treningiem.",
                    Instructions = """
                    1. Rozpuść masło na patelni.
                    2. Dodaj szpinak i chwilę podsmaż, aż zwiędnie.
                    3. Wlej jajka, mieszaj na małym ogniu.
                    4. Dopraw solą i pieprzem, podawaj z pokrojonym awokado.
                    """,
                    YoutubeUrl = "https://www.youtube.com/watch?v=qCLbUbgK1YM",
                    ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?auto=format&fit=crop&w=1200&q=80",
                    DietType = DietType.Keto,
                    MealCategory = MealCategory.Sniadanie,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Ingredients =
                    [
                        new() { Id = Guid.NewGuid(), Name = "Jajka", Amount = "3 sztuki", SortOrder = 0 },
                        new() { Id = Guid.NewGuid(), Name = "Szpinak baby", Amount = "2 garście", SortOrder = 1 },
                        new() { Id = Guid.NewGuid(), Name = "Masło", Amount = "1 łyżka", SortOrder = 2 },
                        new() { Id = Guid.NewGuid(), Name = "Awokado", Amount = "1/2 sztuki", SortOrder = 3 }
                    ]
                },
                new Recipe
                {
                    Id = Guid.NewGuid(),
                    Title = "Kurczak z cukinią i pesto",
                    Description = "Lowcarb obiad po bieganiu — białko i warzywa bez zbędnych węgli.",
                    Instructions = """
                    1. Pokrój kurczaka w paski i oprósz solą oraz pieprzem.
                    2. Smaż na oliwie, aż się zarumieni.
                    3. Dodaj cukinię i duś kilka minut.
                    4. Na koniec wymieszaj z pesto i posyp parmezanem.
                    """,
                    ImageUrl = "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=1200&q=80",
                    DietType = DietType.LowCarb,
                    MealCategory = MealCategory.Obiad,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Ingredients =
                    [
                        new() { Id = Guid.NewGuid(), Name = "Filet z kurczaka", Amount = "200 g", SortOrder = 0 },
                        new() { Id = Guid.NewGuid(), Name = "Cukinia", Amount = "1 sztuka", SortOrder = 1 },
                        new() { Id = Guid.NewGuid(), Name = "Pesto", Amount = "2 łyżki", SortOrder = 2 },
                        new() { Id = Guid.NewGuid(), Name = "Oliwa", Amount = "1 łyżka", SortOrder = 3 }
                    ]
                },
                new Recipe
                {
                    Id = Guid.NewGuid(),
                    Title = "Łosoś pieczony z brokułem",
                    Description = "Keto kolacja bogata w tłuszcze omega-3, bez kombinowania.",
                    Instructions = """
                    1. Rozgrzej piekarnik do 200°C.
                    2. Połóż łososia i różyczki brokułu na blachę.
                    3. Polej oliwą, dopraw solą, pieprzem i sokiem z cytryny.
                    4. Piecz 15–18 minut.
                    """,
                    ImageUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?auto=format&fit=crop&w=1200&q=80",
                    DietType = DietType.Keto,
                    MealCategory = MealCategory.Kolacja,
                    CreatedAt = DateTime.UtcNow,
                    Ingredients =
                    [
                        new() { Id = Guid.NewGuid(), Name = "Filet z łososia", Amount = "180 g", SortOrder = 0 },
                        new() { Id = Guid.NewGuid(), Name = "Brokuł", Amount = "1 mały", SortOrder = 1 },
                        new() { Id = Guid.NewGuid(), Name = "Oliwa z oliwek", Amount = "1 łyżka", SortOrder = 2 },
                        new() { Id = Guid.NewGuid(), Name = "Cytryna", Amount = "kilka kropel", SortOrder = 3 }
                    ]
                });
        }

        await db.SaveChangesAsync();
    }
}
