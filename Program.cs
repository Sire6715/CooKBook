
using CookBook.Recipes;
using CookBook.Recipes.Ingredients;
using System.Diagnostics.Metrics;
using System.Formats.Tar;
using static RecipesUserInteraction;

IngredientsRegister ingredientsRegister = new IngredientsRegister();

var CookieRecipesApp = new CookiesRecipesApp(
    new RecipesRepository(
        new StringTextualRepository(),
         ingredientsRegister),
    new RecipesUserInteraction(ingredientsRegister));

CookieRecipesApp.Run(@"C:\Users\hp\source\repos\CookBook\Recipes.txt");

public class CookiesRecipesApp
{
    private readonly IRecipesRepository _recipesRepository;
    private readonly IRecipesUserInteraction _recipesUserInteraction;

    public CookiesRecipesApp(
        RecipesRepository recipesRepository,
        RecipesUserInteraction recipesUserInteraction)
    {
        _recipesRepository = recipesRepository;
        _recipesUserInteraction = recipesUserInteraction;
    }
    public void Run(string filePath)
    {
        var allRecipes = _recipesRepository.Read(filePath);
        _recipesUserInteraction.PrintExistingrecipes(allRecipes);

        _recipesUserInteraction.PromptUserToCreateRecipe();
        var ingredients = _recipesUserInteraction.ReadIngredientsFromUser();

        if (ingredients.Count() > 0)
        {
            var recipe = new Recipe(ingredients);
            allRecipes.Add(recipe);
            _recipesRepository.Write(filePath, allRecipes);

            _recipesUserInteraction.ShowMessage("Recipe added:");
            _recipesUserInteraction.ShowMessage(recipe.ToString());
        }
        else
        {
            _recipesUserInteraction.ShowMessage("No ingreients have been suggested. " +
                "recipe will not be served. ");
        }

        _recipesUserInteraction.Exit();
    }
}

internal interface IRecipesRepository
{
    List<Recipe> Read(string filePath);

    void Write(string filePath, List<Recipe> allRecipes);
}

public class RecipesRepository : IRecipesRepository
{

    private readonly IStringRepository _stringsRepository;
    private readonly IIngredientsRegister _ingredientsRegister;
    private const string Seperator = ",";

    public RecipesRepository(IStringRepository stringsRepository, IIngredientsRegister ingredientsRegister)
    {
        _stringsRepository = stringsRepository;
        _ingredientsRegister = ingredientsRegister;
    }

    public List<Recipe> Read(string filePath)
    {
        List<String> recipesFromFile = _stringsRepository.Read(filePath);
        var recipes = new List<Recipe>();

        foreach (var recipeFromFile in recipesFromFile)
        {
            var recipe = RecipeFromString(recipeFromFile);
            recipes.Add(recipe);
        }
        return recipes;
    }

    private Recipe RecipeFromString(string recipeFromFile)
    {
        var textualIds = recipeFromFile.Split(Seperator);
        var ingredients = new List<Ingredient>();

        foreach (var textualId in textualIds)
        {
            var id = int.Parse(textualId);
            var ingredient = _ingredientsRegister.GetById(id);
            ingredients.Add(ingredient);
        }

        return new Recipe(ingredients);
    }

    public void Write(string filePath, List<Recipe> allRecipes)
    {
        var recipeAsStrings = new List<string>();
        foreach (var recipe in allRecipes)
        {
            var allId = new List<int>();
            foreach (var ingredient in recipe.Ingredients)
            {
                allId.Add(ingredient.Id);
            }
            recipeAsStrings.Add(string.Join(Seperator, allId));
        }
        _stringsRepository.Write(filePath, recipeAsStrings);
    }

}

public interface IRecipesUserInteraction
{
    void ShowMessage(string message);
    void Exit();
    void PrintExistingrecipes(IEnumerable<Recipe> allRecipes);
    void PromptUserToCreateRecipe();
    IEnumerable<Ingredient> ReadIngredientsFromUser();

}

public interface IIngredientsRegister
{
    IEnumerable<Ingredient> All { get; }

    Ingredient GetById(int id);
}

public class IngredientsRegister : IIngredientsRegister
{
    public IEnumerable<Ingredient> All { get; } = new List<Ingredient>
    {
        new WheatFlour(),
        new CoconutFlour(),
        new Butter(),
        new Chocolate(),
        new Sugar(),
        new Cardamom(),
        new Cinnamon(),
        new Cocoa()
    };

    public Ingredient GetById(int id)
    {
        foreach (var ingredients in All)
        {
            if (ingredients.Id == id)
            {
                return ingredients;
            }
        }

        return null;
    }
}

public class RecipesUserInteraction : IRecipesUserInteraction
{
    private readonly IIngredientsRegister _ingredientRegister;

    public RecipesUserInteraction(IIngredientsRegister ingredientsRegister)
    {
        _ingredientRegister = ingredientsRegister;
    }
    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
    public void Exit()
    {
        Console.WriteLine("press any key to close.");
        Console.ReadKey();
    }

    public void PrintExistingrecipes(IEnumerable<Recipe> allRecipes)
    {
        if (allRecipes.Count() > 0)
        {
            Console.WriteLine("Existing Recipes are: " + Environment.NewLine);

            var counter = 1;
            foreach (var recipe in allRecipes)
            {
                Console.WriteLine($"****{counter}*****");
                Console.WriteLine(recipe);
                Console.WriteLine();
                ++counter;
            }
        }
    }

    public void PromptUserToCreateRecipe()
    {
        Console.WriteLine("Create a new cookie recipe! " + "Available ingredients are:");

        foreach (var ingredients in _ingredientRegister.All)
        {
            Console.WriteLine(ingredients);
        }
    }

    public IEnumerable<Ingredient> ReadIngredientsFromUser()
    {
        bool shallStop = false;
        var ingredients = new List<Ingredient>();

        while (!shallStop)
        {
            Console.WriteLine("Add an ingredient by its ID, " + "or type anything else if finished.");

            var userInput = Console.ReadLine();
            if (int.TryParse(userInput, out int id))
            {
                var selectedIngredient = _ingredientRegister.GetById(id);
                if (selectedIngredient is not null)
                {
                    ingredients.Add(selectedIngredient);
                }
            }
            else
            {
                shallStop = true;
            }
        }

        return ingredients;
    }

    public interface IStringRepository
    {
        List<string> Read(string filePath);
        void Write(string filePath, List<string> strings);
    }

    public class StringTextualRepository : IStringRepository
    {
        private static readonly string Seperator = Environment.NewLine;

        public List<string> Read(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);
                return fileContents.Split(Seperator).ToList();
            }
            return new List<string>();
        }

        public void Write(string filePath, List<string> strings)
        {
            File.WriteAllText(filePath, string.Join(Seperator, strings));
        }
    }
}