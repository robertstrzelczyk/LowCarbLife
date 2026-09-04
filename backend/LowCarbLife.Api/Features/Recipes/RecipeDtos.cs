namespace LowCarbLife.Api.Features.Recipes;

public record IngredientDto(string Name, string? Amount);

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
    string? YoutubeUrl,
    string? ImageUrl,
    DietType DietType,
    MealCategory MealCategory,
    IReadOnlyList<IngredientDto> Ingredients);

public record UpsertRecipeRequest(
    string Title,
    string Description,
    string Instructions,
    string? YoutubeUrl,
    string? ImageUrl,
    DietType DietType,
    MealCategory MealCategory,
    IReadOnlyList<IngredientDto>? Ingredients);
