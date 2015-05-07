using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

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
                        LowestId = 1;
                        Id = 1;
                    }
                }
                public long Worth;
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
                        LowestId = 601;
                        Id = 601;
                    }
                }
                public double BaseResourcesPerSecond;
                public double FuelConsumption;
            }

            public static GathererConfig Player = new GathererConfig() { Name = "Player", BaseResourcesPerSecond = 1 };
            public static GathererConfig Miner = new GathererConfig() { Name = "Miner", BaseResourcesPerSecond = 0.5 };
            public static GathererConfig Lumberjack = new GathererConfig() { Name = "Lumberjack", BaseResourcesPerSecond = 0.5 };
            public static GathererConfig Drill = new GathererConfig() { Name = "Drill", BaseResourcesPerSecond = 2.5, FuelConsumption = 1 };
            public static GathererConfig Crusher = new GathererConfig() { Name = "Crusher", BaseResourcesPerSecond = 5, FuelConsumption = 2 };
            public static GathererConfig Excavator = new GathererConfig() { Name = "Excavator", BaseResourcesPerSecond = 10, FuelConsumption = 4 };
            public static GathererConfig MegaDrill = new GathererConfig() { Name = "Mega Drill", BaseResourcesPerSecond = 20, FuelConsumption = 8 };
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
                        LowestId = 901;
                        Id = 901;
                    }
                }
                public int BaseCapacity;
            }

            public static ProcessorConfig Furnace = new ProcessorConfig() { Name = "Furnace", BaseCapacity = 100 };
            public static ProcessorConfig Cauldron = new ProcessorConfig() { Name = "Cauldron", BaseCapacity = 1 };
        }

        // no ids
        public static class StoreItems
        {
            public class StoreItemConfig
            {
                public StoreItemConfig()
                {

                }

                public long BasePrice;
                public Store.Category Category;
                // the maximum of this item you can purchase. 0 means infinite.
                public int MaxQuantity = 1;
                // determines the exponential increase in price. 1 means none.
                public double Factor = 1;
            }

            public static StoreItemConfig EmptyVial = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.ITEMS, MaxQuantity = 0 };
            public static StoreItemConfig Gunpowder = new StoreItemConfig() { BasePrice = 2500, Category = Store.Category.ITEMS, MaxQuantity = 0 };
            public static StoreItemConfig Researcher = new StoreItemConfig() { BasePrice = 1000 * 1000, Category = Store.Category.GATHERING };
            public static StoreItemConfig Foreman = new StoreItemConfig() { BasePrice = 250000, Category = Store.Category.GATHERING };
            public static StoreItemConfig Botanist = new StoreItemConfig() { BasePrice = 1000 * 1000 * 100, Category = Store.Category.GATHERING };

            public static StoreItemConfig Miner = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Lumberjack = new StoreItemConfig() { BasePrice = 20000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Drill = new StoreItemConfig() { BasePrice = 1000 * 1000, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Crusher = new StoreItemConfig() { BasePrice = 1000 * 1000 * 5, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Excavator = new StoreItemConfig() { BasePrice = 1000 * 1000 * 500, Category = Store.Category.MACHINES, MaxQuantity = 10, Factor = 1.15 };
            public static StoreItemConfig Pumpjack = new StoreItemConfig() { BasePrice = 100000, Category = Store.Category.MACHINES, MaxQuantity = 100, Factor = 1.15 };
            public static StoreItemConfig BigTexan = new StoreItemConfig() { BasePrice = 2500000, Category = Store.Category.MACHINES, MaxQuantity = 100, Factor = 1.15 };

            public static StoreItemConfig ChainsawsT1 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig ChainsawsT2 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig ChainsawsT3 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig ChainsawsT4 = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig Backpack = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };


            public static StoreItemConfig ClickUpgradeT1 = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.MINING };
            public static StoreItemConfig ClickUpgradeT2 = new StoreItemConfig() { BasePrice = 100000, Category = Store.Category.MINING };
            public static StoreItemConfig ClickUpgradeT3 = new StoreItemConfig() { BasePrice = 1000000, Category = Store.Category.MINING };

            public static StoreItemConfig ReinforcedFurnace = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };
            public static StoreItemConfig LargerCauldron = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };

            public static StoreItemConfig DeeperTunnels = new StoreItemConfig() { BasePrice = 0, Category = Store.Category.CRAFTING };

            public static StoreItemConfig IronPickaxe = new StoreItemConfig() { BasePrice = 1000, Category = Store.Category.MINING };
            public static StoreItemConfig SteelPickaxe = new StoreItemConfig() { BasePrice = 20000, Category = Store.Category.MINING };
            public static StoreItemConfig GoldPickaxe = new StoreItemConfig() { BasePrice = 100000, Category = Store.Category.MINING };
            public static StoreItemConfig DiamondPickaxe = new StoreItemConfig() { BasePrice = 1000 * 1000, Category = Store.Category.MINING };

            public static StoreItemConfig Furnace = new StoreItemConfig() { BasePrice = 1000 * 1000, Category = Store.Category.PROCESSING };
            public static StoreItemConfig Cauldron = new StoreItemConfig() { BasePrice = 1000 * 1000 * 25, Category = Store.Category.PROCESSING };
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
                        LowestId = 301;
                        Id = 301;
                    }
                }
                public int Duration;
            }

            public static BuffConfig SpeechBuff = new BuffConfig() { Name = "Speech Potion", Duration = 30 };

            public static UpgradeConfig Backpack = new UpgradeConfig() { Name = "Backpack" };
            public static UpgradeConfig Researcher = new UpgradeConfig() { Name = "Researcher" };
            public static UpgradeConfig Botanist = new UpgradeConfig() { Name = "Botanist" };
            public static UpgradeConfig Foreman = new UpgradeConfig() { Name = "Foreman" };

            public static UpgradeConfig ChainsawsT1 = new UpgradeConfig() { Name = "Chainsaws" };
            public static UpgradeConfig ChainsawsT2 = new UpgradeConfig() { Name = "Steel Chainsaws" };

            public static UpgradeConfig ClickUpgradeT1 = new UpgradeConfig() { Name = "Stone Cursor" };
            public static UpgradeConfig ClickUpgradeT2 = new UpgradeConfig() { Name = "Copper Cursor" };
            public static UpgradeConfig ClickUpgradeT3 = new UpgradeConfig() { Name = "Iron Cursor" };

            public static UpgradeConfig ChainsawsT3 = new UpgradeConfig() { Name = "Titanium Chainsaws" };
            public static UpgradeConfig ChainsawsT4 = new UpgradeConfig() { Name = "Diamond Chainsaws" };

            public static UpgradeConfig ReinforcedFurnace = new UpgradeConfig() { Name = "Reinforced Furnace" };
            public static UpgradeConfig LargerCauldron = new UpgradeConfig() { Name = "Larger Cauldron" };

            public static BuffConfig ClickingBuff = new BuffConfig() { Name = "Clicking Potion", Duration = 45 };
            public static BuffConfig SmeltingBuff = new BuffConfig() { Name = "Smelting Potion", Duration = 60 * 5 };
            public static BuffConfig AlchemyBuff = new BuffConfig() { Name = "Alchemy Potion", Duration = 60 * 3 };

            public static UpgradeConfig DeeperTunnels = new UpgradeConfig() { Name = "Deeper Tunnels" };
            public static UpgradeConfig IronPickaxe = new UpgradeConfig() { Name = "Iron Pickaxe" };
            public static UpgradeConfig SteelPickaxe = new UpgradeConfig() { Name = "Steel Pickaxe" };
            public static UpgradeConfig GoldPickaxe = new UpgradeConfig() { Name = "Gold Pickaxe" };
            public static UpgradeConfig DiamondPickaxe = new UpgradeConfig() { Name = "Diamond Pickaxe" };

            public static UpgradeConfig Furnace = new UpgradeConfig() { Name = "Furnace Unlock" };
            public static UpgradeConfig Cauldron = new UpgradeConfig() { Name = "Cauldron Unlock" };
        }
        //1200-1500
        public static class Achievements
        {
            public class AchievementConfig : Config
            {
                public AchievementConfig()
                {
                    if (LowestId < 1200 || LowestId > 1500)
                    {
                        LowestId = 1201;
                        Id = 1201;
                    }
                }

                public GoldRush.Achievements.AchievementType Type;
                public int Goal;
                public int Points;
            }

            public static AchievementConfig TimePlayedT1 = new AchievementConfig() { Name = "Hour Waster", Type = GoldRush.Achievements.AchievementType.TimePlayed, Goal = 60 * 60, Points = 1 };
            public static AchievementConfig TimePlayedT2 = new AchievementConfig() { Name = "Day Waster", Type = GoldRush.Achievements.AchievementType.TimePlayed, Goal = 60 * 60 * 24, Points = 1 };
            public static AchievementConfig TimePlayedT3 = new AchievementConfig() { Name = "Week Waster", Type = GoldRush.Achievements.AchievementType.TimePlayed, Goal = 60 * 60 * 24 * 7, Points = 1 };
            public static AchievementConfig TimePlayedT4 = new AchievementConfig() { Name = "Month Waster", Type = GoldRush.Achievements.AchievementType.TimePlayed, Goal = 60 * 60 * 24 * 7 * 4, Points = 2 };
            public static AchievementConfig TimePlayedT5 = new AchievementConfig() { Name = "Life Waster", Type = GoldRush.Achievements.AchievementType.TimePlayed, Goal = 60 * 60 * 24 * 7 * 4 * 3, Points = 2 };

            public static AchievementConfig MoneyT1 = new AchievementConfig() { Name = "Thousandaire", Type = GoldRush.Achievements.AchievementType.Money, Goal = 1000, Points = 1 };
            public static AchievementConfig MoneyT2 = new AchievementConfig() { Name = "Millionaire", Type = GoldRush.Achievements.AchievementType.Money, Goal = 1000 * 1000, Points = 2 };
            public static AchievementConfig MoneyT3 = new AchievementConfig() { Name = "Billionaire", Type = GoldRush.Achievements.AchievementType.Money, Goal = 1000 * 1000 * 1000, Points = 3 };

            public static AchievementConfig MinerT1 = new AchievementConfig() { Name = "Mouse Miner", Type = GoldRush.Achievements.AchievementType.RockClicks, Goal = 100, Points = 1 };
            public static AchievementConfig MinerT2 = new AchievementConfig() { Name = "Click Cave In", Type = GoldRush.Achievements.AchievementType.RockClicks, Goal = 1000, Points = 1 };
            public static AchievementConfig MinerT3 = new AchievementConfig() { Name = "Carpal Tunneling", Type = GoldRush.Achievements.AchievementType.RockClicks, Goal = 1000 * 10, Points = 2 };

            public static AchievementConfig OilT1 = new AchievementConfig() { Name = "Eminent Domain", Type=GoldRush.Achievements.AchievementType.Oil, Goal = 1, Points = 1 };
        }
        //1500-1800
        public static class Statistics
        {
            public class StatisticConfig : Config
            {
                public StatisticConfig()
                {
                    if (LowestId < 1500 || LowestId > 1800)
                    {
                        LowestId = 1501;
                        Id = 1501;
                    }
                }
            }
            public static StatisticConfig TimePlayed = new StatisticConfig();
            public static StatisticConfig RockClicked = new StatisticConfig();
        }
    }
}
