namespace CookBook.Recipes.Ingredients
{
    internal class Chocolate : Ingredient
    {
        public override int Id => 4;
        public override string Name => "Chocolate";
        public override string PreparationsInstructions => $"Melt in a water bath. {base.PreparationsInstructions}";
    }

}
