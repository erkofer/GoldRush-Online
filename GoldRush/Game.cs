using Caroline.App.Models;
using System;

namespace GoldRush
{
    class Game : IGoldRushGame
    {
        public Game()
        {
            objs = new GameObjects();
            lastUpdate = UnixTimeNow();
        }
        public GameObjects objs;
        long lastUpdate;

        public long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public GameState Update(ClientActions message)
        {
            // TODO
            //return new GameState();
            var currentTime = UnixTimeNow();
            objs.Update((int)((currentTime - lastUpdate) * 1000));
            lastUpdate = currentTime;


            // CLIENT ACTIONS
            if (message != null)
            {
                if (message.InventoryActions.Count > 0)
                {
                    for (var i = 0; i < message.InventoryActions.Count; i++)
                    {
                        if (message.InventoryActions[i].Sell != null)
                        {
                            var item = objs.Items.All[message.InventoryActions[i].Sell.Id];
                            item.Sell(message.InventoryActions[i].Sell.Quantity);
                        }
                    }
                }
                if (message.StoreActions.Count > 0)
                {
                    for (var i = 0; i < message.StoreActions.Count; i++)
                    {
                        if (message.StoreActions[i].Purchase != null)
                        {
                            var item = objs.Store.All[message.StoreActions[i].Purchase.Id];
                            item.Purchase(message.StoreActions[i].Purchase.Quantity);
                        }
                    }
                }
            }

            // SEND DATA TO CLIENT
            var state = new GameState();

            var sendSchema = true;
            // SCHEMATIC
            if (sendSchema)
            {
                var schema = new GameState.Schematic();
                // INVENTORY
                foreach (var item in objs.Items.All)
                {
                    var schemaItem = new GameState.Schematic.SchemaItem();
                    schemaItem.Id = item.Value.Id;
                    schemaItem.Name = item.Value.Name;
                    schemaItem.Worth = item.Value.Worth;
                    schema.Items.Add(schemaItem);
                }
                // STORE
                foreach (var item in objs.Store.All)
                {
                    var schemaItem = new GameState.Schematic.SchemaStoreItem();
                    schemaItem.Id = item.Value.Item.Id;
                    schemaItem.Name = item.Value.Item.Name;
                    schemaItem.Price = item.Value.BasePrice;
                    schemaItem.Factor = item.Value.Factor;
                    schemaItem.MaxQuantity = item.Value.MaxQuantity;
                    schemaItem.Category = (GameState.Schematic.SchemaStoreItem.Section)item.Value.Category;
                    schema.StoreItems.Add(schemaItem);
                }

                state.GameSchema = schema;
            }

            // INVENTORY DATA
            foreach (var item in objs.Items.All)
            {
                var stateItem = new GameState.Item();
                stateItem.Id = item.Value.Id;
                stateItem.Quantity = item.Value.Quantity;

                state.Items.Add(stateItem);
            }
            
            return state;
        }


        public SaveState Save()
        {
            var saveState = new SaveState();
            foreach (var item in objs.Items.All)
            {
                var stateItem = new SaveState.Item();
                var toSaveItem = item.Value;
                stateItem.Id = toSaveItem.Id;
                stateItem.Quantity = toSaveItem.Value;
                stateItem.PrestigeQuantity = toSaveItem.PrestigeTimeTotal;
                stateItem.AlltimeQuantity = toSaveItem.LifeTimeTotal;
                saveState.Items.Add(stateItem);
            }
            return new SaveState();
        }

        public void Load(SaveState save)
        {
            if (save != null)
            {
                if (save.Items != null)
                {
                    foreach (var item in save.Items)
                    {
                        var toLoadItem = objs.Items.All[item.Id];
                        toLoadItem.Quantity = item.Quantity;
                        toLoadItem.PrestigeTimeTotal = item.PrestigeQuantity;
                        toLoadItem.LifeTimeTotal = item.AlltimeQuantity;
                    }
                }
            }
        }
    }
}
