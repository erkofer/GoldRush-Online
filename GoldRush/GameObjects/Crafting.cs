using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Crafting
    {
        public Crafting(GameObjects objs){
            CopperWire = new Recipe();
            CopperWire.Ingredients.Add(new Ingredient(objs.Items.Copper, 1000));
            CopperWire.Ingredients.Add(new Ingredient(objs.Items.BronzeBar, 10));
            CopperWire.Resultants.Add(new Ingredient(objs.Items.CopperWire, 100));
            All.Add(CopperWire.Resultants[0].Item.Id, CopperWire);



        }

        public Dictionary<int, Recipe> All = new Dictionary<int, Recipe>();

        Recipe CopperWire;

        public void Craft(int id, int quantity)
        {
            All[id].Craft(quantity);
        }

        /// <summary>
        /// A collection of ingredients and resultants.
        /// </summary>
        internal class Recipe
        {
            public virtual List<Ingredient> Ingredients { get; set; }
            public virtual List<Ingredient> Resultants { get; set; }

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
            private bool Has(int iterations)
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
        class ProcessorRecipe : Recipe
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
