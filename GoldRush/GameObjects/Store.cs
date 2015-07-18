using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Store
    {
        public Store(GameObjects game)
        {
            var coins = game.Items.Coins;
            //TODO: Finish up the store.
            
            // Items

            var emptyVial = new StoreItem(game.Items.EmptyVial, GameConfig.StoreItems.EmptyVial);
            All.Add(emptyVial.Item.Id, emptyVial);

            var gunpowder = new StoreItem(game.Items.Gunpowder, GameConfig.StoreItems.Gunpowder);
            All.Add(gunpowder.Item.Id, gunpowder);

            // Upgrades

            var backpack = new StoreItem(game.Upgrades.Backpack, GameConfig.StoreItems.Backpack);
            All.Add(backpack.Item.Id, backpack);

            var researcher = new StoreItem(game.Upgrades.Researcher, GameConfig.StoreItems.Researcher);
            All.Add(researcher.Item.Id, researcher);

            var botanist = new StoreItem(game.Upgrades.Botanist, GameConfig.StoreItems.Botanist);
            All.Add(botanist.Item.Id, botanist);

            var foreman = new StoreItem(game.Upgrades.Foreman, GameConfig.StoreItems.Foreman);
            All.Add(foreman.Item.Id, foreman);

            var chainsawsT1 = new StoreItem(game.Upgrades.ChainsawsT1, GameConfig.StoreItems.ChainsawsT1);
            All.Add(chainsawsT1.Item.Id, chainsawsT1);

            var chainsawsT2 = new StoreItem(game.Upgrades.ChainsawsT2, GameConfig.StoreItems.ChainsawsT2);
            All.Add(chainsawsT2.Item.Id, chainsawsT2);
            
            var clickUpgradeT1 = new StoreItem(game.Upgrades.ClickUpgradeT1, GameConfig.StoreItems.ClickUpgradeT1);
            All.Add(clickUpgradeT1.Item.Id, clickUpgradeT1);

            var clickUpgradeT2 = new StoreItem(game.Upgrades.ClickUpgradeT2, GameConfig.StoreItems.ClickUpgradeT2);
            All.Add(clickUpgradeT2.Item.Id, clickUpgradeT2);

            var clickUpgradeT3 = new StoreItem(game.Upgrades.ClickUpgradeT3, GameConfig.StoreItems.ClickUpgradeT3);
            All.Add(clickUpgradeT3.Item.Id, clickUpgradeT3);

            var chainsawsT3 = new StoreItem(game.Upgrades.ChainsawsT3, GameConfig.StoreItems.ChainsawsT3);
            All.Add(chainsawsT3.Item.Id, chainsawsT3);

            var chainsawsT4 = new StoreItem(game.Upgrades.ChainsawsT4, GameConfig.StoreItems.ChainsawsT4);
            All.Add(chainsawsT4.Item.Id, chainsawsT4);

            var reinforcedFurnace = new StoreItem(game.Upgrades.ReinforcedFurnace,GameConfig.StoreItems.ReinforcedFurnace);
            All.Add(reinforcedFurnace.Item.Id, reinforcedFurnace);

            var largerCauldron = new StoreItem(game.Upgrades.LargerCauldron,GameConfig.StoreItems.LargerCauldron);
            All.Add(largerCauldron.Item.Id, largerCauldron);

            var deeperTunnels = new StoreItem(game.Upgrades.DeeperTunnels,GameConfig.StoreItems.DeeperTunnels);
            All.Add(deeperTunnels.Item.Id,deeperTunnels);

            var ironPickaxe = new StoreItem(game.Upgrades.IronPickaxe,GameConfig.StoreItems.IronPickaxe);
            All.Add(ironPickaxe.Item.Id,ironPickaxe);

            var steelPickaxe = new StoreItem(game.Upgrades.SteelPickaxe, GameConfig.StoreItems.SteelPickaxe);
            All.Add(steelPickaxe.Item.Id, steelPickaxe);

            var goldPickaxe = new StoreItem(game.Upgrades.GoldPickaxe, GameConfig.StoreItems.GoldPickaxe);
            All.Add(goldPickaxe.Item.Id, goldPickaxe);

            var diamondPickaxe = new StoreItem(game.Upgrades.DiamondPickaxe, GameConfig.StoreItems.DiamondPickaxe);
            All.Add(diamondPickaxe.Item.Id, diamondPickaxe);

            var furnace = new StoreItem(game.Upgrades.Furnace, GameConfig.StoreItems.Furnace);
            All.Add(furnace.Item.Id,furnace);

            var cauldron = new StoreItem(game.Upgrades.Cauldron, GameConfig.StoreItems.Cauldron);
            All.Add(cauldron.Item.Id,cauldron);

            // Gatherers

            var miner = new StoreItem(game.Gatherers.Miner, GameConfig.StoreItems.Miner);
            All.Add(miner.Item.Id, miner);

            var lumberjack = new StoreItem(game.Gatherers.Lumberjack, GameConfig.StoreItems.Lumberjack);
            All.Add(lumberjack.Item.Id, lumberjack);

            var drill = new StoreItem(game.Gatherers.Drill, GameConfig.StoreItems.Drill);
            All.Add(drill.Item.Id, drill);

            var crusher = new StoreItem(game.Gatherers.Crusher, GameConfig.StoreItems.Crusher);
            All.Add(crusher.Item.Id, crusher);

            var excavator = new StoreItem(game.Gatherers.Excavator, GameConfig.StoreItems.Excavator);
            All.Add(excavator.Item.Id, excavator);

            var pumpjack = new StoreItem(game.Gatherers.Pumpjack, GameConfig.StoreItems.Pumpjack);
            All.Add(pumpjack.Item.Id, pumpjack);

            var bigTexan = new StoreItem(game.Gatherers.BigTexan, GameConfig.StoreItems.BigTexan);
            All.Add(bigTexan.Item.Id, bigTexan);

            var geologist = new StoreItem(game.Upgrades.Geologist, GameConfig.StoreItems.Geologist);
            All.Add(geologist.Item.Id, geologist);


            // If you want an item to be purchased with a different currency define it above.
            foreach (var storeItem in All.Where(storeItem => storeItem.Value.Currency == null))
            {
                storeItem.Value.Currency = coins;
            }
        }

        public Dictionary<int, StoreItem> All = new Dictionary<int, StoreItem>();

        internal class StoreItem
        {
            public GameObjects.GameObject Item;
            public GameObjects.GameObject Currency;
            public int MaxQuantity { get { return _config.MaxQuantity; } }
            public Category Category { get { return _config.Category; } }
            public long BasePrice { get { return _config.BasePrice; } }
            public double Factor { get { return _config.Factor; } }

            GoldRush.GameConfig.StoreItems.StoreItemConfig _config;

            public StoreItem(GameObjects.GameObject item,GameConfig.StoreItems.StoreItemConfig config)
            {
                Item = item;
                _config = config;
            }

            public long GetPrice()
            {
                return (long)Math.Ceiling((Math.Pow(Factor, Item.Quantity))*Convert.ToDouble(BasePrice));
            }

            public void Purchase()
            {
                Purchase(1);
            }

            public void Purchase(int quantity)
            {
                if (Category == Category.CRAFTING) return;

                if ((Item.Quantity+quantity) > MaxQuantity && MaxQuantity > 0) return;

                long price = GetPrice();
                if ((price * quantity) > Currency.Quantity) return;

                Currency.Quantity -= price*quantity;
                Item.Quantity+=quantity;
            }
        }

        public enum Category
        {
            MINING = 1,
            MACHINES = 2,
            GATHERING = 3,
            PROCESSING = 4,
            ITEMS = 5,
            CRAFTING=6
        };
    }
}
