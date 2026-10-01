using LowCarbLife.Api.Data;
using LowCarbLife.Api.Features.Recipes;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Tests.Recipes;

public class RecipesControllerTests
{
    private readonly TestDb _db = new();

    private RecipesController CreateController() => new(_db.NewContext());

    private static UpsertRecipeRequest Request(
        string title = "Jajecznica",
        string description = "Szybkie śniadanie",
        string instructions = "Usmaż jajka.",
        string? imageUrl = null,
        DietType diet = DietType.Keto,
        MealCategory meal = MealCategory.Sniadanie,
        NutritionDto? nutrition = null,
        IReadOnlyList<IngredientDto>? ingredients = null) =>
        new(title, description, instructions, imageUrl, diet, meal, nutrition, ingredients);

    private async Task<Recipe> SeedAsync(
        string title,
        DietType diet = DietType.Keto,
        MealCategory meal = MealCategory.Sniadanie,
        params (string Name, string? Amount, int Order)[] ingredients)
    {
        await using var db = _db.NewContext();
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = $"Opis: {title}",
            Instructions = "Instrukcja",
            DietType = diet,
            MealCategory = meal,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var (name, amount, order) in ingredients)
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                Name = name,
                Amount = amount,
                SortOrder = order
            });
        }

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe;
    }

    // ---------- List ----------

    [Fact]
    public async Task List_WithoutFilters_ReturnsAllRecipesOrderedByTitle()
    {
        await SeedAsync("Cebula");
        await SeedAsync("Awokado");
        await SeedAsync("Boczek");

        var result = await CreateController().List(null, null);

        var items = Assert.IsAssignableFrom<IReadOnlyList<RecipeListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(["Awokado", "Boczek", "Cebula"], items.Select(i => i.Title));
    }

    [Fact]
    public async Task List_FiltersByDiet()
    {
        await SeedAsync("Keto 1", DietType.Keto);
        await SeedAsync("Lowcarb 1", DietType.LowCarb);

        var result = await CreateController().List(DietType.LowCarb, null);

        var items = Assert.IsAssignableFrom<IReadOnlyList<RecipeListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        var item = Assert.Single(items);
        Assert.Equal("Lowcarb 1", item.Title);
        Assert.Equal(DietType.LowCarb, item.DietType);
    }

    [Fact]
    public async Task List_FiltersByMeal()
    {
        await SeedAsync("Rano", meal: MealCategory.Sniadanie);
        await SeedAsync("Wieczorem", meal: MealCategory.Kolacja);

        var result = await CreateController().List(null, MealCategory.Kolacja);

        var items = Assert.IsAssignableFrom<IReadOnlyList<RecipeListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Wieczorem", Assert.Single(items).Title);
    }

    [Fact]
    public async Task List_CombinesDietAndMealFilters()
    {
        await SeedAsync("Keto sałatka", DietType.Keto, MealCategory.Salatki);
        await SeedAsync("Keto obiad", DietType.Keto, MealCategory.Obiad);
        await SeedAsync("Lowcarb obiad", DietType.LowCarb, MealCategory.Obiad);

        var result = await CreateController().List(DietType.Keto, MealCategory.Obiad);

        var items = Assert.IsAssignableFrom<IReadOnlyList<RecipeListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Keto obiad", Assert.Single(items).Title);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await CreateController().List(null, null);

        var items = Assert.IsAssignableFrom<IReadOnlyList<RecipeListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Empty(items);
    }

    // ---------- Get ----------

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Get(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Get_ExistingRecipe_ReturnsDetailsWithIngredientsInSortOrder()
    {
        var recipe = await SeedAsync(
            "Muffiny",
            DietType.Keto,
            MealCategory.Przekaski,
            ("Jajka", "3 sztuki", 1),
            ("Ser", "50 g", 0),
            ("Sól", null, 2));

        var result = await CreateController().Get(recipe.Id);

        var detail = Assert.IsType<RecipeDetailDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(recipe.Id, detail.Id);
        Assert.Equal("Muffiny", detail.Title);
        Assert.Equal(MealCategory.Przekaski, detail.MealCategory);
        Assert.Equal(["Ser", "Jajka", "Sól"], detail.Ingredients.Select(i => i.Name));
        Assert.Null(detail.Ingredients[2].Amount);
    }

    [Fact]
    public async Task Get_MapsNutritionValues()
    {
        Guid id;
        await using (var db = _db.NewContext())
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Title = "Z makro",
                Description = "d",
                Instructions = "i",
                DietType = DietType.Keto,
                MealCategory = MealCategory.Obiad,
                CaloriesKcal = 520.5m,
                ProteinGrams = 30m,
                FatGrams = 40m,
                CarbsGrams = 6m,
                FiberGrams = 4m,
                CreatedAt = DateTime.UtcNow
            };
            db.Recipes.Add(recipe);
            await db.SaveChangesAsync();
            id = recipe.Id;
        }

        var result = await CreateController().Get(id);

        var detail = Assert.IsType<RecipeDetailDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(new NutritionDto(520.5m, 30m, 40m, 6m, 4m), detail.Nutrition);
    }

    // ---------- Create ----------

    [Fact]
    public async Task Create_ValidRequest_PersistsRecipeAndReturnsCreated()
    {
        var request = Request(
            title: "  Jajecznica  ",
            description: "  Opis  ",
            instructions: "  Krok 1  ",
            imageUrl: "  /recipes/jajecznica.png  ",
            nutrition: new NutritionDto(300m, 20m, 25m, 3m, 1m),
            ingredients: [new IngredientDto("  Jajka ", " 3 sztuki "), new IngredientDto("Masło", "10 g")]);

        var result = await CreateController().Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(RecipesController.Get), created.ActionName);
        var detail = Assert.IsType<RecipeDetailDto>(created.Value);
        Assert.Equal("Jajecznica", detail.Title);
        Assert.Equal("/recipes/jajecznica.png", detail.ImageUrl);

        await using var db = _db.NewContext();
        var stored = await db.Recipes.Include(r => r.Ingredients).SingleAsync();
        Assert.Equal(detail.Id, stored.Id);
        Assert.Equal("Jajecznica", stored.Title);
        Assert.Equal("Opis", stored.Description);
        Assert.Equal("Krok 1", stored.Instructions);
        Assert.Equal(300m, stored.CaloriesKcal);
        Assert.Equal(20m, stored.ProteinGrams);
        Assert.Equal(25m, stored.FatGrams);
        Assert.Equal(3m, stored.CarbsGrams);
        Assert.Equal(1m, stored.FiberGrams);
        Assert.Equal(
            [("Jajka", "3 sztuki", 0), ("Masło", "10 g", 1)],
            stored.Ingredients.OrderBy(i => i.SortOrder).Select(i => (i.Name, i.Amount, i.SortOrder)));
    }

    [Fact]
    public async Task Create_SetsCreatedAtAndLeavesUpdatedAtEmpty()
    {
        var before = DateTime.UtcNow;

        await CreateController().Create(Request());

        await using var db = _db.NewContext();
        var stored = await db.Recipes.SingleAsync();
        Assert.True(stored.CreatedAt >= before);
        Assert.Null(stored.UpdatedAt);
    }

    [Fact]
    public async Task Create_BlankImageUrl_StoresNull()
    {
        await CreateController().Create(Request(imageUrl: "   "));

        await using var db = _db.NewContext();
        Assert.Null((await db.Recipes.SingleAsync()).ImageUrl);
    }

    [Fact]
    public async Task Create_WithoutNutritionAndIngredients_StoresEmptyValues()
    {
        await CreateController().Create(Request(nutrition: null, ingredients: null));

        await using var db = _db.NewContext();
        var stored = await db.Recipes.Include(r => r.Ingredients).SingleAsync();
        Assert.Null(stored.CaloriesKcal);
        Assert.Null(stored.ProteinGrams);
        Assert.Null(stored.FatGrams);
        Assert.Null(stored.CarbsGrams);
        Assert.Null(stored.FiberGrams);
        Assert.Empty(stored.Ingredients);
    }

    [Fact]
    public async Task Create_SkipsIngredientsWithBlankNames()
    {
        var request = Request(ingredients:
        [
            new IngredientDto("Jajka", "2"),
            new IngredientDto("   ", "1"),
            new IngredientDto("", null),
            new IngredientDto("Ser", null)
        ]);

        await CreateController().Create(request);

        await using var db = _db.NewContext();
        var stored = await db.Recipes.Include(r => r.Ingredients).SingleAsync();
        Assert.Equal(
            [("Jajka", 0), ("Ser", 1)],
            stored.Ingredients.OrderBy(i => i.SortOrder).Select(i => (i.Name, i.SortOrder)));
    }

    [Theory]
    [InlineData("", "opis", "instrukcja")]
    [InlineData("   ", "opis", "instrukcja")]
    [InlineData("tytuł", "", "instrukcja")]
    [InlineData("tytuł", "   ", "instrukcja")]
    [InlineData("tytuł", "opis", "")]
    [InlineData("tytuł", "opis", "   ")]
    public async Task Create_MissingRequiredText_ReturnsBadRequestAndDoesNotSave(
        string title, string description, string instructions)
    {
        var result = await CreateController().Create(Request(title, description, instructions));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Tytuł, opis i przygotowanie są wymagane.", bad.Value);
        await using var db = _db.NewContext();
        Assert.Empty(db.Recipes);
    }

    [Fact]
    public async Task Create_UndefinedDiet_ReturnsBadRequest()
    {
        var result = await CreateController().Create(Request(diet: (DietType)99));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Nieprawidłowa dieta lub kategoria posiłku.", bad.Value);
    }

    [Fact]
    public async Task Create_UndefinedMealCategory_ReturnsBadRequest()
    {
        var result = await CreateController().Create(Request(meal: (MealCategory)0));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Nieprawidłowa dieta lub kategoria posiłku.", bad.Value);
    }

    [Theory]
    [InlineData(MealCategory.Przekaski)]
    [InlineData(MealCategory.Smoothie)]
    [InlineData(MealCategory.Desery)]
    [InlineData(MealCategory.Salatki)]
    [InlineData(MealCategory.Zupy)]
    [InlineData(MealCategory.Lunchboxy)]
    public async Task Create_KetoOnlyMealWithLowCarb_ReturnsBadRequest(MealCategory meal)
    {
        var result = await CreateController().Create(Request(diet: DietType.LowCarb, meal: meal));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Ta kategoria jest dostępna tylko w przepisach keto.", bad.Value);
        await using var db = _db.NewContext();
        Assert.Empty(db.Recipes);
    }

    [Theory]
    [InlineData(MealCategory.Przekaski)]
    [InlineData(MealCategory.Smoothie)]
    [InlineData(MealCategory.Desery)]
    [InlineData(MealCategory.Salatki)]
    [InlineData(MealCategory.Zupy)]
    [InlineData(MealCategory.Lunchboxy)]
    public async Task Create_KetoOnlyMealWithKeto_IsAccepted(MealCategory meal)
    {
        var result = await CreateController().Create(Request(diet: DietType.Keto, meal: meal));

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Theory]
    [InlineData(MealCategory.Sniadanie)]
    [InlineData(MealCategory.Obiad)]
    [InlineData(MealCategory.Kolacja)]
    public async Task Create_CoreMealWithLowCarb_IsAccepted(MealCategory meal)
    {
        var result = await CreateController().Create(Request(diet: DietType.LowCarb, meal: meal));

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    // ---------- Update ----------

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Update(Guid.NewGuid(), Request());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_InvalidRequest_ReturnsBadRequestAndKeepsRecipe()
    {
        var recipe = await SeedAsync("Oryginał");

        var result = await CreateController().Update(recipe.Id, Request(title: ""));

        Assert.IsType<BadRequestObjectResult>(result.Result);
        await using var db = _db.NewContext();
        Assert.Equal("Oryginał", (await db.Recipes.SingleAsync()).Title);
    }

    [Fact]
    public async Task Update_KetoOnlyMealForLowCarb_ReturnsBadRequest()
    {
        var recipe = await SeedAsync("Lowcarb obiad", DietType.LowCarb, MealCategory.Obiad);

        var result = await CreateController().Update(
            recipe.Id,
            Request(diet: DietType.LowCarb, meal: MealCategory.Salatki));

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReplacesFieldsAndIngredientsAndSetsUpdatedAt()
    {
        var recipe = await SeedAsync(
            "Stary tytuł",
            DietType.Keto,
            MealCategory.Sniadanie,
            ("Stary składnik", "1", 0));
        var request = Request(
            title: " Nowy tytuł ",
            description: "Nowy opis",
            instructions: "Nowa instrukcja",
            diet: DietType.LowCarb,
            meal: MealCategory.Obiad,
            nutrition: new NutritionDto(100m, 10m, 5m, 2m, 1m),
            ingredients: [new IngredientDto("Nowy A", "1"), new IngredientDto("Nowy B", null)]);

        var result = await CreateController().Update(recipe.Id, request);

        var detail = Assert.IsType<RecipeDetailDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Nowy tytuł", detail.Title);
        Assert.Equal(DietType.LowCarb, detail.DietType);
        Assert.Equal(MealCategory.Obiad, detail.MealCategory);
        Assert.Equal(["Nowy A", "Nowy B"], detail.Ingredients.Select(i => i.Name));

        await using var db = _db.NewContext();
        var stored = await db.Recipes.Include(r => r.Ingredients).SingleAsync();
        Assert.Equal("Nowy tytuł", stored.Title);
        Assert.Equal(100m, stored.CaloriesKcal);
        Assert.NotNull(stored.UpdatedAt);
        Assert.Equal(["Nowy A", "Nowy B"], stored.Ingredients.OrderBy(i => i.SortOrder).Select(i => i.Name));
        Assert.Equal(2, await db.RecipeIngredients.CountAsync());
    }

    [Fact]
    public async Task Update_ClearingNutritionAndImage_StoresNulls()
    {
        Guid id;
        await using (var db = _db.NewContext())
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Title = "t",
                Description = "d",
                Instructions = "i",
                ImageUrl = "/recipes/a.png",
                DietType = DietType.Keto,
                MealCategory = MealCategory.Obiad,
                CaloriesKcal = 500m,
                CreatedAt = DateTime.UtcNow
            };
            db.Recipes.Add(recipe);
            await db.SaveChangesAsync();
            id = recipe.Id;
        }

        await CreateController().Update(id, Request(meal: MealCategory.Obiad, imageUrl: null, nutrition: null));

        await using var verify = _db.NewContext();
        var stored = await verify.Recipes.SingleAsync();
        Assert.Null(stored.ImageUrl);
        Assert.Null(stored.CaloriesKcal);
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingRecipe_RemovesItAndReturnsNoContent()
    {
        var keep = await SeedAsync("Zostaje");
        var remove = await SeedAsync("Do usunięcia");

        var result = await CreateController().Delete(remove.Id);

        Assert.IsType<NoContentResult>(result);
        await using var db = _db.NewContext();
        var remaining = await db.Recipes.SingleAsync();
        Assert.Equal(keep.Id, remaining.Id);
    }
}
