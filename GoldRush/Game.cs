using Caroline.App.Models;

namespace GoldRush
{
    class Game : IGoldRushGame
    {
        public Game()
        {
            objs = new GameObjects();
        }
        public GameObjects objs;


        public GameState Update(ClientActions message)
        {
            // TODO
            //return new GameState();
          

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
