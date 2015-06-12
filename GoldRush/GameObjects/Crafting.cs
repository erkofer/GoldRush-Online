
﻿using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Runtime.CompilerServices;
﻿using System.Runtime.Remoting.Messaging;
﻿using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Crafting : INotifier
    {
        public Crafting(GameObjects objs)
        {

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

            ChainsawsT1 = new Recipe();
            ChainsawsT1.Ingredients.Add(new Ingredient(objs.Items.IronBar, 25));
            ChainsawsT1.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT1, 1));
            All.Add(ChainsawsT1.Resultants[0].Item.Id, ChainsawsT1);

            ChainsawsT2 = new Recipe();
            ChainsawsT2.Ingredients.Add(new Ingredient(objs.Items.SteelBar, 20));
            ChainsawsT2.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT2, 1));
            All.Add(ChainsawsT2.Resultants[0].Item.Id, ChainsawsT2);

            ChainsawsT3 = new Recipe();
            ChainsawsT3.Ingredients.Add(new Ingredient(objs.Items.TitaniumBar, 15));
            ChainsawsT3.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT3, 1));
            All.Add(ChainsawsT3.Resultants[0].Item.Id, ChainsawsT3);

            ChainsawsT4 = new Recipe();
            ChainsawsT4.Ingredients.Add(new Ingredient(objs.Items.Diamond, 10));
            ChainsawsT4.Resultants.Add(new Ingredient(objs.Upgrades.ChainsawsT4, 1));
            All.Add(ChainsawsT4.Resultants[0].Item.Id, ChainsawsT4);

            ReinforcedFurnace = new Recipe();
            ReinforcedFurnace.Ingredients.Add(new Ingredient(objs.Items.Stone, 50000));
            ReinforcedFurnace.Ingredients.Add(new Ingredient(objs.Items.IronBar, 100));
            ReinforcedFurnace.Resultants.Add(new Ingredient(objs.Upgrades.ReinforcedFurnace, 1));
            All.Add(ReinforcedFurnace.Resultants[0].Item.Id, ReinforcedFurnace);

            LargerCauldron = new Recipe();
            LargerCauldron.Ingredients.Add(new Ingredient(objs.Items.IronBar, 50));
            LargerCauldron.Resultants.Add(new Ingredient(objs.Upgrades.LargerCauldron, 1));
            All.Add(LargerCauldron.Resultants[0].Item.Id, LargerCauldron);

            Backpack = new Recipe();
            Backpack.Ingredients.Add(new Ingredient(objs.Items.CopperWire, 100));
            Backpack.Resultants.Add(new Ingredient(objs.Upgrades.Backpack, 1));
            All.Add(Backpack.Resultants[0].Item.Id, Backpack);

            DeeperTunnels = new Recipe();
            DeeperTunnels.Ingredients.Add(new Ingredient(objs.Items.Tnt, 500));
            DeeperTunnels.Resultants.Add(new Ingredient(objs.Upgrades.DeeperTunnels, 1));
            All.Add(DeeperTunnels.Resultants[0].Item.Id, DeeperTunnels);
        }

        public Dictionary<int, Recipe> All = new Dictionary<int, Recipe>();

        private Recipe CopperWire;
        private Recipe Tnt;
        private Recipe ChainsawsT1;
        private Recipe ChainsawsT2;
        private Recipe ChainsawsT3;
        private Recipe ChainsawsT4;
        private Recipe ReinforcedFurnace;
        private Recipe LargerCauldron;
        private Recipe Backpack;
        private Recipe DeeperTunnels;


        public void Craft(int id, int iterations)
        {
            var recipe = All[id];
            
            if (!recipe.Craft(iterations))
            {
                var name = recipe.Resultants[0].Item.Name;
                var quantity = recipe.Resultants[0].Quantity;
                Notify(new GameNotification("Insufficient resources to craft " + iterations * quantity + " " + name+".", "chat"));
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
            /// Determines if the player has enough ingredients to craft the recipe a number of times.
            /// </summary>
            /// <param name="iterations">The number of crafting iterations.</param>
            /// <returns>Whether the player has the required ingredients.</returns>
            public bool Has(int iterations = 1)
            {
                foreach (Ingredient ingredient in Ingredients)
                    if (ingredient.Item.Quantity < ingredient.Quantity * iterations) return false;

                return true;
            }


            /// <summary>
            /// Consumes all the ingredients and provides all the resultants.
            /// </summary>
            /// <param name="iterations">The number of iterations to craft.</param>
            public virtual bool Craft(int iterations = 1)
            {
                if (!Has(iterations)) return false;

                foreach (var ingredient in Ingredients)
                    ingredient.Consume(iterations);

                foreach (var resultant in Resultants)
                    resultant.Provide(iterations);

                return true;
            }
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
            /// <param name="iterations">The amount of this ingredient we want.</param>
            /// <returns>Whether we have enough of items.</returns>
            public bool Has(int iterations = 1)
            {
                return Item.Quantity >= Quantity * iterations;
            }

            /// <summary>
            /// Deducts this ingredients worth of items from the player a number of times.
            /// </summary>
            /// <param name="iterations">The number of iterations of this ingredient to deduct.</param>
            public void Consume(int iterations = 1)
            {
                Item.Quantity -= Quantity * iterations;
            }

            /// <summary>
            /// Gives this ingredients worth of items a number of times.
            /// </summary>
            /// <param name="iterations">The number of iterations of this ingredient to give.</param>
            public void Provide(int iterations = 1)
            {
                Item.Quantity += Quantity * iterations;
            }
        }

        public event GameNotificationEventHandler GameNotification;

        private void Notify(GameNotification notification)
        {
            if (GameNotification != null)
            {
                GameNotification(this,new GameNotificationEventArgs(){Notification = notification});
            }
        }
    }
}
