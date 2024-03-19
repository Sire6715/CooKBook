namespace CookBook.Recipes.Ingredients
{
    internal class Cocoa : Ingredient
    {
        public override int Id => 8;
        public override string Name => "Cocoa powder";
        public override string PreparationsInstructions => $"{base.PreparationsInstructions}";
    }

}
