namespace CookBook.Recipes.Ingredients
{
    internal class Butter : Ingredient
    {
        public override int Id => 3;
        public override string Name => "Butter";
        public override string PreparationsInstructions => $"Melt on low heat. {base.PreparationsInstructions}";
    }

}
