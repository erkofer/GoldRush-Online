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
            //TODO: Finish up the store.-+
            EmptyVial = new StoreItem(game.Items.EmptyVial, GameConfig.StoreItems.EmptyVial);
            All.Add(EmptyVial.Item.Id, EmptyVial);

            Gunpowder = new StoreItem(game.Items.Gunpowder, GameConfig.StoreItems.Gunpowder);
            All.Add(Gunpowder.Item.Id, Gunpowder);

            Researcher = new StoreItem(game.Upgrades.Researcher, GameConfig.StoreItems.Researcher);
            All.Add(Researcher.Item.Id, Researcher);

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
                if (Item.Quantity > MaxQuantity && MaxQuantity>0) return;

                int price = GetPrice();
                if (price > Currency.Quantity) return;
                
                Currency.Quantity -= price;
                Item.Quantity++;
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
