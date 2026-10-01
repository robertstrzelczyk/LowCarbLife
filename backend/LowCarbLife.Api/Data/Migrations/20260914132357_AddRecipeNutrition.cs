using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowCarbLife.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeNutrition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "Recipes");

            migrationBuilder.AddColumn<decimal>(
                name: "CaloriesKcal",
                table: "Recipes",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarbsGrams",
                table: "Recipes",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FatGrams",
                table: "Recipes",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FiberGrams",
                table: "Recipes",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProteinGrams",
                table: "Recipes",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesKcal",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CarbsGrams",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "FatGrams",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "FiberGrams",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ProteinGrams",
                table: "Recipes");

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
