namespace LowCarbLife.Api.Features.Recipes;

public class Recipe
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DietType DietType { get; set; }
    public MealCategory MealCategory { get; set; }
    public decimal? CaloriesKcal { get; set; }
    public decimal? ProteinGrams { get; set; }
    public decimal? FatGrams { get; set; }
    public decimal? CarbsGrams { get; set; }
    public decimal? FiberGrams { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; } = [];
}

public class RecipeIngredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Amount { get; set; }
    public int SortOrder { get; set; }
}
