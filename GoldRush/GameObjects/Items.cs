﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GoldRush
{
    class Items
    {
        public Items(GameObjects objs)
        {
            #region Recipe Creation
            CopperWireRecipe.Ingredients.Add(new Ingredient(Copper, 1000));
            CopperWireRecipe.Ingredients.Add(new Ingredient(BronzeBar, 10));
            CopperWireRecipe.Resultants.Add(new Ingredient(CopperWire, 100));

            TntRecipe.Ingredients.Add(new Ingredient(Gunpowder, 100));
            TntRecipe.Ingredients.Add(new Ingredient(CopperWire, 25));
            TntRecipe.Resultants.Add(new Ingredient(Tnt, 1));
            #endregion

            #region Item Creation
            items.AddRange(new Item[]
            {
                Stone,Copper,Iron,Silver,Gold,Uranium,Titanium,Opal,Jade,Topaz,Sapphire,
            Emerald,Ruby,Onyx,Quartz,Diamond,BronzeBar,IronBar,SilverBar,SteelBar,GoldBar,TitaniumBar,
            BitterRoot,Cubicula,IronFlower,TongtwistaFlower,Thornberries,Transfruit,MeltingNuts,
            EmptyVial,Gunpowder,Logs,Oil,Coins,ClickingPotion,SmeltingPotion,SpeechPotion,AlchemyPotion,
            CopperWire,Tnt
            });

            // If items should have a currency other than coins assign them here.
            // Such as EmptyVial.Currency = ISK;

            foreach (var item in items)
                All.Add(item.Id, item);

            foreach (var item in items)
                item.Currency = item.Currency ?? Coins;

            // Assign item recipes here.

            CopperWire.Recipe = CopperWireRecipe;
            Tnt.Recipe = TntRecipe;

            // Potion buffs should be assigned in the upgrades class.

            #endregion

            foreach (var item in items)
                item.Quantity = 100;

        }
        private List<Item> items = new List<Item>();
        //public List<Item> All { get { return items; } }

        public Dictionary<int, Item> All = new Dictionary<int,Item>();

        public double WorthModifier
        {
            set
            {
                foreach (var item in items)
                    item.WorthMultiplier = value;
            }
            get { return items[0].WorthMultiplier; }
        }

        #region Item Declaration

        public Resource Stone = new Resource(GameConfig.Items.Stone);
        public Resource Copper = new Resource(GameConfig.Items.Copper);
        public Resource Iron = new Resource(GameConfig.Items.Iron);
        public Resource Silver = new Resource(GameConfig.Items.Silver);
        public Resource Gold = new Resource(GameConfig.Items.Gold);
        public Resource Uranium = new Resource(GameConfig.Items.Uranium);
        public Resource Titanium = new Resource(GameConfig.Items.Titanium);
        public Resource Opal = new Resource(GameConfig.Items.Opal);
        public Resource Jade = new Resource(GameConfig.Items.Jade);
        public Resource Topaz = new Resource(GameConfig.Items.Topaz);
        public Resource Sapphire = new Resource(GameConfig.Items.Sapphire);
        public Resource Emerald = new Resource(GameConfig.Items.Emerald);
        public Resource Ruby = new Resource(GameConfig.Items.Ruby);
        public Resource Onyx = new Resource(GameConfig.Items.Onyx);
        public Resource Quartz = new Resource(GameConfig.Items.Quartz);
        public Resource Diamond = new Resource(GameConfig.Items.Diamond);
        public Item BronzeBar = new Item(GameConfig.Items.BronzeBar);
        public Item IronBar = new Item(GameConfig.Items.IronBar);
        public Item SteelBar = new Item(GameConfig.Items.SteelBar);
        public Item SilverBar = new Item(GameConfig.Items.SilverBar);
        public Item GoldBar = new Item(GameConfig.Items.GoldBar);
        public Item TitaniumBar = new Item(GameConfig.Items.TitaniumBar);
        public Resource BitterRoot = new Resource(GameConfig.Items.BitterRoot);
        public Resource Cubicula = new Resource(GameConfig.Items.Cubicula);
        public Resource IronFlower = new Resource(GameConfig.Items.IronFlower);
        public Resource TongtwistaFlower = new Resource(GameConfig.Items.TongtwistaFlower);
        public Resource Thornberries = new Resource(GameConfig.Items.Thornberries);
        public Resource Transfruit = new Resource(GameConfig.Items.Transfruit);
        public Resource MeltingNuts = new Resource(GameConfig.Items.MeltingNuts);
        public Item EmptyVial = new Item(GameConfig.Items.EmptyVial);
        public Item Gunpowder = new Item(GameConfig.Items.Gunpowder);
        public Resource Logs = new Resource(GameConfig.Items.Logs);
        public Resource Oil = new Resource(GameConfig.Items.Oil);
        public Item Coins = new Item(GameConfig.Items.Coins);
        public Potion ClickingPotion = new Potion(GameConfig.Items.ClickingPotion);
        public Potion SmeltingPotion = new Potion(GameConfig.Items.SmeltingPotion);
        public Potion SpeechPotion = new Potion(GameConfig.Items.SpeechPotion);
        public Potion AlchemyPotion = new Potion(GameConfig.Items.AlchemyPotion);
        public Item CopperWire = new Item(GameConfig.Items.CopperWire);
        public Item Tnt = new Item(GameConfig.Items.Tnt);

        public Recipe CopperWireRecipe = new Recipe();
        public Recipe TntRecipe = new Recipe();

        #endregion

        //public static Recipe CopperWireRecipe = new Recipe();

        /// <summary>
        /// A collectable GameObject that is generally shown in the players inventory.
        /// </summary>
        internal class Item : GameObjects.GameObject
        {
            public Item()
            {
                
            }

            private readonly GameConfig.Items.ItemConfig _config;

            public Item(GameConfig.Items.ItemConfig config)
                :base(config)
            {
                _config = config;
            }

            private int quantity;

            public override int Quantity
            {
                get { return quantity; }
                set
                {
                    int difference = value - quantity;
                    if (difference > 0)
                    {
                        PrestigeTimeTotal += difference;
                        LifeTimeTotal += difference;
                    }
                    quantity = value;
                }
            }

            public int PrestigeTimeTotal { get; set; }
            public int LifeTimeTotal { get; set; }

            /// <summary>
            /// How much the item is worth at base.
            /// </summary>
            public int Worth { get { return _config.Worth; } }

            /// <summary>
            /// How much the item worth is to be multiplied by. For upgrades.
            /// </summary>
            public double WorthMultiplier { get; set; }

            /// <summary>
            /// The currency this item can be sold for.
            /// </summary>
            public GameObjects.GameObject Currency { get; set; }

            /// <summary>
            /// The items worth multiplied by the current item worth modifier.
            /// </summary>
            public int Value
            {
                get { return (int)Math.Floor(Worth * (WorthMultiplier+1)); }
            }

            public Recipe Recipe { get; set; }

            public void Craft()
            {
                Craft(1);
            }

            public void Craft(int iterations)
            {
                if (Recipe.Has(iterations))
                    Recipe.Craft(iterations);
            }

            public void Sell()
            {
                Sell(1);
            }

            public void Sell(int iterations)
            {
                iterations = Math.Min(Quantity, iterations);
                Quantity -= iterations;
                Currency.Quantity += Value * iterations;
            }
        }

        /// <summary>
        /// A collectable GameObject that is shown in the players inventory and gathered by a machine.
        /// </summary>
        internal class Resource : Item
        {
            private readonly GameConfig.Items.ResourceConfig _config;

            public Resource(GameConfig.Items.ResourceConfig config)
                :base(config)
            {
                _config = config;
            }

            public int Probability { get { return _config.Probability; } }
        }

        /// <summary>
        /// A collectable GameObject that is shown in the players inventory and can be consumed for a buff.
        /// </summary>
        internal class Potion : Item
        {
            public Potion(GameConfig.Items.ItemConfig config)
                :base(config)
            {
                
            }
            /// <summary>
            /// Consumes the potion. Set and forget, will be managed by Upgrades class.
            /// </summary>
            public void Consume()
            {
                if (Quantity <= 0) return;
                Effect.Activate();
                Quantity--;
            }

            public Upgrades.Upgrade Effect;
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
            public virtual bool Has()
            {
                return Has(1);
            }

            /// <summary>
            /// Determines if the player has enough ingredients to craft the recipe a number of times.
            /// </summary>
            /// <param name="iterations">The number of crafting iterations.</param>
            /// <returns>Whether the player has the required ingredients.</returns>
            public virtual bool Has(int iterations)
            {
                foreach (Ingredient ingredient in Ingredients)
                {
                    if (ingredient.Item.Quantity < ingredient.Quantity * iterations) return false;
                }
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
                foreach (var ingredient in Ingredients)
                    ingredient.Consume(iterations);

                foreach (var resultant in Resultants)
                    resultant.Provide(iterations);
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