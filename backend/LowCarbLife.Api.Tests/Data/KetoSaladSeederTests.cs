using System.Runtime.CompilerServices;
using LowCarbLife.Api.Data;
using LowCarbLife.Api.Features.Recipes;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Tests.Data;

public class KetoSaladSeederTests
{
    private readonly TestDb _db = new();

    private async Task<List<Recipe>> SeedAndLoadAsync()
    {
        await using (var db = _db.NewContext())
        {
            await KetoSaladSeeder.SeedAsync(db);
            await db.SaveChangesAsync();
        }

        await using var verify = _db.NewContext();
        return await verify.Recipes.Include(r => r.Ingredients).ToListAsync();
    }

    [Fact]
    public async Task SeedAsync_AddsKetoSaladRecipes()
    {
        var recipes = await SeedAndLoadAsync();

        Assert.NotEmpty(recipes);
        Assert.All(recipes, r =>
        {
            Assert.Equal(DietType.Keto, r.DietType);
            Assert.Equal(MealCategory.Salatki, r.MealCategory);
        });
    }

    [Fact]
    public async Task SeedAsync_RecipesHaveCompleteContent()
    {
        var recipes = await SeedAndLoadAsync();

        Assert.All(recipes, r =>
        {
            Assert.False(string.IsNullOrWhiteSpace(r.Title));
            Assert.False(string.IsNullOrWhiteSpace(r.Description));
            Assert.False(string.IsNullOrWhiteSpace(r.Instructions));
            Assert.NotEmpty(r.Ingredients);
            Assert.All(r.Ingredients, i => Assert.False(string.IsNullOrWhiteSpace(i.Name)));
        });
    }

    [Fact]
    public async Task SeedAsync_TitlesAreUnique()
    {
        var recipes = await SeedAndLoadAsync();

        Assert.Equal(recipes.Count, recipes.Select(r => r.Title).Distinct().Count());
    }

    [Fact]
    public async Task SeedAsync_IngredientsAreOrderedSequentiallyFromZero()
    {
        var recipes = await SeedAndLoadAsync();

        Assert.All(recipes, r =>
            Assert.Equal(
                Enumerable.Range(0, r.Ingredients.Count),
                r.Ingredients.Select(i => i.SortOrder).OrderBy(order => order)));
    }

    [Fact]
    public async Task SeedAsync_FitsDatabaseColumnLimits()
    {
        var recipes = await SeedAndLoadAsync();

        Assert.All(recipes, r =>
        {
            Assert.InRange(r.Title.Length, 1, 200);
            Assert.True(r.ImageUrl is null || r.ImageUrl.Length <= 500);
            Assert.All(r.Ingredients, i =>
            {
                Assert.InRange(i.Name.Length, 1, 200);
                Assert.True(i.Amount is null || i.Amount.Length <= 100);
            });
        });
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicateRecipes()
    {
        var first = await SeedAndLoadAsync();
        var second = await SeedAndLoadAsync();

        Assert.Equal(first.Count, second.Count);
    }

    [Fact]
    public async Task SeedAsync_SkipsRecipesThatAlreadyExist()
    {
        var all = await SeedAndLoadAsync();
        var existingTitle = all[0].Title;

        var other = new TestDb();
        await using (var db = other.NewContext())
        {
            db.Recipes.Add(new Recipe
            {
                Id = Guid.NewGuid(),
                Title = existingTitle,
                Description = "Własny opis",
                Instructions = "Własna instrukcja",
                DietType = DietType.Keto,
                MealCategory = MealCategory.Salatki,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            await KetoSaladSeeder.SeedAsync(db);
            await db.SaveChangesAsync();
        }

        await using var verify = other.NewContext();
        Assert.Equal(all.Count, await verify.Recipes.CountAsync());
        var kept = await verify.Recipes.SingleAsync(r => r.Title == existingTitle);
        Assert.Equal("Własny opis", kept.Description);
    }

    [Fact]
    public async Task SeedAsync_ReferencedImagesExistInFrontendPublicFolder()
    {
        var recipesFolder = FindRecipeImagesFolder();
        var recipes = await SeedAndLoadAsync();

        Assert.All(recipes, r =>
        {
            Assert.NotNull(r.ImageUrl);
            Assert.StartsWith("/recipes/", r.ImageUrl);
            var file = Path.Combine(recipesFolder, Path.GetFileName(r.ImageUrl!));
            Assert.True(File.Exists(file), $"Brak pliku zdjęcia dla „{r.Title}”: {file}");
        });
    }

    private static string FindRecipeImagesFolder([CallerFilePath] string sourceFile = "")
    {
        // Start od pliku źródłowego (ścieżka wstawiana w czasie kompilacji), dzięki czemu
        // test działa niezależnie od tego, gdzie trafiają artefakty budowania.
        for (var dir = new DirectoryInfo(Path.GetDirectoryName(sourceFile)!); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "frontend", "public", "recipes");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Nie znaleziono frontend/public/recipes w drzewie katalogów repozytorium.");
    }
}
