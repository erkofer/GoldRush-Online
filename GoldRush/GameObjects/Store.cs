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

            EmptyVial = new StoreItem(game.Items.EmptyVial, GameConfig.StoreItems.EmptyVial);
            All.Add(EmptyVial.Item.Id, EmptyVial);

            Gunpowder = new StoreItem(game.Items.Gunpowder, GameConfig.StoreItems.Gunpowder);
            All.Add(Gunpowder.Item.Id, Gunpowder);

            // Upgrades

            Backpack = new StoreItem(game.Upgrades.Backpack, GameConfig.StoreItems.Backpack);
            All.Add(Backpack.Item.Id, Backpack);

            Researcher = new StoreItem(game.Upgrades.Researcher, GameConfig.StoreItems.Researcher);
            All.Add(Researcher.Item.Id, Researcher);

            // ENABLE AFTER BACKPACK CRAFTING.
            Botanist = new StoreItem(game.Upgrades.Botanist, GameConfig.StoreItems.Botanist);
            All.Add(Botanist.Item.Id, Botanist);

            Foreman = new StoreItem(game.Upgrades.Foreman, GameConfig.StoreItems.Foreman);
            All.Add(Foreman.Item.Id, Foreman);

            ChainsawsT1 = new StoreItem(game.Upgrades.ChainsawsT1, GameConfig.StoreItems.ChainsawsT1);
            All.Add(ChainsawsT1.Item.Id, ChainsawsT1);

            ChainsawsT2 = new StoreItem(game.Upgrades.ChainsawsT2, GameConfig.StoreItems.ChainsawsT2);
            All.Add(ChainsawsT2.Item.Id, ChainsawsT2);
            
            ClickUpgradeT1 = new StoreItem(game.Upgrades.ClickUpgradeT1, GameConfig.StoreItems.ClickUpgradeT1);
            All.Add(ClickUpgradeT1.Item.Id, ClickUpgradeT1);

            ClickUpgradeT2 = new StoreItem(game.Upgrades.ClickUpgradeT2, GameConfig.StoreItems.ClickUpgradeT2);
            All.Add(ClickUpgradeT2.Item.Id, ClickUpgradeT2);

            ClickUpgradeT3 = new StoreItem(game.Upgrades.ClickUpgradeT3, GameConfig.StoreItems.ClickUpgradeT3);
            All.Add(ClickUpgradeT3.Item.Id, ClickUpgradeT3);

            ChainsawsT3 = new StoreItem(game.Upgrades.ChainsawsT3, GameConfig.StoreItems.ChainsawsT3);
            All.Add(ChainsawsT3.Item.Id, ChainsawsT3);

            ChainsawsT4 = new StoreItem(game.Upgrades.ChainsawsT4, GameConfig.StoreItems.ChainsawsT4);
            All.Add(ChainsawsT4.Item.Id, ChainsawsT4);

            ReinforcedFurnace = new StoreItem(game.Upgrades.ReinforcedFurnace,GameConfig.StoreItems.ReinforcedFurnace);
            All.Add(ReinforcedFurnace.Item.Id, ReinforcedFurnace);

            LargerCauldron = new StoreItem(game.Upgrades.LargerCauldron,GameConfig.StoreItems.LargerCauldron);
            All.Add(LargerCauldron.Item.Id, LargerCauldron);

            DeeperTunnels = new StoreItem(game.Upgrades.DeeperTunnels,GameConfig.StoreItems.DeeperTunnels);
            All.Add(DeeperTunnels.Item.Id,DeeperTunnels);

            IronPickaxe = new StoreItem(game.Upgrades.IronPickaxe,GameConfig.StoreItems.IronPickaxe);
            All.Add(IronPickaxe.Item.Id,IronPickaxe);

            SteelPickaxe = new StoreItem(game.Upgrades.SteelPickaxe, GameConfig.StoreItems.SteelPickaxe);
            All.Add(SteelPickaxe.Item.Id, SteelPickaxe);

            GoldPickaxe = new StoreItem(game.Upgrades.GoldPickaxe, GameConfig.StoreItems.GoldPickaxe);
            All.Add(GoldPickaxe.Item.Id, GoldPickaxe);

            DiamondPickaxe = new StoreItem(game.Upgrades.DiamondPickaxe, GameConfig.StoreItems.DiamondPickaxe);
            All.Add(DiamondPickaxe.Item.Id, DiamondPickaxe);

            // Gatherers

            Miner = new StoreItem(game.Gatherers.Miner, GameConfig.StoreItems.Miner);
            All.Add(Miner.Item.Id, Miner);

            Lumberjack = new StoreItem(game.Gatherers.Lumberjack, GameConfig.StoreItems.Lumberjack);
            All.Add(Lumberjack.Item.Id, Lumberjack);

            Drill = new StoreItem(game.Gatherers.Drill, GameConfig.StoreItems.Drill);
            All.Add(Drill.Item.Id, Drill);

            Crusher = new StoreItem(game.Gatherers.Crusher, GameConfig.StoreItems.Crusher);
            All.Add(Crusher.Item.Id, Crusher);

            Excavator = new StoreItem(game.Gatherers.Excavator, GameConfig.StoreItems.Excavator);
            All.Add(Excavator.Item.Id, Excavator);

            Pumpjack = new StoreItem(game.Gatherers.Pumpjack, GameConfig.StoreItems.Pumpjack);
            All.Add(Pumpjack.Item.Id, Pumpjack);

            BigTexan = new StoreItem(game.Gatherers.BigTexan, GameConfig.StoreItems.BigTexan);
            All.Add(BigTexan.Item.Id, BigTexan);


            // If you want an item to be purchased with a different currency define it above.
            foreach (var storeItem in All)
            {
                if (storeItem.Value.Currency != null) continue;
                storeItem.Value.Currency = coins;
            }
        }

        public Dictionary<int, StoreItem> All = new Dictionary<int, StoreItem>();

        StoreItem EmptyVial;
        StoreItem Gunpowder;
        StoreItem Researcher;
        StoreItem Botanist;
        private StoreItem Backpack;
        StoreItem Foreman;

        StoreItem Miner;
        StoreItem Lumberjack;
        StoreItem Drill;
        StoreItem Crusher;
        StoreItem Excavator;
        StoreItem Pumpjack;
        StoreItem BigTexan;

        StoreItem ClickUpgradeT1;
        StoreItem ClickUpgradeT2;
        StoreItem ClickUpgradeT3;

        StoreItem ChainsawsT1;
        StoreItem ChainsawsT2;
        StoreItem ChainsawsT3;
        StoreItem ChainsawsT4;

        StoreItem ReinforcedFurnace;
        StoreItem LargerCauldron;

        private StoreItem DeeperTunnels;

        private StoreItem IronPickaxe;
        private StoreItem SteelPickaxe;
        private StoreItem GoldPickaxe;
        private StoreItem DiamondPickaxe;

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
                return (int)Math.Ceiling((Math.Pow(Factor, Item.Quantity))*Convert.ToDouble(BasePrice));
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
