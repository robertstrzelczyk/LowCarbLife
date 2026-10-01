namespace LowCarbLife.Api.Features.Recipes;

public record IngredientDto(string Name, string? Amount);

public record NutritionDto(
    decimal? CaloriesKcal,
    decimal? ProteinGrams,
    decimal? FatGrams,
    decimal? CarbsGrams,
    decimal? FiberGrams);

public record RecipeListItemDto(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    DietType DietType,
    MealCategory MealCategory);

public record RecipeDetailDto(
    Guid Id,
    string Title,
    string Description,
    string Instructions,
    string? ImageUrl,
    DietType DietType,
    MealCategory MealCategory,
    NutritionDto Nutrition,
    IReadOnlyList<IngredientDto> Ingredients);

public record UpsertRecipeRequest(
    string Title,
    string Description,
    string Instructions,
    string? ImageUrl,
    DietType DietType,
    MealCategory MealCategory,
    NutritionDto? Nutrition,
    IReadOnlyList<IngredientDto>? Ingredients);
