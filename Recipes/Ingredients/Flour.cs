namespace CookBook.Recipes.Ingredients
{
    public abstract class Flour : Ingredient
    {
        public override string PreparationsInstructions => $"Sieve. {base.PreparationsInstructions}";
    }

}
