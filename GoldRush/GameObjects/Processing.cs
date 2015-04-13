using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Processing
    {
        public Processing(GameObjects objs)
        {

            Processors.Add(Furnace.Id, Furnace);
            Processors.Add(Cauldron.Id, Cauldron);

            BronzeBar = new ProcessorRecipe();
            BronzeBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Copper, 1));
            BronzeBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 50));
            BronzeBar.Resultants.Add(new Crafting.Ingredient(objs.Items.BronzeBar, 1));
            BronzeBar.Duration = 5;
            Furnace.Recipes.Add(BronzeBar);

            IronBar = new ProcessorRecipe();
            IronBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Iron, 1));
            IronBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 150));
            IronBar.Resultants.Add(new Crafting.Ingredient(objs.Items.IronBar, 1));
            IronBar.Duration = 15;
            Furnace.Recipes.Add(IronBar);

            SteelBar = new ProcessorRecipe();
            SteelBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.IronBar, 1));
            SteelBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 300));
            SteelBar.Resultants.Add(new Crafting.Ingredient(objs.Items.SteelBar, 1));
            SteelBar.Duration = 30;
            Furnace.Recipes.Add(SteelBar);

            SilverBar = new ProcessorRecipe();
            SilverBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Silver, 1));
            SilverBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 300));
            SilverBar.Resultants.Add(new Crafting.Ingredient(objs.Items.SilverBar, 1));
            SilverBar.Duration = 35;
            Furnace.Recipes.Add(SilverBar);

            GoldBar = new ProcessorRecipe();
            GoldBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Gold, 1));
            GoldBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 700));
            GoldBar.Resultants.Add(new Crafting.Ingredient(objs.Items.GoldBar, 1));
            GoldBar.Duration = 75;
            Furnace.Recipes.Add(GoldBar);

            TitaniumBar = new ProcessorRecipe();
            TitaniumBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Titanium, 1));
            TitaniumBar.Ingredients.Add(new Crafting.Ingredient(objs.Items.Oil, 1400));
            TitaniumBar.Resultants.Add(new Crafting.Ingredient(objs.Items.TitaniumBar, 1));
            TitaniumBar.Duration = 165;
            Furnace.Recipes.Add(TitaniumBar);

            ClickingPotion = new ProcessorRecipe();
            ClickingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.Cubicula, 1));
            ClickingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.Transfruit, 10));
            ClickingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.EmptyVial, 1));
            ClickingPotion.Resultants.Add(new Crafting.Ingredient(objs.Items.ClickingPotion, 1));
            ClickingPotion.Duration = 15;
            Cauldron.Recipes.Add(ClickingPotion);

            SmeltingPotion = new ProcessorRecipe();
            SmeltingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.BitterRoot, 1));
            SmeltingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.Thornberries, 10));
            SmeltingPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.EmptyVial, 1));
            SmeltingPotion.Resultants.Add(new Crafting.Ingredient(objs.Items.SmeltingPotion, 1));
            SmeltingPotion.Duration = 15;
            Cauldron.Recipes.Add(SmeltingPotion);

            SpeechPotion = new ProcessorRecipe();
            SpeechPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.TongtwistaFlower, 1));
            SpeechPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.Quartz, 20));
            SpeechPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.EmptyVial, 1));
            SpeechPotion.Resultants.Add(new Crafting.Ingredient(objs.Items.SpeechPotion, 1));
            SpeechPotion.Duration = 15;
            Cauldron.Recipes.Add(SpeechPotion);

            AlchemyPotion = new ProcessorRecipe();
            AlchemyPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.IronFlower, 5));
            AlchemyPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.GoldBar, 20));
            AlchemyPotion.Ingredients.Add(new Crafting.Ingredient(objs.Items.EmptyVial, 1));
            AlchemyPotion.Resultants.Add(new Crafting.Ingredient(objs.Items.AlchemyPotion, 1));
            AlchemyPotion.Duration = 15;
            Cauldron.Recipes.Add(AlchemyPotion);
        }


        public Dictionary<int, Processor> Processors = new Dictionary<int, Processor>(); 

        public Processor Furnace = new Processor(GameConfig.Processors.Furnace);
        private ProcessorRecipe BronzeBar;
        private ProcessorRecipe IronBar;
        private ProcessorRecipe SteelBar;
        private ProcessorRecipe SilverBar;
        private ProcessorRecipe GoldBar;
        private ProcessorRecipe TitaniumBar;

        public Processor Cauldron = new Processor(GameConfig.Processors.Cauldron);
        private ProcessorRecipe ClickingPotion;
        private ProcessorRecipe SmeltingPotion;
        private ProcessorRecipe SpeechPotion;
        private ProcessorRecipe AlchemyPotion;

        public void Update(int ms)
        {
            foreach (var processor in Processors)
            {
                processor.Value.Update(ms);
            }
        }

        public void Process(int processorId, int recipeIndex, int iterations)
        {
            Processors[processorId].Start(recipeIndex, iterations);
        }

        internal class Processor : GameObjects.GameObject
        {
            public Processor(GameConfig.Processors.ProcessorConfig config)
                : base(config)
            {
                _config = config;
            }

            private readonly GameConfig.Processors.ProcessorConfig _config;

            public int RecipesCraftedPerIteration = 1;

            public int Capacity { get { return (_config.BaseCapacity + ExtraCapacity); } }

            public int ExtraCapacity;

            /// <summary>
            /// A list of recipes the processor can craft.
            /// </summary>
            public List<ProcessorRecipe> Recipes = new List<ProcessorRecipe>();

            /// <summary>
            /// The speed the processor completes recipes at.
            /// </summary>
            public double Speed = 1;

            /// <summary>
            /// The recipe currently being crafted by the processor.
            /// </summary>
            private ProcessorRecipe selectedRecipe;

            public int SelectedRecipeIndex
            {
                get { return selectedRecipe != null ? Recipes.IndexOf(selectedRecipe) : -1; }
                set
                {
                    selectedRecipe = value == -1 ? null : Recipes[value];
                }
            }

            public double SelectedRecipeDuration { get { return selectedRecipe != null ? selectedRecipe.Duration : 0; } }
            /// <summary>
            /// The number of iterations the recipe should be crafted for.
            /// </summary>
            private int recipesToCraft;

            public int RecipesToCraft { get { return recipesToCraft; } set { recipesToCraft = value; } }

            /// <summary>
            /// The number of iterations the recipe has been crafted for.
            /// </summary>
            private int recipesCrafted;

            public int RecipesCrafted { get { return recipesCrafted; } set { recipesCrafted = value; } }

            private int recipesLeftToCraft { get { return recipesToCraft - recipesCrafted; } }

            /// <summary>
            /// The progress on the current crafting iteration.
            /// </summary>
            private double recipeProgress;

            public double RecipeProgress { get { return recipeProgress; } set { recipeProgress = value; } }

            public void Stop()
            {
                var remainingRecipes = (double)recipesLeftToCraft;
                remainingRecipes *= 0.5;
                remainingRecipes = Math.Floor(remainingRecipes);

                if (selectedRecipe != null)
                {
                    // Refund half of the ingredients.
                    foreach (var ingredient in selectedRecipe.Ingredients)
                        ingredient.Provide((int)remainingRecipes);
                }

                // Reset the processor.
                recipesCrafted = 0;
                recipesToCraft = 0;
                recipeProgress = 0;
            }

            public void Start(int recipeIndex, int iterations)
            {
                if (Recipes[recipeIndex] == null || recipesLeftToCraft != 0) return;

                selectedRecipe = Recipes[recipeIndex];
                iterations = Math.Min(iterations, Capacity);

                if (!selectedRecipe.Has(iterations)) return;

                foreach (var ingredient in selectedRecipe.Ingredients)
                    ingredient.Consume(iterations);

                recipesToCraft = iterations;
                recipesCrafted = 0;
                recipeProgress = 0;
            }

            public void Update(int ms)
            {
                if (recipesLeftToCraft == 0)
                {
                    Stop();
                    return;
                }

                var ticks = ms / 1000;
                recipeProgress += ticks * Speed;

                if (recipeProgress < selectedRecipe.Duration) return;

                var iterations = Math.Floor(recipeProgress / selectedRecipe.Duration);
                Craft((int)iterations);

                if (recipesLeftToCraft == 0) Stop();
            }

            private void Craft(int iterations)
            {
                recipeProgress %= selectedRecipe.Duration;

                if (iterations > recipesLeftToCraft)
                    iterations = recipesLeftToCraft;

                foreach (var resultant in selectedRecipe.Resultants)
                    resultant.Provide(iterations * RecipesCraftedPerIteration);

                recipesCrafted += iterations;
            }
        }

        /// <summary>
        /// A collection of ingredients and resulants with a duration.
        /// </summary>
        internal class ProcessorRecipe : Crafting.Recipe
        {
            public ProcessorRecipe()
            {
                Duration = 0;
            }

            public ProcessorRecipe(List<Crafting.Ingredient> ingredients, List<Crafting.Ingredient> resultants, int duration)
                : base(ingredients, resultants)
            {
                Duration = duration;
            }
            public int Duration { get; set; }
        }
    }
}
