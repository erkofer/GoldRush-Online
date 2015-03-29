using Caroline.App.Models;
using System;

namespace GoldRush
{
    class Game
    {
        public Game()
        {
            objs = new GameObjects();
            lastUpdate = UnixTimeNow();
            sendSchema = true;
        }

        bool sendSchema;
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
                        var msg = message.InventoryActions[i];
                        if (msg.Sell != null)
                        {
                            var item = objs.Items.All[msg.Sell.Id];
                            item.Sell(msg.Sell.Quantity);
                        }
                        if(msg.Config != null)
                        {
                            var item = objs.Items.All[msg.Config.Id];
                            item.IncludeInSellAll = msg.Config.Enabled;;
                        }
                        if(msg.SellAll == true)
                        {
                            objs.Items.SellAll();
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

            
          

            // SCHEMATIC
            if (sendSchema)
            {
                var schema = new GameState.Schematic();
                sendSchema = false;
                // INVENTORY
                foreach (var item in objs.Items.All)
                {
                    var schemaItem = new GameState.Schematic.SchemaItem();
                    schemaItem.Id = item.Value.Id;
                    schemaItem.Name = item.Value.Name;
                    schemaItem.Worth = item.Value.Worth;
                    schemaItem.Category = (GameState.Schematic.SchemaItem.Section)item.Value.Category;
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

            // INVENTORY & STATS DATA
            foreach (var item in objs.Items.All)
            {
                var stateItem = new GameState.Item();
                stateItem.Id = item.Value.Id;
                stateItem.Quantity = item.Value.Quantity;


                var stateStatsItem = new GameState.StatItem();
                stateStatsItem.Id = item.Value.Id;
                stateStatsItem.PrestigeQuantity = item.Value.PrestigeTimeTotal;
                stateStatsItem.LifeTimeQuantity = item.Value.LifeTimeTotal;

                state.StatItemsUpdate.Add(stateStatsItem);
                state.Items.Add(stateItem);

            }
            
            


            // STORE DATA
            foreach (var item in objs.Store.All)
            {
                var stateStoreItem = new GameState.StoreItem();
                GameObjects.GameObject gameobject;
                objs.All.TryGetValue(item.Key,out gameobject);
                stateStoreItem.Id = gameobject.Id;
                stateStoreItem.Quantity = gameobject.Quantity;

                if (gameobject.Requires != null && gameobject.Requires.Active == false)// if the gameobject requires something and is not active.
                    stateStoreItem.Quantity = -1;

                state.StoreItemsUpdate.Add(stateStoreItem);
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
                stateItem.Quantity = toSaveItem.Quantity;
                stateItem.PrestigeQuantity = toSaveItem.PrestigeTimeTotal;
                stateItem.AlltimeQuantity = toSaveItem.LifeTimeTotal;
                saveState.Items.Add(stateItem);
            }
            return saveState;
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
