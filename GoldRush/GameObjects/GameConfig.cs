using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    internal class GameConfig
    {
        public class Config
        {
            public string Name;
        }

        public class Items
        {
            public class ItemConfig : Config
            {
                public int Worth;
            }

            public class ResourceConfig : ItemConfig
            {
                public int Probability;
            }

            public static ResourceConfig Stone = new ResourceConfig {Name = "Stone", Worth = 1, Probability = 3500000};
            public static ResourceConfig Copper = new ResourceConfig {Name = "Copper", Worth = 5, Probability = 2000000};
            public static ResourceConfig Iron = new ResourceConfig {Name = "Iron", Worth = 20, Probability = 1500000};
            public static ResourceConfig Silver = new ResourceConfig {Name="Silver",Worth=100,Probability = 1000000};
            public static ResourceConfig Gold = new ResourceConfig {Name = "Gold", Worth = 1000, Probability = 500000};
            public static ResourceConfig Uranium = new ResourceConfig {Name = "Uranium", Worth = 5000, Probability = 5000};
            public static ResourceConfig Titanium = new ResourceConfig{Name = "Titanium", Worth = 1000000, Probability = 25};
            public static ResourceConfig Opal = new ResourceConfig {Name = "Opal", Worth = 2000, Probability = 25000};
            public static ResourceConfig Jade = new ResourceConfig {Name = "Jade", Worth = 5000, Probability = 20000};
            public static ResourceConfig Topaz = new ResourceConfig {Name = "Topaz", Worth = 10000, Probability = 15000};
            public static ResourceConfig Sapphire = new ResourceConfig{Name = "Sapphire", Worth = 25000, Probability = 10000};
            public static ResourceConfig Emerald = new ResourceConfig{Name = "Emerald", Worth = 50000, Probability = 5000};
            public static ResourceConfig Ruby = new ResourceConfig {Name = "Ruby", Worth = 100000, Probability = 2500};
            public static ResourceConfig Onyx = new ResourceConfig {Name = "Onyx", Worth = 250000, Probability = 1000};
            public static ResourceConfig Quartz = new ResourceConfig {Name = "Quartz", Worth = 500000, Probability = 50};
            public static ResourceConfig Diamond = new ResourceConfig {Name = "Diamond", Worth = 5000000, Probability = 20 };
            public static ItemConfig BronzeBar = new ItemConfig {Name = "Bronze bar", Worth = 250};
            public static ItemConfig IronBar = new ItemConfig {Name = "Iron bar", Worth = 1000};
            public static ItemConfig SilverBar = new ItemConfig {Name = "Silver bar", Worth = 2500};
            public static ItemConfig SteelBar = new ItemConfig {Name = "Steel bar", Worth = 5000};
            public static ItemConfig GoldBar = new ItemConfig {Name = "Gold bar", Worth = 25000};
            public static ItemConfig TitaniumBar = new ItemConfig {Name = "Titanium bar", Worth = 5000000};
            public static ResourceConfig BitterRoot = new ResourceConfig{Name = "Bitter root", Worth = 10000,Probability = 2000};
            public static ResourceConfig Cubicula = new ResourceConfig{Name = "Cubicula", Worth = 25000, Probability = 1500};
            public static ResourceConfig IronFlower = new ResourceConfig{Name="Iron flower", Worth = 500000, Probability = 250};
            public static ResourceConfig TongtwistaFlower = new ResourceConfig{Name="Tongtwista flower", Worth = 1000000, Probability = 100};
            public static ResourceConfig Thornberries = new ResourceConfig{Name="Thornberries",Worth=1000,Probability = 2000};
            public static ResourceConfig Transfruit = new ResourceConfig{Name="Transfruit",Worth=5000,Probability = 1000};
            public static ResourceConfig MeltingNuts = new ResourceConfig{Name="Melting nuts",Worth=10000, Probability = 500};
            public static ItemConfig EmptyVial = new ItemConfig {Name = "Empty vial", Worth = 500};
            public static ItemConfig Gunpowder = new ItemConfig { Name = "Gunpowder", Worth = 1250};
            public static ResourceConfig Logs = new ResourceConfig { Name = "Logs", Worth = 250,Probability=0};
            public static ResourceConfig Oil = new ResourceConfig { Name = "Oil", Worth = 300, Probability=0};
            public static ItemConfig Coins = new ItemConfig {Name = "Coins", Worth = 0};
            public static ItemConfig ClickingPotion = new ItemConfig {Name = "Clicking Potion", Worth = 25000};
            public static ItemConfig SmeltingPotion = new ItemConfig { Name = "Smelting Potion", Worth = 50000};
            public static ItemConfig SpeechPotion = new ItemConfig { Name = "Speech Potion", Worth = 100000};
            public static ItemConfig AlchemyPotion = new ItemConfig { Name = "Alchemy Potion", Worth = 250000};
            public static ItemConfig CopperWire = new ItemConfig {Name = "Copper wire", Worth = 250};
            public static ItemConfig Tnt = new ItemConfig {Name = "Tnt", Worth = 100000};
        }

        public class Gatherers
        {
            public class GathererConfig : Config
            {
                public double BaseResourcesPerSecond;
            }

           
            public static GathererConfig Miner = new GathererConfig() {Name = "Miner", BaseResourcesPerSecond = 0.5};
            public static GathererConfig Lumberjack = new GathererConfig() {Name = "Lumberjack", BaseResourcesPerSecond = 0.5};
            public static GathererConfig Pumpjack = new GathererConfig(){Name="Pumpjack", BaseResourcesPerSecond = 0.25};
        }
    }
}
