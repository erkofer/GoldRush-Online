using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Caroline.App.Models;
using Caroline.Domain.Models;
using GoldRush.Market;
using MongoDB.Bson;

namespace GoldRush
{
    internal class Game
    {
        private enum Tab
        {
            Inventory = 1,
            Statistics = 2,
            Equipment = 3,
            Store = 4,
            Crafting = 5,
            Achievements = 6,
            Market = 7
        }

        public Game()
        {
            objs = new GameObjects();
        }

        public GameObjects objs;
        public long LastUpdate;
        public long Score;

        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long) timeSpan.TotalSeconds;
        }


        public async Task<GameState> Update(ClientActions message, IMarketPlace market, User user)
        {
            objs.Items.MarketPlace = market;
            objs.Items.User = user;

            var currentTime = UnixTimeNow();
            if (LastUpdate == 0) LastUpdate = currentTime;

            var timeSinceLastUpdate = currentTime - LastUpdate;
            if (timeSinceLastUpdate > 0)
            {
                LastUpdate = currentTime;
                await objs.Update(timeSinceLastUpdate);
            }
            objs.Statistics.TimePlayed.Value += timeSinceLastUpdate;

            // CLIENT ACTIONS
            await InterpretClientActions(message);

            var state = new GameState();
            state.GameSchema = GenerateSchema();
            GetGameData(state);
            return state;
        }

        #region Update Helper Methods

        #region Client Actions Interpretation

        private async Task InterpretClientActions(ClientActions message)
        {
            if (message == null) return;
            InterpretInventoryActions(message);
            InterpretStoreActions(message);
            InterpretCraftingActions(message);
            InterpretProcessingActions(message);
            InterpretMiningActions(message);
            InterpretPotionActions(message);
            InterpretGathererActions(message);
            await InterpretOrderActions(message);
        }

        private async Task InterpretOrderActions(ClientActions message)
        {
            if (message.RequestOrders == true) await objs.Items.GetOrders();

            for (var i = 0; i < message.Orders.Count; i++)
            {
                var order = message.Orders[i];
                await
                    objs.Items.PlaceOrder(order.Position, order.ItemId, order.ItemQuantity, order.ItemValue,
                        order.IsSelling);
            }

            for (var i = 0; i < message.Claims.Count; i++)
            {
                var claim = message.Claims[i];
                await objs.Items.ClaimOrder(claim.Slot, claim.Coins ? ClaimField.Money : ClaimField.Items);
            }

            for (var i = 0; i < message.Cancels.Count; i++)
            {
                var cancel = message.Cancels[i];
                await objs.Items.CancelOrder(cancel.Slot);
            }
        }

        private void InterpretGathererActions(ClientActions message)
        {
            if (message.GathererActions.Count <= 0) return;
            for (var i = 0; i < message.GathererActions.Count; i++)
            {
                GoldRush.Gatherers.Gatherer gatherer;
                if (objs.Gatherers.All.TryGetValue(message.GathererActions[i].Id, out gatherer))
                    gatherer.Enabled = message.GathererActions[i].Enabled;
            }
        }

        private void InterpretPotionActions(ClientActions message)
        {
            if (message.PotionActions.Count <= 0) return;
            for (var i = 0; i < message.PotionActions.Count; i++)
            {
                objs.Items.Drink(message.PotionActions[i].Id);
            }
        }

        private void InterpretMiningActions(ClientActions message)
        {
            if (message.MiningActions.Count <= 0) return;
            for (var i = 0; i < message.MiningActions.Count; i++)
            {
                objs.Gatherers.Mine(message.MiningActions[i].X, message.MiningActions[i].Y);
            }
        }

        private void InterpretProcessingActions(ClientActions message)
        {
            if (message.ProcessingActions.Count <= 0) return;
            for (var i = 0; i < message.ProcessingActions.Count; i++)
            {
                objs.Processing.Process(message.ProcessingActions[i].Id,
                    message.ProcessingActions[i].RecipeIndex, message.ProcessingActions[i].Iterations);
            }
        }

        private void InterpretCraftingActions(ClientActions message)
        {
            if (message.CraftingActions.Count <= 0) return;
            for (var i = 0; i < message.CraftingActions.Count; i++)
            {
                objs.Crafting.Craft(message.CraftingActions[i].Id, message.CraftingActions[i].Quantity);
            }
        }

        private void InterpretStoreActions(ClientActions message)
        {
            if (message.StoreActions.Count <= 0) return;
            for (var i = 0; i < message.StoreActions.Count; i++)
            {
                if (message.StoreActions[i].Purchase == null) continue;
                var item = objs.Store.All[message.StoreActions[i].Purchase.Id];
                item.Purchase(message.StoreActions[i].Purchase.Quantity);
            }
        }

        private void InterpretInventoryActions(ClientActions message)
        {
            if (message.InventoryActions.Count <= 0) return;
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
                    item.IncludeInSellAll = msg.Config.Enabled;
                }
                if (msg.SellAll == true)
                {
                    objs.Items.SellAll();
                }
            }
        }

        #endregion

        #region Game State Writing

        #region Schema

        private GameState.Schematic GenerateSchema()
        {
            var schema = new GameState.Schematic();
            // INVENTORY
            populateInventorySchema(schema);
            // STORE
            populateStoreSchema(schema);
            // Crafting
            populateCraftingSchema(schema);
            // Processing
            populateProcessingSchema(schema);
            populateBuffSchema(schema);
            populateAchievementSchema(schema);
            return schema;
        }

        private void populateAchievementSchema(GameState.Schematic schema)
        {
            foreach (var achievementPair in objs.Achievements.All)
            {
                var achievement = achievementPair.Value;
                var achievementSchema = new GameState.Schematic.SchemaAchievement();

                achievementSchema.Id = achievement.Id;
                achievementSchema.RequiredId = achievement.Requires != null ? achievement.Requires.Id : 0;
                achievementSchema.Name = achievement.Name;
                achievementSchema.Category = (GameState.Schematic.SchemaAchievement.Section) achievement.Type;
                achievementSchema.Goal = achievement.Goal;

                schema.Achievements.Add(achievementSchema);
            }
        }

        private void populateBuffSchema(GameState.Schematic schema)
        {
            foreach (var buffPair in objs.Upgrades.Buffs)
            {
                var buff = buffPair.Value;
                var schemaBuff = new GameState.Schematic.SchemaBuff();
                schemaBuff.Id = buff.Id;
                schemaBuff.Name = buff.Name;
                schemaBuff.Duration = buff.Duration;
                schemaBuff.Description = buff.Tooltip;

                schema.Buffs.Add(schemaBuff);
            }
        }

        private void populateProcessingSchema(GameState.Schematic schema)
        {
            foreach (var processor in objs.Processing.Processors)
            {
                var schemaItem = new GameState.Schematic.SchemaProcessor();
                schemaItem.Id = processor.Value.Id;
                schemaItem.Name = processor.Value.Name;
                schemaItem.RequiredId = processor.Value.Requires != null ? processor.Value.Requires.Id : 0;

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
        }

        private void populateCraftingSchema(GameState.Schematic schema)
        {
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
        }

        private void populateStoreSchema(GameState.Schematic schema)
        {
            foreach (var item in objs.Store.All)
            {
                var schemaItem = new GameState.Schematic.SchemaStoreItem();
                schemaItem.Id = item.Value.Item.Id;
                schemaItem.Name = item.Value.Item.Name;
                schemaItem.Price = item.Value.BasePrice;
                schemaItem.Factor = item.Value.Factor;
                schemaItem.MaxQuantity = item.Value.MaxQuantity;
                schemaItem.Category = (GameState.Schematic.SchemaStoreItem.Section) item.Value.Category;
                schemaItem.Tooltip = item.Value.Item.Tooltip;
                schemaItem.RequiredId = item.Value.Item.Requires != null ? item.Value.Item.Requires.Id : 0;

                schema.StoreItems.Add(schemaItem);
            }
        }

        private void populateInventorySchema(GameState.Schematic schema)
        {
            foreach (var item in objs.Items.All)
            {
                var schemaItem = new GameState.Schematic.SchemaItem();
                schemaItem.Id = item.Value.Id;
                schemaItem.Name = item.Value.Name;
                schemaItem.Worth = item.Value.Worth;
                schemaItem.Category = (GameState.Schematic.SchemaItem.Section) item.Value.Category;
                schema.Items.Add(schemaItem);
            }
        }

        #endregion

        private void GetGameData(GameState state)
        {
            WriteInventoryAndStatsData(state);
            WriteStoreData(state);
            WriteProcessorData(state);
            WriteAntiCheatData(state);
            WriteBuffData(state);
            WriteGathererData(state);
            WriteAchievementData(state);
            WriteMarketData(state); 
            WriteNotificationData(state);
            WriteRecordData(state);
        } 

        private void WriteRecordData(GameState state)
        {
            if (objs.OfflineRecord == null) return;

            state.OfflineRecord = new GameState.ProgressRecord();
            state.OfflineRecord.SecondsGone = objs.OfflineRecord.FormattedTimeGone;
            foreach (var item in objs.OfflineRecord.Items)
            {
                state.OfflineRecord.Items.Add(new GameState.ProgressRecord.ProgressItem()
                {
                    Id = item.Key,
                    Change = item.Value.Change
                });
            }
        }

        private void WriteNotificationData(GameState state)
        {
            foreach (var notification in objs.Notifications)
            {
                state.Notifications.Add(new GameState.Notification
                {
                    Message = notification.Message,
                    Tag = notification.Tag
                });
            }
            state.CurrentTutorial = objs.Tutorial.GetActiveTutorialTitle();
        }

        private void WriteMarketData(GameState state)
        {
            state.OrdersSent = objs.Items.SentOrders;

            foreach (var order in objs.Items.Orders)
            {
                var stateOrder = new GameState.Order();
                stateOrder.Id = order.Id.ToString();
                stateOrder.UnclaimedItems = order.UnclaimedItemsRecieved;
                stateOrder.UnclaimedCoins = order.UnclaimedMoneyRecieved;
                stateOrder.UnfulfilledQuantity = order.UnfulfilledQuantity;
                stateOrder.UnitValue = order.UnitValue;
                stateOrder.Quantity = order.Quantity;
                stateOrder.ItemId = order.ItemId;
                stateOrder.IsSelling = order.IsSelling;
                stateOrder.IsCanceled = order.IsCanceled();
                for (var i = 0; i < objs.Items.SavedOrders.Count; i++)
                {
                    var savedOrder = objs.Items.SavedOrders[i];
                    if (stateOrder.Id != savedOrder.Id) continue;

                    stateOrder.Slot = savedOrder.Position;
                    break;
                }
                state.Orders.Add(stateOrder);
            }
        }

        private void WriteAchievementData(GameState state)
        {
            foreach (var achievementPair in objs.Achievements.All)
            {
                var achievement = achievementPair.Value;
                var stateAchievement = new GameState.Achievement();

                stateAchievement.Id = achievement.Id;
                stateAchievement.Progress = achievement.Progress;

                // if it requires nothing or it's prerequisite is unlocked.
                if (achievement.Active) state.Achievements.Add(stateAchievement);
            }
        }

        private void WriteGathererData(GameState state)
        {
            foreach (var gathererPair in objs.Gatherers.All)
            {
                var gatherer = gathererPair.Value;
                var stateGatherer = new GameState.Gatherer();

                stateGatherer.Id = gatherer.Id;
                stateGatherer.Enabled = gatherer.Enabled;
                stateGatherer.FuelConsumed = gatherer.FuelConsumption;
                stateGatherer.Efficiency = gatherer.ResourcesPerSecond;
                stateGatherer.RarityBonus = gatherer.ProbabilityModifier;
                state.Gatherers.Add(stateGatherer);
            }
        }

        private void WriteBuffData(GameState state)
        {
            foreach (var buffPair in objs.Upgrades.Buffs)
            {
                var stateBuff = new GameState.Buff();
                var buff = buffPair.Value;

                stateBuff.Id = buff.Id;
                stateBuff.TimeActive = buff.TimeActive;
                state.Buffs.Add(stateBuff);
            }
        }

        private void WriteAntiCheatData(GameState state)
        {
            var stateAntiCheat = new GameState.AntiCheat();
            stateAntiCheat.X = objs.Gatherers.AntiCheatX;
            stateAntiCheat.Y = objs.Gatherers.AntiCheatY;

            state.AntiCheatCoordinates = stateAntiCheat;
        }

        private void WriteProcessorData(GameState state)
        {
            foreach (var processor in objs.Processing.Processors)
            {
                var stateProcessor = new GameState.Processor();
                stateProcessor.Id = processor.Value.Id;
                stateProcessor.SelectedRecipe = processor.Value.SelectedRecipeIndex;
                stateProcessor.OperationDuration = (int) processor.Value.SelectedRecipeDuration;
                //stateProcessor.OperationCompletion = currentTime + (long)processor.Value.RemainingOperationTime;
                stateProcessor.CompletedOperations = processor.Value.RecipesCrafted;
                stateProcessor.TotalOperations = processor.Value.RecipesToCraft;
                stateProcessor.Capacity = processor.Value.Capacity;
                state.Processors.Add(stateProcessor);
            }
        }

        private void WriteStoreData(GameState state)
        {
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

                // if the gameobject requires something and is not active.
                if (gameobject.Requires != null && gameobject.Requires.Active == false)
                    stateStoreItem.Quantity = -1;

                state.StoreItemsUpdate.Add(stateStoreItem);
            }
        }

        private void WriteInventoryAndStatsData(GameState state)
        {
            foreach (var item in objs.Items.All)
            {
                var stateItem = new GameState.Item();
                stateItem.Id = item.Value.Id;
                stateItem.Quantity = item.Value.Quantity;
                stateItem.Worth = item.Value.Value; // confirm issues with speech potion.


                var stateStatsItem = new GameState.StatItem();
                stateStatsItem.Id = item.Value.Id;
                stateStatsItem.PrestigeQuantity = item.Value.PrestigeTimeTotal;
                stateStatsItem.LifeTimeQuantity = item.Value.LifeTimeTotal;

                state.StatItemsUpdate.Add(stateStatsItem);
                state.Items.Add(stateItem);

                if (item.Value.Category == Items.Category.NOTFORSALE) continue;

                var configItem = new GameState.ConfigItem();
                configItem.Id = item.Value.Id;
                configItem.Enabled = item.Value.IncludeInSellAll;
                state.ConfigItems.Add(configItem);
            }
        }
    

        #endregion

        #endregion

        public SaveState Save()
        {
            var saveState = new SaveState();

            // Save last update time
            saveState.LastUpdate = LastUpdate;

            SaveInventoryData(saveState);
            SaveGathererData(saveState);
            SaveProcessorData(saveState);
            SaveAntiCheatData(saveState);
            SaveStoreData(saveState);
            SaveBuffData(saveState);
            SaveStatisticData(saveState);
            SaveMarketData(saveState);

            return saveState;
        }

        #region Save Helpers
        private void SaveMarketData(SaveState saveState)
        {
            foreach (var saveOrder in objs.Items.SavedOrders)
            {
                var save = new SaveState.Order();
                save.Id = saveOrder.Id;
                save.Position = saveOrder.Position;

                saveState.Orders.Add(save);
            }
            saveState.TimeSinceMarketUpdate = objs.Items.SecondsSinceMarketUpdate;
        }

        private void SaveStatisticData(SaveState saveState)
        {
            foreach (var statPair in objs.Statistics.All)
            {
                var stat = statPair.Value;
                var stateSave = new SaveState.Statistic();
                stateSave.Id = stat.Id;
                stateSave.Value = stat.Value;

                saveState.Statistics.Add(stateSave);
            }
        }

        private void SaveBuffData(SaveState saveState)
        {
            foreach (var buff in objs.Upgrades.Buffs)
            {
                var toSaveBuff = buff.Value;
                var saveStateBuff = new SaveState.Buff();
                saveStateBuff.Id = toSaveBuff.Id;
                saveStateBuff.TimeActive = toSaveBuff.TimeActive;

                saveState.Buffs.Add(saveStateBuff);
            }
        }

        private void SaveStoreData(SaveState saveState)
        {
            foreach (var storeItem in objs.Store.All)
            {
                var toSaveStoreItem = storeItem.Value;
                var saveStateStoreItem = new SaveState.StoreItem();

                saveStateStoreItem.Id = toSaveStoreItem.Item.Id;
                saveStateStoreItem.Quantity = toSaveStoreItem.Item.Quantity;

                saveState.StoreItems.Add(saveStateStoreItem);
            }
        }

        private void SaveAntiCheatData(SaveState saveState)
        {
            var anticheatSave = new SaveState.AntiCheat();
            anticheatSave.X = objs.Gatherers.AntiCheatX;
            anticheatSave.Y = objs.Gatherers.AntiCheatY;
            anticheatSave.NextChange = objs.Gatherers.AntiCheatNextChange;

            saveState.AntiCheatCoordinates = anticheatSave;
        }

        private void SaveProcessorData(SaveState saveState)
        {
            foreach (var processor in objs.Processing.Processors)
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
        }

        private void SaveGathererData(SaveState saveState)
        {
            foreach (var gatherer in objs.Gatherers.All)
            {
                var toSaveGatherer = gatherer.Value;
                var saveStateGatherer = new SaveState.Gatherer();

                saveStateGatherer.Id = toSaveGatherer.Id;
                saveStateGatherer.Quantity = toSaveGatherer.Quantity;
                saveStateGatherer.ResourceBuffer = toSaveGatherer.ResourceBuffer;
                saveStateGatherer.Enabled = toSaveGatherer.Enabled;

                saveState.Gatherers.Add(saveStateGatherer);
            }
        }

        private void SaveInventoryData(SaveState saveState)
        {
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
        }


        #endregion

        public void Load(SaveState save)
        {
            if (save != null) LoadData(save);
        }

        #region Load Helpers

        private void LoadData(SaveState save)
        {
            if (save.Items != null) LoadItemData(save);
            if (save.ItemConfigs != null) LoadItemConfigData(save);
            if (save.Gatherers != null) LoadGathererData(save);
            if (save.LastUpdate != null) 
                if (save.LastUpdate != null) LastUpdate = save.LastUpdate;
            if (save.Processors != null) LoadProcessorData(save);
            if (save.AntiCheatCoordinates != null) LoadAntiCheatData(save);
            if (save.StoreItems != null) LoadStoreData(save);
            if (save.Buffs != null) LoadBuffData(save);
            if (save.Statistics != null)LoadStatisticData(save);
            if (save.Orders != null) LoadMarketData(save);

            LoadLeaderboardData();
        }

        private void LoadMarketData(SaveState save)
        {
            foreach (var saveOrder in save.Orders)
            {
                var toLoadOrder = new SaveOrder();
                toLoadOrder.Id = saveOrder.Id;
                toLoadOrder.Position = saveOrder.Position;

                objs.Items.SavedOrders.Add(toLoadOrder);
            }
            if (save.TimeSinceMarketUpdate != null) objs.Items.SecondsSinceMarketUpdate = save.TimeSinceMarketUpdate;
        }

        private void LoadLeaderboardData()
        {
            Score = objs.Items.Coins.LifeTimeTotal;
        }

        private void LoadStatisticData(SaveState save)
        {
            foreach (var stat in save.Statistics)
            {
                var toLoadStat = objs.Statistics.All[stat.Id];
                toLoadStat.Value = stat.Value;
            }
        }

        private void LoadBuffData(SaveState save)
        {
            foreach (var buff in save.Buffs)
            {
                var toLoadBuff = objs.Upgrades.Buffs[buff.Id];
                toLoadBuff.TimeActive = buff.TimeActive;
            }
        }

        private void LoadStoreData(SaveState save)
        {
            foreach (var storeItem in save.StoreItems)
            {
                var toLoadStoreItem = objs.Store.All[storeItem.Id];
                toLoadStoreItem.Item.Quantity = storeItem.Quantity;
            }
        }

        private void LoadAntiCheatData(SaveState save)
        {
            objs.Gatherers.AntiCheatX = save.AntiCheatCoordinates.X;
            objs.Gatherers.AntiCheatY = save.AntiCheatCoordinates.Y;
            objs.Gatherers.AntiCheatNextChange = save.AntiCheatCoordinates.NextChange;
        }

        private void LoadProcessorData(SaveState save)
        {
            foreach (var processor in save.Processors)
            {
                var toLoadProcessor = objs.Processing.Processors[processor.Id];
                if (processor.SelectedRecipe != null) toLoadProcessor.SelectedRecipeIndex = processor.SelectedRecipe;
                if (processor.Progress != null) toLoadProcessor.RecipeProgress = processor.Progress;
                if (processor.RecipesCrafted != null) toLoadProcessor.RecipesCrafted = processor.RecipesCrafted;
                if (processor.RecipesToCraft != null) toLoadProcessor.RecipesToCraft = processor.RecipesToCraft;
            }
        }

        private void LoadGathererData(SaveState save)
        {
            foreach (var gatherer in save.Gatherers)
            {
                var toLoadGatherer = objs.Gatherers.All[gatherer.Id];
                if (gatherer.Quantity != null) toLoadGatherer.Quantity = gatherer.Quantity;
                if (gatherer.ResourceBuffer != null) toLoadGatherer.ResourceBuffer = gatherer.ResourceBuffer;
                if (gatherer.Enabled != null) toLoadGatherer.Enabled = gatherer.Enabled;
            }
        }

        private void LoadItemConfigData(SaveState save)
        {
            foreach (var item in save.ItemConfigs)
            {
                var toLoadItem = objs.Items.All[item.Id];
                if (item.Enabled != null) toLoadItem.IncludeInSellAll = item.Enabled;
            }
        }

        private void LoadItemData(SaveState save)
        {
            foreach (var item in save.Items)
            {
                var toLoadItem = objs.Items.All[item.Id];
                if (item.Quantity != null) toLoadItem.Quantity = item.Quantity;
                if (item.PrestigeQuantity != null) toLoadItem.PrestigeTimeTotal = item.PrestigeQuantity;
                if (item.AlltimeQuantity != null) toLoadItem.LifeTimeTotal = item.AlltimeQuantity;
            }
        }

        #endregion
    }
}
