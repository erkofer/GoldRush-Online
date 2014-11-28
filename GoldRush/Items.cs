using System;
using System.Collections.Generic;
using System.Linq;

namespace GoldRush
{
    public class Items
    {
        public Items(GameObjects objs)
        {
            #region Recipe Creation
            CopperWireRecipe.Ingredients.Add(new Ingredient(Copper,1000));
            CopperWireRecipe.Ingredients.Add(new Ingredient(BronzeBar,10));
            CopperWireRecipe.Resultants.Add(new Ingredient(CopperWire,100));

            TntRecipe.Ingredients.Add(new Ingredient(Gunpowder,100));
            TntRecipe.Ingredients.Add(new Ingredient(CopperWire,25));
            TntRecipe.Resultants.Add(new Ingredient(Tnt,1));
            #endregion

            #region Item Creation
            Stone.Name = "Stone";
            Stone.Worth = 1;
            Stone.Probability = 2000000;
            Stone.Currency = Coins;
            items.Add(Stone);

            Copper.Name = "Copper";
            Copper.Worth = 5;
            Copper.Probability = 1500000;
            Copper.Currency = Coins;
            items.Add(Copper);

            Iron.Name = "Iron";
            Iron.Worth = 20;
            Iron.Probability = 1000000;
            Iron.Currency = Coins;
            items.Add(Iron);

            Silver.Name = "Silver";
            Silver.Worth = 100;
            Silver.Probability = 500000;
            Silver.Currency = Coins;
            items.Add(Silver);

            Gold.Name = "Gold";
            Gold.Worth = 1000;
            Gold.Probability = 125000;
            Gold.Currency = Coins;
            items.Add(Gold);

            Uranium.Name = "Uranium";
            Uranium.Worth = 5000;
            Uranium.Probability = 5000;
            Uranium.Currency = Coins;
            items.Add(Uranium);

            Titanium.Name = "Titanium";
            Titanium.Worth = 1000000;
            Titanium.Probability = 25;
            Titanium.Currency = Coins;
            items.Add(Titanium);

            Opal.Name = "Opal";
            Opal.Worth = 2000;
            Opal.Probability = 5000;
            Opal.Currency = Coins;
            items.Add(Opal);

            Jade.Name = "Jade";
            Jade.Worth = 5000;
            Jade.Probability = 4000;
            Jade.Currency = Coins;
            items.Add(Jade);

            Topaz.Name = "Topaz";
            Topaz.Worth = 10000;
            Topaz.Probability = 3000;
            Topaz.Currency = Coins;
            items.Add(Topaz);

            Sapphire.Name = "Sapphire";
            Sapphire.Worth = 25000;
            Sapphire.Probability = 2000;
            Sapphire.Currency = Coins;
            items.Add(Sapphire);

            Emerald.Name = "Emerald";
            Emerald.Worth = 50000;
            Emerald.Probability = 1000;
            Emerald.Currency = Coins;
            items.Add(Emerald);

            Ruby.Name = "Ruby";
            Ruby.Worth = 100000;
            Ruby.Probability = 750;
            Ruby.Currency = Coins;
            items.Add(Ruby);

            Onyx.Name = "Onyx";
            Onyx.Worth = 250000;
            Onyx.Probability = 200;
            Onyx.Currency = Coins;
            items.Add(Onyx);

            Quartz.Name = "Quartz";
            Quartz.Worth = 500000;
            Quartz.Probability = 20;
            Quartz.Currency = Coins;
            items.Add(Quartz);

            Diamond.Name = "Diamond";
            Diamond.Worth = 5000000;
            Diamond.Probability = 7;
            Diamond.Currency = Coins;
            items.Add(Diamond);
            
            BronzeBar.Name = "Bronze bar";
            BronzeBar.Worth = 250;
            BronzeBar.Currency = Coins;
            items.Add(BronzeBar);

            IronBar.Name = "Iron bar";
            IronBar.Worth = 1000;
            IronBar.Currency = Coins;
            items.Add(IronBar);

            SilverBar.Name = "Silver bar";
            SilverBar.Worth = 2500;
            SilverBar.Currency = Coins;
            items.Add(SilverBar);

            SteelBar.Name = "Steel bar";
            SteelBar.Worth = 5000;
            SteelBar.Currency = Coins;
            items.Add(SteelBar);

            GoldBar.Name = "Gold bar";
            GoldBar.Worth = 25000;
            GoldBar.Currency = Coins;
            items.Add(GoldBar);

            TitaniumBar.Name = "Titanium bar";
            TitaniumBar.Worth = 5000000;
            TitaniumBar.Currency = Coins;
            items.Add(TitaniumBar);

            BitterRoot.Name = "Bitter root";
            BitterRoot.Worth = 10000;
            BitterRoot.Probability = 2000;
            BitterRoot.Currency = Coins;
            items.Add(BitterRoot);

            Cubicula.Name = "Cubicula";
            Cubicula.Worth = 25000;
            Cubicula.Probability = 2000;
            Cubicula.Currency = Coins;
            items.Add(Cubicula);

            IronFlower.Name = "Iron flower";
            IronFlower.Worth = 500000;
            IronFlower.Probability = 250;
            IronFlower.Currency = Coins;
            items.Add(IronFlower);

            TongtwistaFlower.Name = "Tongtwista flower";
            TongtwistaFlower.Worth = 1000000;
            TongtwistaFlower.Probability = 100;
            TongtwistaFlower.Currency = Coins;
            items.Add(TongtwistaFlower);

            Thornberries.Name = "Thornberries";
            Thornberries.Worth = 1000;
            Thornberries.Probability = 2000;
            Thornberries.Currency = Coins;
            items.Add(Thornberries);

            Transfruit.Name = "Transfruit";
            Transfruit.Worth = 5000;
            Transfruit.Probability = 1000;
            Transfruit.Currency = Coins;
            items.Add(Transfruit);

            MeltingNuts.Name = "Melting nuts";
            MeltingNuts.Worth = 10000;
            MeltingNuts.Probability = 250;
            MeltingNuts.Currency = Coins;
            items.Add(MeltingNuts);

            EmptyVial.Name = "Empty vial";
            EmptyVial.Worth = 500;
            EmptyVial.Currency = Coins;

            Gunpowder.Name = "Gunpowder";
            Gunpowder.Worth = 1250;
            Gunpowder.Currency = Coins;

            Logs.Name = "Logs";
            Logs.Worth = 500;
            Logs.Currency = Coins;

            Oil.Name = "Oil";
            Oil.Worth = 400;
            Oil.Currency = Coins;

            Coins.Name = "Coins";
            Coins.Worth = 0;
            Coins.Currency = Coins;

            //TODO: Add potion items.

            CopperWire.Name = "Copper wire";
            CopperWire.Worth = 250;
            CopperWire.Currency = Coins;
            CopperWire.Recipe = CopperWireRecipe;

            Tnt.Name = "TNT";
            Tnt.Worth = 100000;
            Tnt.Currency = Coins;
            Tnt.Recipe = TntRecipe;

            #endregion

            WorthModifier = 1;
        }
        private List<Item> items = new List<Item>();
        public List<Item> All { get { return items; } }

