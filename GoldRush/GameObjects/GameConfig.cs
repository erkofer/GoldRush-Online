﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    internal static class GameConfig
    {
        public static int LowestId;

        public class Config
        {
            public string Name;
            public int Id;

            public Config()
            {
                Id = ++LowestId;
            }
        }
        // 0 - 300
        public static class Items
        {
            public class ItemConfig : Config
            {
                public ItemConfig()
                {
                    if (LowestId > 300)
                    {
                        LowestId = 0;

                        Id = 1;
                    }
                }
                public int Worth;
                public GoldRush.Items.Category Category;
            }

            public class ResourceConfig : ItemConfig
            {

                public int Probability;
            }

            public static ResourceConfig Stone = new ResourceConfig { Name = "Stone", Worth = 1, Probability = 3500000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Copper = new ResourceConfig { Name = "Copper", Worth = 5, Probability = 2000000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Iron = new ResourceConfig { Name = "Iron", Worth = 20, Probability = 1500000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Silver = new ResourceConfig { Name = "Silver", Worth = 100, Probability = 1000000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Gold = new ResourceConfig { Name = "Gold", Worth = 1000, Probability = 500000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Uranium = new ResourceConfig { Name = "Uranium", Worth = 5000, Probability = 5000, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Titanium = new ResourceConfig { Name = "Titanium", Worth = 1000000, Probability = 25, Category = GoldRush.Items.Category.ORE };
            public static ResourceConfig Opal = new ResourceConfig { Name = "Opal", Worth = 2000, Probability = 25000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Jade = new ResourceConfig { Name = "Jade", Worth = 5000, Probability = 20000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Topaz = new ResourceConfig { Name = "Topaz", Worth = 10000, Probability = 15000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Sapphire = new ResourceConfig { Name = "Sapphire", Worth = 25000, Probability = 10000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Emerald = new ResourceConfig { Name = "Emerald", Worth = 50000, Probability = 5000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Ruby = new ResourceConfig { Name = "Ruby", Worth = 100000, Probability = 2500, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Onyx = new ResourceConfig { Name = "Onyx", Worth = 250000, Probability = 1000, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Quartz = new ResourceConfig { Name = "Quartz", Worth = 500000, Probability = 50, Category = GoldRush.Items.Category.GEM };
            public static ResourceConfig Diamond = new ResourceConfig { Name = "Diamond", Worth = 5000000, Probability = 20, Category = GoldRush.Items.Category.GEM };
            public static ItemConfig BronzeBar = new ItemConfig { Name = "Bronze bar", Worth = 250, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig IronBar = new ItemConfig { Name = "Iron bar", Worth = 1000, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig SilverBar = new ItemConfig { Name = "Silver bar", Worth = 2500, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig SteelBar = new ItemConfig { Name = "Steel bar", Worth = 5000, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig GoldBar = new ItemConfig { Name = "Gold bar", Worth = 25000, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig TitaniumBar = new ItemConfig { Name = "Titanium bar", Worth = 5000000, Category = GoldRush.Items.Category.CRAFTING };
            public static ResourceConfig BitterRoot = new ResourceConfig { Name = "Bitter root", Worth = 10000, Probability = 2000, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig Cubicula = new ResourceConfig { Name = "Cubicula", Worth = 25000, Probability = 1500, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig IronFlower = new ResourceConfig { Name = "Iron flower", Worth = 500000, Probability = 250, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig TongtwistaFlower = new ResourceConfig { Name = "Tongtwista flower", Worth = 1000000, Probability = 100, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig Thornberries = new ResourceConfig { Name = "Thornberries", Worth = 1000, Probability = 2000, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig Transfruit = new ResourceConfig { Name = "Transfruit", Worth = 5000, Probability = 1000, Category = GoldRush.Items.Category.INGREDIENT };
            public static ResourceConfig MeltingNuts = new ResourceConfig { Name = "Melting nuts", Worth = 10000, Probability = 500, Category = GoldRush.Items.Category.INGREDIENT };
            public static ItemConfig EmptyVial = new ItemConfig { Name = "Empty vial", Worth = 500, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig Gunpowder = new ItemConfig { Name = "Gunpowder", Worth = 1250, Category = GoldRush.Items.Category.CRAFTING };
            public static ResourceConfig Logs = new ResourceConfig { Name = "Logs", Worth = 250, Probability = 0, Category = GoldRush.Items.Category.CRAFTING };
            public static ResourceConfig Oil = new ResourceConfig { Name = "Oil", Worth = 0, Probability = 0, Category = GoldRush.Items.Category.NOTFORSALE };
            public static ItemConfig Coins = new ItemConfig { Name = "Coins", Worth = 0, Category = GoldRush.Items.Category.NOTFORSALE };
            public static ItemConfig ClickingPotion = new ItemConfig { Name = "Clicking Potion", Worth = 25000, Category = GoldRush.Items.Category.POTION };
            public static ItemConfig SmeltingPotion = new ItemConfig { Name = "Smelting Potion", Worth = 50000, Category = GoldRush.Items.Category.POTION };
            public static ItemConfig SpeechPotion = new ItemConfig { Name = "Speech Potion", Worth = 100000, Category = GoldRush.Items.Category.POTION };
            public static ItemConfig AlchemyPotion = new ItemConfig { Name = "Alchemy Potion", Worth = 250000, Category = GoldRush.Items.Category.POTION };
            public static ItemConfig CopperWire = new ItemConfig { Name = "Copper wire", Worth = 250, Category = GoldRush.Items.Category.CRAFTING };
            public static ItemConfig Tnt = new ItemConfig { Name = "TNT", Worth = 100000, Category = GoldRush.Items.Category.CRAFTING };


        }
        // 600-900
        public static class Gatherers
        {
            public class GathererConfig : Config
            {
                public GathererConfig()
                {
                    if (LowestId < 600 || LowestId > 900)
                    {
                        LowestId = 600;
                        Id = 601;
                    }
                }
                public double BaseResourcesPerSecond;
                public int FuelConsumption;
            }

            public static GathererConfig Player = new GathererConfig() { Name = "Player", BaseResourcesPerSecond = 1};
            public static GathererConfig Miner = new GathererConfig() { Name = "Miner", BaseResourcesPerSecond = 0.5 };
            public static GathererConfig Lumberjack = new GathererConfig() { Name = "Lumberjack", BaseResourcesPerSecond = 0.5 };
            public static GathererConfig Drill = new GathererConfig() { Name = "Drill", BaseResourcesPerSecond = 2.5, FuelConsumption = 1};
            public static GathererConfig Crusher = new GathererConfig() { Name = "Crusher", BaseResourcesPerSecond = 5, FuelConsumption = 2};
            public static GathererConfig Excavator = new GathererConfig() { Name = "Excavator", BaseResourcesPerSecond = 10, FuelConsumption = 4};
            public static GathererConfig MegaDrill = new GathererConfig() { Name = "Mega Drill", BaseResourcesPerSecond = 20, FuelConsumption = 8};
            public static GathererConfig Pumpjack = new GathererConfig() { Name = "Pumpjack", BaseResourcesPerSecond = 0.2 };
            public static GathererConfig BigTexan = new GathererConfig() { Name = "Big Texan", BaseResourcesPerSecond = 0.5 };
        }
        // 900-1200
        public static class Processors
        {
            public class ProcessorConfig : Config
            {
                public ProcessorConfig()
                {
                    if (LowestId < 900 || LowestId > 1200)
                    {
                        LowestId = 900;
                        Id = 901;
                    }
                }
                public int BaseCapacity;
            }

            public static ProcessorConfig Furnace = new ProcessorConfig() { Name = "Furnace", BaseCapacity=100 };
            public static ProcessorConfig Cauldron = new ProcessorConfig() { Name = "Cauldron", BaseCapacity=1 };
        }

        // no ids
        public static class StoreItems
        {
            public class StoreItemConfig
            {
                public StoreItemConfig()
                {

                }

                public int BasePrice;
                public Store.Category Category;
                // the maximum of this item you can purchase. 0 means infinite.
                public int MaxQuantity = 1;
                // determines the exponential increase in price. 1 means none.
                public double Factor = 1;
            }

            public static StoreItemConfig EmptyVial = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.ITEMS};
            public static StoreItemConfig Gunpowder = new StoreItemConfig() { BasePrice = 2500, Category = Store.Category.ITEMS};
            public static StoreItemConfig Researcher = new StoreItemConfig() { BasePrice = 1000000, Category = Store.Category.GATHERING };
            public static StoreItemConfig Foreman = new StoreItemConfig() { BasePrice = 250000, Category = Store.Category.GATHERING };
            public static StoreItemConfig Botanist = new StoreItemConfig() { BasePrice = 100000000, Category = Store.Category.GATHERING };

            public static StoreItemConfig Miner = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Lumberjack = new StoreItemConfig() { BasePrice = 20000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Drill = new StoreItemConfig() { BasePrice = 1000000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Crusher = new StoreItemConfig() { BasePrice = 5000000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Excavator = new StoreItemConfig() { BasePrice = 500000000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Pumpjack = new StoreItemConfig() { BasePrice = 100000, Category = Store.Category.MACHINES, MaxQuantity = 100, Factor = 1.15 };
            public static StoreItemConfig BigTexan = new StoreItemConfig() { BasePrice = 2500000, Category = Store.Category.MACHINES, MaxQuantity = 100, Factor = 1.15 };

            public static StoreItemConfig ChainsawsT1 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig ChainsawsT2 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig ChainsawsT3 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };

            public static StoreItemConfig ClickUpgradeT1 = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.MINING };
            public static StoreItemConfig ClickUpgradeT2 = new StoreItemConfig() { BasePrice = 100000, Category = Store.Category.MINING };
            public static StoreItemConfig ClickUpgradeT3 = new StoreItemConfig() { BasePrice = 1000000, Category = Store.Category.MINING };

        }
        // 300-600
        public static class Upgrades
        {

            public class UpgradeConfig : Config
            {

            }

            public class BuffConfig : UpgradeConfig
            {
                public BuffConfig()
                {
                    if (LowestId < 300 || LowestId > 600)
                    {
                        LowestId = 300;
                        Id = 301;
                    }
                }
                public int Duration;
            }

            public static BuffConfig SpeechBuff = new BuffConfig() { Name = "Speech Buff", Duration = 45 };
            public static UpgradeConfig Backpack = new UpgradeConfig() { Name = "Backpack" };
            public static UpgradeConfig Researcher = new UpgradeConfig() { Name = "Researcher" };
            public static UpgradeConfig Botanist = new UpgradeConfig() { Name = "Botanist" };
            public static UpgradeConfig Foreman = new UpgradeConfig() { Name = "Foreman" };
            public static UpgradeConfig ChainsawsT1 = new UpgradeConfig() { Name = "Chainsaws" };
            public static UpgradeConfig ChainsawsT2 = new UpgradeConfig() { Name = "Steel Chainsaws" };

            public static UpgradeConfig ClickUpgradeT1 = new UpgradeConfig() { Name = "Stone Cursor" };
            public static UpgradeConfig ClickUpgradeT2 = new UpgradeConfig() { Name = "Copper Cursor" };
            public static UpgradeConfig ClickUpgradeT3 = new UpgradeConfig() { Name = "Iron Cursor" };
        }
    }
}
