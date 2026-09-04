using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Features.Blog;
using LowCarbLife.Api.Features.Contact;
using LowCarbLife.Api.Features.Recipes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BlogPost>(entity =>
        {
            entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Content).IsRequired();
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            entity.Property(p => p.AuthorId).HasMaxLength(450).IsRequired();
            entity.HasIndex(p => p.CreatedAt);
        });

        builder.Entity<Recipe>(entity =>
        {
            entity.Property(r => r.Title).HasMaxLength(200).IsRequired();
            entity.Property(r => r.Description).IsRequired();
            entity.Property(r => r.Instructions).IsRequired();
            entity.Property(r => r.YoutubeUrl).HasMaxLength(500);
            entity.Property(r => r.ImageUrl).HasMaxLength(500);
            entity.Property(r => r.DietType).HasConversion<string>().HasMaxLength(20);
            entity.Property(r => r.MealCategory).HasConversion<string>().HasMaxLength(20);
            entity.HasMany(r => r.Ingredients)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RecipeIngredient>(entity =>
        {
            entity.Property(i => i.Name).HasMaxLength(200).IsRequired();
            entity.Property(i => i.Amount).HasMaxLength(100);
        });

        builder.Entity<ContactMessage>(entity =>
        {
            entity.Property(m => m.Name).HasMaxLength(120).IsRequired();
            entity.Property(m => m.Email).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Message).IsRequired();
        });
    }
}