        public double WorthModifier
        {
            set
            {
                foreach (var item in All)
                    item.WorthMultiplier = value;
            }
            get { return All[0].WorthMultiplier; }
        }

        #region Item Declaration

        public Resource Stone = new Resource();
        public Resource Copper = new Resource();
        public Resource Iron = new Resource();
        public Resource Silver = new Resource();
        public Resource Gold = new Resource();
        public Resource Uranium = new Resource();
        public Resource Titanium = new Resource();
        public Resource Opal = new Resource();
        public Resource Jade = new Resource();
        public Resource Topaz = new Resource();
        public Resource Sapphire = new Resource();
        public Resource Emerald = new Resource();
        public Resource Ruby = new Resource();
        public Resource Onyx = new Resource();
        public Resource Quartz = new Resource();
        public Resource Diamond = new Resource();
        public Item BronzeBar = new Item();
        public Item IronBar = new Item();
        public Item SteelBar = new Item();
        public Item SilverBar = new Item();
        public Item GoldBar = new Item();
        public Item TitaniumBar = new Item();
        public Resource BitterRoot = new Resource();
        public Resource Cubicula = new Resource();
        public Resource IronFlower = new Resource();
        public Resource TongtwistaFlower = new Resource();
        public Resource Thornberries = new Resource();
        public Resource Transfruit = new Resource();
        public Resource MeltingNuts = new Resource();
        public Item EmptyVial = new Item();
        public Item Gunpowder = new Item();
        public Resource Logs = new Resource();
        public Resource Oil = new Resource();
        public Item Coins = new Item();
        public Item ClickingPotion = new Item();
        public Item SmeltingPotion = new Item();
        public Item SpeechPotion = new Item();
        public Item AlchemyPotion = new Item();
        public Item CopperWire = new Item();
        public Item Tnt = new Item();

        public Recipe CopperWireRecipe = new Recipe();
        public Recipe TntRecipe = new Recipe();

        #endregion

        //public static Recipe CopperWireRecipe = new Recipe();

        /// <summary>
        /// A collectable GameObject that is generally shown in the players inventory.
        /// </summary>
        public class Item : GameObjects.GameObject
        {
            public Item()
            {
                quantity = 0;
                PrestigeTimeTotal = 0;
                LifeTimeTotal = 0;
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
            public int Worth { get; set; }

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
                get { return (int)Math.Floor(Worth * WorthMultiplier); }
            }

            public Recipe Recipe { get; set; }

            public void Craft()
            {
                Craft(1);
            }

            public void Craft(int iterations)
            {
                if(Recipe.Has(iterations))
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
                Currency.Quantity += Value*iterations;
            }
        }

        /// <summary>
        /// A collectable GameObject that is show in the players inventory and gathered by a machine.
        /// </summary>
        public class Resource : Item
        {
            public int Probability { get; set; }
        }

        public class Potion : Item
        {
            // TODO: Implement buffs.
        }

        /// <summary>
        /// A collection of ingredients and resultants.
        /// </summary>
        public class Recipe
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
                    if (ingredient.Item.Quantity < ingredient.Quantity*iterations) return false;
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
        public class ProcessorRecipe : Recipe
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
        public class Ingredient
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