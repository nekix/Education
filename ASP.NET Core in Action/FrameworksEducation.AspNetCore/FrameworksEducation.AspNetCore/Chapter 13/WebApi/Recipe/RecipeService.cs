namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Models;

public partial class RecipeService
{
    private static readonly List<Recipe> _recipes = new();
    private static int _nextId = 1;

    private readonly ILogger<RecipeService> _logger;

    [LoggerMessage(Level = LogLevel.Debug, Message = "Create recipe command start. Params: {name} {timeToCookHrs} {timeToCookMins} {method}.")]
    public static partial void StartCreateRecipe(ILogger logger, string name, int timeToCookHrs, int timeToCookMins, string method);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Create recipe command end. Params: {name} {timeToCookHrs} {timeToCookMins} {method}.")]
    public static partial void EndCreateRecipe(ILogger logger, string name, int timeToCookHrs, int timeToCookMins, string method);


    public RecipeService(ILogger<RecipeService> logger)
    {
        _logger = logger;

        // Добавляем начальные рецепты, если список пуст
        if (!_recipes.Any())
        {
            _recipes.Add(new Recipe
            {
                RecipeId = _nextId++,
                Name = "Борщ",
                TimeToCook = new TimeSpan(2, 30, 0),
                Method = "1. Нарезать овощи\n2. Сварить бульон\n3. Добавить овощи\n4. Варить до готовности",
                IsDeleted = false,
                LastModified = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Свекла", Quantity = 2, Unit = "шт" },
                    new() { Name = "Картофель", Quantity = 4, Unit = "шт" },
                    new() { Name = "Капуста", Quantity = 300, Unit = "г" },
                    new() { Name = "Морковь", Quantity = 1, Unit = "шт" },
                    new() { Name = "Лук", Quantity = 1, Unit = "шт" }
                }
            });

            _recipes.Add(new Recipe
            {
                RecipeId = _nextId++,
                Name = "Оливье",
                TimeToCook = new TimeSpan(1, 0, 0),
                Method = "1. Отварить овощи\n2. Нарезать все ингредиенты\n3. Смешать с майонезом",
                IsDeleted = false,
                LastModified = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Картофель", Quantity = 4, Unit = "шт" },
                    new() { Name = "Морковь", Quantity = 2, Unit = "шт" },
                    new() { Name = "Яйца", Quantity = 4, Unit = "шт" },
                    new() { Name = "Огурцы", Quantity = 2, Unit = "шт" },
                    new() { Name = "Горошек", Quantity = 200, Unit = "г" },
                    new() { Name = "Майонез", Quantity = 200, Unit = "г" }
                }
            });

            _recipes.Add(new Recipe
            {
                RecipeId = _nextId++,
                Name = "Пельмени",
                TimeToCook = new TimeSpan(1, 30, 0),
                Method = "1. Приготовить тесто\n2. Сделать фарш\n3. Слепить пельмени\n4. Отварить в подсоленной воде",
                IsDeleted = false,
                LastModified = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Мука", Quantity = 500, Unit = "г" },
                    new() { Name = "Фарш мясной", Quantity = 500, Unit = "г" },
                    new() { Name = "Лук", Quantity = 2, Unit = "шт" },
                    new() { Name = "Яйцо", Quantity = 1, Unit = "шт" },
                    new() { Name = "Соль", Quantity = 1, Unit = "ч.л" }
                }
            });
        }
    }

    //public required string Name { get; set; }
    //public int TimeToCookHrs { get; set; }
    //public int TimeToCookMins { get; set; }
    //public bool IsDeleted { get; set; }
    //public required string Method { get; set; }
    //public required ICollection<Ingredient> Ingredients { get; set; }


    public int CreateRecipe(CreateRecipeCommand cmd)
    {
        StartCreateRecipe(_logger, cmd.Name, cmd.TimeToCookHrs, cmd.TimeToCookMins, cmd.Method);

        Recipe recipe = new Recipe
        {
            RecipeId = _nextId++,
            Name = cmd.Name,
            TimeToCook = new TimeSpan(
                cmd.TimeToCookHrs, cmd.TimeToCookMins, 0),
            Method = cmd.Method,
            Ingredients = cmd.Ingredients.Select(i => new Ingredient
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit,
            }).ToList(),
            LastModified = DateTime.UtcNow
        };

        _recipes.Add(recipe);

        EndCreateRecipe(_logger, cmd.Name, cmd.TimeToCookHrs, cmd.TimeToCookMins, cmd.Method);

        return recipe.RecipeId;
    }

    public bool DoesRecipeExist(int id)
    {
        return _recipes.Any(r => r.RecipeId == id && !r.IsDeleted);
    }

    public ICollection<RecipeSummaryViewModel> GetRecipes()
    {
        return _recipes
            .Where(r => !r.IsDeleted)
            .Select(r => new RecipeSummaryViewModel
            {
                Id = r.RecipeId,
                Name = r.Name,
                TimeToCook = $"{r.TimeToCook.Hours}hrs {r.TimeToCook.Minutes}mins"
            })
            .ToList();
    }

    public RecipeDetailViewModel GetRecipeDetail(int id)
    {
        var recipe = _recipes.FirstOrDefault(r => r.RecipeId == id && !r.IsDeleted);
        if (recipe == null)
            return null;

        return new RecipeDetailViewModel
        {
            Id = recipe.RecipeId,
            Name = recipe.Name,
            Method = recipe.Method,
            LastModified = recipe.LastModified,
            Ingredients = recipe.Ingredients.Select(i => new RecipeDetailViewModel.Item
            {
                Name = i.Name,
                Quantity = $"{i.Quantity} {i.Unit}"
            }).ToList()
        };
    }

    public void UpdateRecipe(UpdateRecipeCommand cmd)
    {
        var recipe = _recipes.FirstOrDefault(r => r.RecipeId == cmd.Id && !r.IsDeleted);
        if (recipe == null)
            return;

        recipe.TimeToCook = new TimeSpan(cmd.TimeToCookHrs, cmd.TimeToCookMins, 0);
        recipe.Method = cmd.Method;
        recipe.LastModified = DateTime.UtcNow;
    }

    public void DeleteRecipe(int recipeId)
    {
        var recipe = _recipes.FirstOrDefault(r => r.RecipeId == recipeId);
        if (recipe != null)
        {
            recipe.IsDeleted = true;
            recipe.LastModified = DateTime.UtcNow;
        }
    }
}

public class CreateRecipeCommand
{
    public required string Name { get; set; }
    public int TimeToCookHrs { get; set; }
    public int TimeToCookMins { get; set; }
    public required string Method { get; set; }
    public required ICollection<Ingredient> Ingredients { get; set; }
}

public class RecipeSummaryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TimeToCook { get; set; }
}

public class RecipeDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public ICollection<Item> Ingredients { get; set; }
    public DateTime LastModified { get; set; }

    public class Item
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
    }
}

public class UpdateRecipeCommand
{
    public int Id { get; set; }
    public int TimeToCookHrs { get; set; }
    public int TimeToCookMins { get; set; }
    public required string Method { get; set; }
}