namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public required string Name { get; set; }
        public TimeSpan TimeToCook { get; set; }
        public bool IsDeleted { get; set; }
        public required string Method { get; set; }
        public required ICollection<Ingredient> Ingredients { get; set; }
        public DateTime LastModified { get; set; }
    }
}
