﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GoldRush
{
    class Items
    {
        public Items(GameObjects objs)
        {

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


            // Potion buffs should be assigned in the upgrades class.

            #endregion

            /*foreach (var item in items)
                item.Quantity = 100;*/
            Coins.Quantity = 5000000000;

        }
        private List<Item> items = new List<Item>();
        //public List<Item> All { get { return items; } }

        public Dictionary<int, Item> All = new Dictionary<int,Item>();

        public void SellAll()
        {
            foreach (var item in items)
                if (item.IncludeInSellAll)
                    item.Sell(item.Quantity);
            
        }

        public void Drink(int id)
        {
            Potion potion = (Potion)All[id];
            potion.Consume();
        }

        public double WorthModifier
        {
            set
            {
                foreach (var item in items)
                    item.WorthMultiplier = value;
            }
            get { return items[0].WorthMultiplier; }
        }

        public enum Category
        {
            NOTFORSALE=0,
            ORE = 1,
            GEM = 2,
            INGREDIENT = 3,
            CRAFTING = 4,
            POTION = 5
        };

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
        /*
        public Recipe CopperWireRecipe = new Recipe();
        public Recipe TntRecipe = new Recipe();*/

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
                IncludeInSellAll = false;
            }

            private long quantity { get { return base.Quantity; } set { base.Quantity = value; } }

            public override long Quantity
            {
                get { return quantity; }
                set
                {
                    long difference = value - quantity;
                    if (difference > 0)
                    {
                        PrestigeTimeTotal += difference;
                        LifeTimeTotal += difference;
                    }

                    quantity = value;
                }
            }

            public override bool Active
            {
                get { return Quantity > 0; }
                set
                {
                    if (value)
                        if (Quantity <= 0) Quantity = 1;
                    else
                        if (Quantity > 0) Quantity = 0;
                }
            }

            public long PrestigeTimeTotal { get; set; }
            public long LifeTimeTotal { get; set; }

            public bool IncludeInSellAll { get; set; }

            public Category Category { get { return _config.Category; } }


            /// <summary>
            /// How much the item is worth at base.
            /// </summary>
            public long Worth { get { return _config.Worth; } }


            private double worthMultiplier = 1;
            /// <summary>
            /// How much the item worth is to be multiplied by. For upgrades.
            /// </summary>
            public double WorthMultiplier { get { return worthMultiplier; } set { worthMultiplier = value; } }

            /// <summary>
            /// The currency this item can be sold for.
            /// </summary>
            public GameObjects.GameObject Currency { get; set; }

            /// <summary>
            /// The items worth multiplied by the current item worth modifier.
            /// </summary>
            public long Value
            {
                get { return (long)Math.Floor(Worth * (WorthMultiplier)); }
            }

            public GoldRush.Crafting.Recipe Recipe { get; set; }

            public void Craft()
            {
                Craft(1);
            }

            public void Craft(int iterations)
            {
               Recipe.Craft(iterations);
            }

            public void Sell()
            {
                Sell(1);
            }

            public void Sell(long iterations)
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
                if(Buff == null) throw new NotImplementedException(Name + " has no effect assigned to it.");
                Buff.TimeActive = 1;
                Quantity--;
            }

            public Upgrades.Buff Buff;
        }

       
    }
}