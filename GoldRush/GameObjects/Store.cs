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
            EmptyVial = new StoreItem(game.Items.EmptyVial, GameConfig.StoreItems.EmptyVial);
            All.Add(EmptyVial.Item.Id, EmptyVial);

            Gunpowder = new StoreItem(game.Items.Gunpowder, GameConfig.StoreItems.Gunpowder);
            All.Add(Gunpowder.Item.Id, Gunpowder);

            Researcher = new StoreItem(game.Upgrades.Researcher, GameConfig.StoreItems.Researcher);
            All.Add(Researcher.Item.Id, Researcher);

            // ENABLE AFTER BACKPACK CRAFTING.
            Botanist = new StoreItem(game.Upgrades.Botanist, GameConfig.StoreItems.Botanist);
            //All.Add(Botanist.Item.Id, Botanist);

            Foreman = new StoreItem(game.Upgrades.Foreman, GameConfig.StoreItems.Foreman);
            All.Add(Foreman.Item.Id, Foreman);

            // GATHERERS

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

            ClickUpgradeT1 = new StoreItem(game.Upgrades.ClickUpgradeT1, GameConfig.StoreItems.ClickUpgradeT1);
            All.Add(ClickUpgradeT1.Item.Id, ClickUpgradeT1);

            ClickUpgradeT2 = new StoreItem(game.Upgrades.ClickUpgradeT2, GameConfig.StoreItems.ClickUpgradeT2);
            All.Add(ClickUpgradeT2.Item.Id, ClickUpgradeT2);

            ClickUpgradeT3 = new StoreItem(game.Upgrades.ClickUpgradeT3, GameConfig.StoreItems.ClickUpgradeT3);
            All.Add(ClickUpgradeT3.Item.Id, ClickUpgradeT3);

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

        internal class StoreItem
        {
            public GameObjects.GameObject Item;
            public GameObjects.GameObject Currency;
            public int MaxQuantity { get { return _config.MaxQuantity; } }
            public Category Category { get { return _config.Category; } }
            public int BasePrice { get { return _config.BasePrice; } }
            public double Factor { get { return _config.Factor; } }

            GoldRush.GameConfig.StoreItems.StoreItemConfig _config;

            public StoreItem(GameObjects.GameObject item,GameConfig.StoreItems.StoreItemConfig config)
            {
                Item = item;
                _config = config;
            }

            public int GetPrice()
            {
                return (int)Math.Ceiling((Math.Pow(Factor, Item.Quantity))*Convert.ToDouble(BasePrice));
            }

            public void Purchase()
            {
                Purchase(1);
            }

            public void Purchase(int quantity)
            {
                if ((Item.Quantity+quantity) > MaxQuantity && MaxQuantity > 0) return;

                int price = GetPrice();
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
