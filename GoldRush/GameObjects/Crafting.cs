
﻿using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Runtime.CompilerServices;
﻿using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Crafting
    {
        public Crafting(GameObjects objs){
            // Item Recipes
            CopperWire = new Recipe();
            CopperWire.Ingredients.Add(new Ingredient(objs.Items.Copper, 1000));
            CopperWire.Ingredients.Add(new Ingredient(objs.Items.BronzeBar, 10));
            CopperWire.Resultants.Add(new Ingredient(objs.Items.CopperWire, 100));
            All.Add(CopperWire.Resultants[0].Item.Id, CopperWire);

            Tnt = new Recipe();
            Tnt.Ingredients.Add(new Ingredient(objs.Items.Gunpowder, 100));
            Tnt.Ingredients.Add(new Ingredient(objs.Items.CopperWire, 25));
            Tnt.Resultants.Add(new Ingredient(objs.Items.Tnt, 1));
            All.Add(Tnt.Resultants[0].Item.Id, Tnt);

            TestRecipe = new Recipe();
            TestRecipe.Ingredients.Add(new Ingredient(objs.Items.Coins, 1000));
            TestRecipe.Resultants.Add(new Ingredient(objs.Items.IronBar, 1));
            All.Add(TestRecipe.Resultants[0].Item.Id, TestRecipe);

            ChainsawsT1 = new Recipe();
            ChainsawsT1.Ingredients.Add(new Ingredient(objs.Items.IronBar, 25));
            ChainsawsT1.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT1, 1));
            All.Add(ChainsawsT1.Resultants[0].Item.Id, ChainsawsT1);

            ChainsawsT2 = new Recipe();
            ChainsawsT2.Ingredients.Add(new Ingredient(objs.Items.SteelBar, 50));
            ChainsawsT2.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT2, 1));
            All.Add(ChainsawsT2.Resultants[0].Item.Id, ChainsawsT2);

            // Processors

            Furnace = new Processor(GameConfig.Processors.Furnace);
            Processors.Add(Furnace.Id,Furnace);

            BronzeBar = new ProcessorRecipe();
            BronzeBar.Ingredients.Add(new Ingredient(objs.Items.Copper,1));
            BronzeBar.Ingredients.Add(new Ingredient(objs.Items.Oil,50));
            BronzeBar.Resultants.Add(new Ingredient(objs.Items.BronzeBar,1));
            BronzeBar.Duration = 5;
            Furnace.Recipes.Add(BronzeBar);

            IronBar = new ProcessorRecipe();
            IronBar.Ingredients.Add(new Ingredient(objs.Items.Iron, 1));
            IronBar.Ingredients.Add(new Ingredient(objs.Items.Oil, 150));
            IronBar.Resultants.Add(new Ingredient(objs.Items.IronBar, 1));
            IronBar.Duration = 15;
            Furnace.Recipes.Add(IronBar);

            SteelBar = new ProcessorRecipe();
            SteelBar.Ingredients.Add(new Ingredient(objs.Items.IronBar, 1));
            SteelBar.Ingredients.Add(new Ingredient(objs.Items.Oil, 300));
            SteelBar.Resultants.Add(new Ingredient(objs.Items.SteelBar, 1));
            SteelBar.Duration = 30;
            Furnace.Recipes.Add(SteelBar);

            SilverBar = new ProcessorRecipe();
            SilverBar.Ingredients.Add(new Ingredient(objs.Items.Silver, 1));
            SilverBar.Ingredients.Add(new Ingredient(objs.Items.Oil, 300));
            SilverBar.Resultants.Add(new Ingredient(objs.Items.SilverBar, 1));
            SilverBar.Duration = 35;
            Furnace.Recipes.Add(SilverBar);

            GoldBar = new ProcessorRecipe();
            GoldBar.Ingredients.Add(new Ingredient(objs.Items.Gold, 1));
            GoldBar.Ingredients.Add(new Ingredient(objs.Items.Oil, 700));
            GoldBar.Resultants.Add(new Ingredient(objs.Items.GoldBar, 1));
            GoldBar.Duration = 75;
            Furnace.Recipes.Add(GoldBar);

            TitaniumBar = new ProcessorRecipe();
            TitaniumBar.Ingredients.Add(new Ingredient(objs.Items.Titanium, 1));
            TitaniumBar.Ingredients.Add(new Ingredient(objs.Items.Oil, 1400));
            TitaniumBar.Resultants.Add(new Ingredient(objs.Items.TitaniumBar, 1));
            TitaniumBar.Duration = 165;
            Furnace.Recipes.Add(TitaniumBar);

            Cauldron = new Processor(GameConfig.Processors.Cauldron);

        }

        public Dictionary<int, Recipe> All = new Dictionary<int, Recipe>();
        public Dictionary<int,Processor> Processors = new Dictionary<int, Processor>(); 

        private Recipe CopperWire;
        private Recipe Tnt;
        private Recipe ChainsawsT1;
        private Recipe ChainsawsT2;
        private Recipe TestRecipe;

        private Processor Furnace;
        private ProcessorRecipe BronzeBar;
        private ProcessorRecipe IronBar;
        private ProcessorRecipe SteelBar;
        private ProcessorRecipe SilverBar;
        private ProcessorRecipe GoldBar;
        private ProcessorRecipe TitaniumBar;

        private Processor Cauldron;
        private ProcessorRecipe ClickingPotion;
        private ProcessorRecipe SmeltingPotion;
        private ProcessorRecipe SpeechPotion;
        private ProcessorRecipe AlchemyPotion;

        public void Craft(int id, int quantity)
        {
            All[id].Craft(quantity);
        }

        public void Update(int ms)
        {
            foreach (var processor in Processors)
            {
                processor.Value.Update(ms);
            }
        }

        public void Process(int processorId, int recipeIndex, int iterations)
        {
            Processors[processorId].Start(recipeIndex,iterations);
        }

        internal class Processor : GameObjects.GameObject
        {
            public Processor(GameConfig.Processors.ProcessorConfig config):base(config)
            {
                _config = config;
            }

            private readonly GameConfig.Processors.ProcessorConfig _config;

            public int Capacity { get { return (_config.BaseCapacity + ExtraCapacity); } }

            public int ExtraCapacity;

            /// <summary>
            /// A list of recipes the processor can craft.
            /// </summary>
            public List<ProcessorRecipe> Recipes= new List<ProcessorRecipe>();

            /// <summary>
            /// The speed the processor completes recipes at.
            /// </summary>
            public double Speed = 1;

            /// <summary>
            /// The recipe currently being crafted by the processor.
            /// </summary>
            private ProcessorRecipe selectedRecipe;

            public int SelectedRecipeIndex { get { return selectedRecipe != null ? Recipes.IndexOf(selectedRecipe) : -1; } }

            public double SelectedRecipeDuration { get { return selectedRecipe != null ? selectedRecipe.Duration : -1; } }
            /// <summary>
            /// The number of iterations the recipe should be crafted for.
            /// </summary>
            private int recipesToCraft;

            public int RecipesToCraft { get { return recipesToCraft; } }

            /// <summary>
            /// The number of iterations the recipe has been crafted for.
            /// </summary>
            private int recipesCrafted;

            public int RecipesCrafted { get { return recipesCrafted; } }

            private int recipesLeftToCraft { get { return recipesToCraft - recipesCrafted; } }

            /// <summary>
            /// The progress on the current crafting iteration.
            /// </summary>
            private double recipeProgress;

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
                if (recipesLeftToCraft == 0) {
                    Stop();
                    return; 
                }

                var ticks = ms / 1000;
                recipeProgress += ticks * Speed;

                if (recipeProgress < selectedRecipe.Duration) return;

                var iterations = Math.Floor(recipeProgress / selectedRecipe.Duration);
                Craft((int)iterations);
            }

            private void Craft(int iterations)
            {
                recipeProgress %= selectedRecipe.Duration;

                if (iterations > recipesLeftToCraft)
                    iterations = recipesLeftToCraft;

                foreach(var resultant in selectedRecipe.Resultants)
                    resultant.Provide(iterations);

                recipesCrafted += iterations;
            }
        }

        /// <summary>
        /// A collection of ingredients and resultants.
        /// </summary>
        internal class Recipe
        {
            public virtual List<Ingredient> Ingredients { get; set; }
            public virtual List<Ingredient> Resultants { get; set; }

            public virtual int Id { get { return Resultants[0].Item.Id; } }

            public Recipe()
            {
                Ingredients = new List<Ingredient>();
                Resultants = new List<Ingredient>();
            }

            public Recipe(List<Ingredient> ingredients, List<Ingredient> resultants)
            {
                Ingredients = ingredients;
                Resultants = resultants;
            }

            /// <summary>
            /// Determines if the player has enough ingredients to craft the recipe.
            /// </summary>
            /// <returns>Whether the player has the required ingredients.</returns>
            private bool Has()
            {
                return Has(1);
            }

            /// <summary>
            /// Determines if the player has enough ingredients to craft the recipe a number of times.
            /// </summary>
            /// <param name="iterations">The number of crafting iterations.</param>
            /// <returns>Whether the player has the required ingredients.</returns>
            public bool Has(int iterations)
            {
                foreach (Ingredient ingredient in Ingredients)
                    if (ingredient.Item.Quantity < ingredient.Quantity * iterations) return false;
                
                return true;
            }

            /// <summary>
            /// Consumes all the ingredients and provides all the resultants.
            /// </summary>
            public virtual void Craft()
            {
                Craft(1);
            }

            /// <summary>
            /// Consumes all the ingredients and provides all the resultants.
            /// </summary>
            /// <param name="iterations">The number of iterations to craft.</param>
            public virtual void Craft(int iterations)
            {
                if (Has(iterations))
                {
                    foreach (var ingredient in Ingredients)
                        ingredient.Consume(iterations);

                    foreach (var resultant in Resultants)
                        resultant.Provide(iterations);
                }
            }
        }

        /// <summary>
        /// A collection of ingredients and resulants with a duration.
        /// </summary>
        internal class ProcessorRecipe : Recipe
        {
            public ProcessorRecipe()
            {
                Duration = 0;
            }

            public ProcessorRecipe(List<Ingredient> ingredients, List<Ingredient> resultants, int duration)
                : base(ingredients, resultants)
            {
                Duration = duration;
            }
            public int Duration { get; set; }
        }

        /// <summary>
        /// A GameObject and quantity to be used in recipes.
        /// </summary>
        internal class Ingredient
        {
            public GameObjects.GameObject Item { get; set; }
            public int Quantity { get; set; }

            public Ingredient(GameObjects.GameObject item, int quantity)
            {
                Item = item;
                Quantity = quantity;
            }

            /// <summary>
            /// Determines whether we have enough items to consume this ingredient.
            /// </summary>
            /// <returns>Whether we have enough of items.</returns>
            public bool Has()
            {
                return Has(1);
            }

            /// <summary>
            /// Determines whether we have enough items to consume this ingredient.
            /// </summary>
            /// <param name="iterations">The amount of this ingredient we want.</param>
            /// <returns>Whether we have enough of items.</returns>
            public bool Has(int iterations)
            {
                return Item.Quantity >= Quantity * iterations;
            }

            /// <summary>
            /// Deducts this ingredients worth of items from the player.
            /// </summary>
            public void Consume()
            {
                Consume(1);
            }

            /// <summary>
            /// Deducts this ingredients worth of items from the player a number of times.
            /// </summary>
            /// <param name="iterations">The number of iterations of this ingredient to deduct.</param>
            public void Consume(int iterations)
            {
                Item.Quantity -= Quantity * iterations;
            }

            /// <summary>
            /// Gives this ingredients worth of items.
            /// </summary>
            public void Provide()
            {
                Provide(1);
            }


            /// <summary>
            /// Gives this ingredients worth of items a number of times.
            /// </summary>
            /// <param name="iterations">The number of iterations of this ingredient to give.</param>
            public void Provide(int iterations)
            {
                Item.Quantity += Quantity * iterations;
            }
        }
    }
}
