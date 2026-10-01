using LowCarbLife.Api.Data;
using LowCarbLife.Api.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Features.Recipes;

[ApiController]
[Route("api/recipes")]
public class RecipesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<RecipeListItemDto>>> List(
        [FromQuery] DietType? diet,
        [FromQuery] MealCategory? meal)
    {
        var query = db.Recipes.AsNoTracking().AsQueryable();

        if (diet.HasValue)
        {
            query = query.Where(r => r.DietType == diet.Value);
        }

        if (meal.HasValue)
        {
            query = query.Where(r => r.MealCategory == meal.Value);
        }

        var recipes = await query
            .OrderBy(r => r.Title)
            .Select(r => new RecipeListItemDto(
                r.Id,
                r.Title,
                r.Description,
                r.ImageUrl,
                r.DietType,
                r.MealCategory))
            .ToListAsync();

        return Ok(recipes);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<RecipeDetailDto>> Get(Guid id)
    {
        var recipe = await db.Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        return recipe is null ? NotFound() : Ok(ToDetail(recipe));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<RecipeDetailDto>> Create(UpsertRecipeRequest request)
    {
        if (!TryValidate(request, out var error))
        {
            return BadRequest(error);
        }

        var recipe = new Recipe { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        Apply(recipe, request);
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = recipe.Id }, ToDetail(recipe));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<RecipeDetailDto>> Update(Guid id, UpsertRecipeRequest request)
    {
        if (!TryValidate(request, out var error))
        {
            return BadRequest(error);
        }

        var recipe = await db.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        db.RecipeIngredients.RemoveRange(recipe.Ingredients);
        recipe.Ingredients.Clear();
        recipe.UpdatedAt = DateTime.UtcNow;
        Apply(recipe, request);
        await db.SaveChangesAsync();
        return Ok(ToDetail(recipe));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id);
        if (recipe is null)
        {
            return NotFound();
        }

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool TryValidate(UpsertRecipeRequest request, out string error)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Description) ||
            string.IsNullOrWhiteSpace(request.Instructions))
        {
            error = "Tytuł, opis i przygotowanie są wymagane.";
            return false;
        }

        if (!Enum.IsDefined(request.DietType) || !Enum.IsDefined(request.MealCategory))
        {
            error = "Nieprawidłowa dieta lub kategoria posiłku.";
            return false;
        }

        if (request.DietType != DietType.Keto && IsKetoOnlyMeal(request.MealCategory))
        {
            error = "Ta kategoria jest dostępna tylko w przepisach keto.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static void Apply(Recipe recipe, UpsertRecipeRequest request)
    {
        recipe.Title = request.Title.Trim();
        recipe.Description = request.Description.Trim();
        recipe.Instructions = request.Instructions.Trim();
        recipe.ImageUrl = NullIfEmpty(request.ImageUrl);
        recipe.DietType = request.DietType;
        recipe.MealCategory = request.MealCategory;
        recipe.CaloriesKcal = request.Nutrition?.CaloriesKcal;
        recipe.ProteinGrams = request.Nutrition?.ProteinGrams;
        recipe.FatGrams = request.Nutrition?.FatGrams;
        recipe.CarbsGrams = request.Nutrition?.CarbsGrams;
        recipe.FiberGrams = request.Nutrition?.FiberGrams;
        recipe.Ingredients = (request.Ingredients ?? [])
            .Where(i => !string.IsNullOrWhiteSpace(i.Name))
            .Select((i, index) => new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                Name = i.Name.Trim(),
                Amount = NullIfEmpty(i.Amount),
                SortOrder = index
            })
            .ToList();
    }

    private static RecipeDetailDto ToDetail(Recipe recipe) =>
        new(
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.Instructions,
            recipe.ImageUrl,
            recipe.DietType,
            recipe.MealCategory,
            new NutritionDto(
                recipe.CaloriesKcal,
                recipe.ProteinGrams,
                recipe.FatGrams,
                recipe.CarbsGrams,
                recipe.FiberGrams),
            recipe.Ingredients
                .OrderBy(i => i.SortOrder)
                .Select(i => new IngredientDto(i.Name, i.Amount))
                .ToList());

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsKetoOnlyMeal(MealCategory meal) =>
        meal is MealCategory.Przekaski
            or MealCategory.Smoothie
            or MealCategory.Desery
            or MealCategory.Salatki
            or MealCategory.Zupy
            or MealCategory.Lunchboxy;
}
