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
            //TODO
            return new SaveState();
        }

        public void Load(SaveState save)
        {
            //TODO
        }
    }
}
