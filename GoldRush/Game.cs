using System;
using Caroline.Persistence.Models;
using Caroline.App.Models;

namespace GoldRush
{
    class Game
    {
        public Game()
        {
            objs = new GameObjects();
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
            if (lastUpdate == 0) 
                lastUpdate = currentTime;

            long timeSinceLastUpdate = currentTime - lastUpdate;
            if (timeSinceLastUpdate > 0)
            {
                lastUpdate = currentTime;
                objs.Update((int) timeSinceLastUpdate*1000);
            }



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
                        if (msg.Config != null)
                        {
                            var item = objs.Items.All[msg.Config.Id];
                            item.IncludeInSellAll = msg.Config.Enabled; ;
                        }
                        if (msg.SellAll == true)
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
                if (message.CraftingActions.Count > 0)
                {
                    for (var i = 0; i < message.CraftingActions.Count; i++)
                    {
                        objs.Crafting.Craft(message.CraftingActions[i].Id, message.CraftingActions[i].Quantity);
                    }
                }
                if (message.ProcessingActions.Count > 0)
                {
                    for (var i = 0; i < message.ProcessingActions.Count; i++)
                    {
                        objs.Crafting.Process(message.ProcessingActions[i].Id, message.ProcessingActions[i].RecipeIndex, message.ProcessingActions[i].Iterations);
                    }
                }
                if (message.MiningActions.Count > 0)
                {
                    for (var i = 0; i < message.MiningActions.Count; i++)
                    {
                        objs.Gatherers.Mine(message.MiningActions[i].X,message.MiningActions[i].Y);
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
                // Crafting
                foreach (var item in objs.Crafting.All)
                {
                    var schemaItem = new GameState.Schematic.SchemaCraftingItem();
                    schemaItem.Id = item.Value.Id;

                    foreach (var ing in item.Value.Ingredients)
                    {
                        var ingredient = new GameState.Schematic.SchemaCraftingItem.Ingredient();
                        ingredient.Id = ing.Item.Id;
                        ingredient.Quantity = ing.Quantity;
                        schemaItem.Ingredients.Add(ingredient);
                    }

                    foreach (var res in item.Value.Resultants)
                    {
                        var ingredient = new GameState.Schematic.SchemaCraftingItem.Ingredient();
                        ingredient.Id = res.Item.Id;
                        ingredient.Quantity = res.Quantity;
                        schemaItem.Resultants.Add(ingredient);
                    }

                    schemaItem.IsItem = (item.Value.Resultants[0].Item is GoldRush.Items.Item);

                    schema.CraftingItems.Add(schemaItem);
                }
                // Processing
                foreach (var processor in objs.Crafting.Processors)
                {
                    var schemaItem = new GameState.Schematic.SchemaProcessor();
                    schemaItem.Id = processor.Value.Id;
                    schemaItem.Name = processor.Value.Name;

                    foreach (var recipe in processor.Value.Recipes)
                    {
                        var schemaRecipe = new GameState.Schematic.SchemaProcessor.Recipe();
                        foreach (var ingredient in recipe.Ingredients)
                        {
                            var schemaIngredient = new GameState.Schematic.SchemaProcessor.Recipe.Ingredient();
                            schemaIngredient.Id = ingredient.Item.Id;
                            schemaIngredient.Quantity = ingredient.Quantity;
                            schemaRecipe.Ingredients.Add(schemaIngredient);
                        }

                        foreach (var resultant in recipe.Resultants)
                        {
                            var schemaIngredient = new GameState.Schematic.SchemaProcessor.Recipe.Ingredient();
                            schemaIngredient.Id = resultant.Item.Id;
                            schemaIngredient.Quantity = resultant.Quantity;
                            schemaRecipe.Resultants.Add(schemaIngredient);
                        }
                        schemaRecipe.Duration = recipe.Duration;
                        schemaItem.Recipes.Add(schemaRecipe);
                    }
                    schema.Processors.Add(schemaItem);
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

                if (item.Value.Category != Items.Category.NOTFORSALE)
                {
                    var configItem = new GameState.ConfigItem();
                    configItem.Id = item.Value.Id;
                    configItem.Enabled = item.Value.IncludeInSellAll;
                    state.ConfigItems.Add(configItem);
                }
            }




            // STORE DATA
            foreach (var item in objs.Store.All)
            {
                var stateStoreItem = new GameState.StoreItem();
                GameObjects.GameObject gameobject;
                objs.All.TryGetValue(item.Key, out gameobject);
                stateStoreItem.Id = gameobject.Id;
                stateStoreItem.Quantity = gameobject.Quantity;
                stateStoreItem.MaxQuantity = item.Value.MaxQuantity;
                stateStoreItem.Price = item.Value.GetPrice();

                if (gameobject.Requires != null && gameobject.Requires.Active == false)// if the gameobject requires something and is not active.
                    stateStoreItem.Quantity = -1;

                state.StoreItemsUpdate.Add(stateStoreItem);
            }

            // PROCESSORS

            foreach (var processor in objs.Crafting.Processors)
            {
                var stateProcessor = new GameState.Processor();
                stateProcessor.Id = processor.Value.Id;
                stateProcessor.SelectedRecipe = processor.Value.SelectedRecipeIndex;
                stateProcessor.OperationDuration = (int)processor.Value.SelectedRecipeDuration;
                //stateProcessor.OperationCompletion = currentTime + (long)processor.Value.RemainingOperationTime;
                stateProcessor.CompletedOperations = processor.Value.RecipesCrafted;
                stateProcessor.TotalOperations = processor.Value.RecipesToCraft;
                stateProcessor.Capacity = processor.Value.Capacity;
                state.Processors.Add(stateProcessor);
            }

            // ANTICHEAT

            var stateAntiCheat = new GameState.AntiCheat();
            stateAntiCheat.X = objs.Gatherers.AntiCheatX;
            stateAntiCheat.Y = objs.Gatherers.AntiCheatY;

            state.AntiCheatCoordinates = stateAntiCheat;

            return state;
        }


        public SaveState Save()
        {
            var saveState = new SaveState();

            // Save last update time
            saveState.LastUpdate = lastUpdate;

            foreach (var item in objs.Items.All)
            {
                var stateItem = new SaveState.Item();
                var toSaveItem = item.Value;
                stateItem.Id = toSaveItem.Id;
                stateItem.Quantity = toSaveItem.Quantity;
                stateItem.PrestigeQuantity = toSaveItem.PrestigeTimeTotal;
                stateItem.AlltimeQuantity = toSaveItem.LifeTimeTotal;
                saveState.Items.Add(stateItem);

                var configStateItem = new SaveState.ItemConfig();
                configStateItem.Id = toSaveItem.Id;
                configStateItem.Enabled = toSaveItem.IncludeInSellAll;
                saveState.ItemConfigs.Add(configStateItem);
            }

            foreach (var gatherer in objs.Gatherers.All)
            {
                var toSaveGatherer = gatherer.Value;
                var saveStateGatherer = new SaveState.Gatherer();

                saveStateGatherer.Id = toSaveGatherer.Id;
                saveStateGatherer.Quantity = toSaveGatherer.Quantity;
                saveStateGatherer.ResourceBuffer = toSaveGatherer.ResourceBuffer;

                saveState.Gatherers.Add(saveStateGatherer);
            }

            foreach (var processor in objs.Crafting.Processors)
            {
                var toSaveProcessor = processor.Value;
                var saveStateProcessor = new SaveState.Processor();

                saveStateProcessor.Id = toSaveProcessor.Id;
                saveStateProcessor.SelectedRecipe = toSaveProcessor.SelectedRecipeIndex;
                saveStateProcessor.Progress = toSaveProcessor.RecipeProgress;
                saveStateProcessor.RecipesCrafted = toSaveProcessor.RecipesCrafted;
                saveStateProcessor.RecipesToCraft = toSaveProcessor.RecipesToCraft;

                saveState.Processors.Add(saveStateProcessor);
            }

            //Anticheat
            var anticheatSave = new SaveState.AntiCheat();
            anticheatSave.X = objs.Gatherers.AntiCheatX;
            anticheatSave.Y = objs.Gatherers.AntiCheatY;
            anticheatSave.NextChange = objs.Gatherers.AntiCheatNextChange;

            saveState.AntiCheatCoordinates = anticheatSave;

            foreach (var storeItem in objs.Store.All)
            {
                var toSaveStoreItem = storeItem.Value;
                var saveStateStoreItem = new SaveState.StoreItem();

                saveStateStoreItem.Id = toSaveStoreItem.Item.Id;
                saveStateStoreItem.Quantity = toSaveStoreItem.Item.Quantity;

                saveState.StoreItems.Add(saveStateStoreItem);
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
                if (save.ItemConfigs != null)
                {
                    foreach (var item in save.ItemConfigs)
                    {
                        var toLoadItem = objs.Items.All[item.Id];
                        toLoadItem.IncludeInSellAll = item.Enabled;
                    }
                }
                if (save.Gatherers != null)
                {
                    foreach (var gatherer in save.Gatherers)
                    {
                        var toLoadGatherer = objs.Gatherers.All[gatherer.Id];
                        toLoadGatherer.Quantity = gatherer.Quantity;
                        toLoadGatherer.ResourceBuffer = gatherer.ResourceBuffer;
                    }
                }
                if (save.LastUpdate != null)
                {
                    lastUpdate = save.LastUpdate;
                }
                if (save.Processors != null)
                {
                    foreach (var processor in save.Processors)
                    {
                        var toLoadProcessor = objs.Crafting.Processors[processor.Id];
                        toLoadProcessor.SelectedRecipeIndex = processor.SelectedRecipe;
                        toLoadProcessor.RecipeProgress = processor.Progress;
                        toLoadProcessor.RecipesCrafted = processor.RecipesCrafted;
                        toLoadProcessor.RecipesToCraft = processor.RecipesToCraft;
                    }
                }
                if (save.AntiCheatCoordinates != null)
                {
                    objs.Gatherers.AntiCheatX = save.AntiCheatCoordinates.X;
                    objs.Gatherers.AntiCheatY = save.AntiCheatCoordinates.Y;
                    objs.Gatherers.AntiCheatNextChange = save.AntiCheatCoordinates.NextChange;
                }
                if (save.StoreItems != null)
                {
                    foreach (var storeItem in save.StoreItems)
                    {
                        var toLoadStoreItem = objs.Store.All[storeItem.Id];
                        toLoadStoreItem.Item.Quantity = storeItem.Quantity;
                    }
                }
            }
        }
    }
}
