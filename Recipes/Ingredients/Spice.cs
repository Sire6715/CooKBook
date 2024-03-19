namespace CookBook.Recipes.Ingredients
{
    public abstract class Spice : Ingredient
    {
        public override string PreparationsInstructions => $"Take half a teaspoon. {base.PreparationsInstructions}";
    }

}
