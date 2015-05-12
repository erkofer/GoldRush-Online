using System.Data.Odbc;

namespace Caroline.Persistence.Models
{
    public partial class GameState : ICompressable<GameState>
    {
        public GameState Compress(GameState oldState)
        {
            // create it eagerly because we are compressing lists
            var newState = new GameState();

            // ISERROR
            if (_IsError != oldState._IsError)
            {
                newState._IsError = _IsError;
            }

            if (_ConnectedUsers != oldState._ConnectedUsers)
            {
                newState._ConnectedUsers = _ConnectedUsers;
            }

            // ITEMS
            CompressableHelpers.CompressList(_Items, oldState._Items, newState._Items);

            // MESSAGE
            //TODO: Messages are a list now. Opps. Shouldn't ever need to be compressed anyways tho.
            /*if (_Message != null)
            {
                var oldMessage = oldState._Message;
                if (oldMessage == null)
                {
                    newState._Message = _Message;
                }
                else
                {
                    var message = _Message.Compress(oldMessage);
                    if (message != null)
                    {
                        newState._Message = message;
                    }
                }
            }*/
            if (_IsRateLimited != oldState._IsRateLimited)
            {
                newState._IsRateLimited = _IsRateLimited;
            }


            if (_AntiCheatCoordinates != null)
            {
                var oldAC = oldState._AntiCheatCoordinates;
                if (oldAC == null)
                {
                    newState._AntiCheatCoordinates = _AntiCheatCoordinates;
                }
                else
                {
                    var ac = _AntiCheatCoordinates.Compress(oldAC);
                    if (ac != null)
                    {
                        newState._AntiCheatCoordinates = ac;
                    }
                }
            }

            // GAMESCHEMA
            if (_GameSchema != null)
            {
                var oldSchema = oldState._GameSchema;
                if (oldSchema == null)
                {
                    newState._GameSchema = _GameSchema;
                }
                else
                {
                    var schema = _GameSchema.Compress(oldSchema);
                    if (schema != null)
                    {
                        newState._GameSchema = schema;
                    }
                }
            }

            // STOREITEMUPDATE
            CompressableHelpers.CompressList(_StoreItemsUpdate,
                oldState._StoreItemsUpdate,
                newState._StoreItemsUpdate);

            // STATITEMSUPDATE
            CompressableHelpers.CompressList(_StatItemsUpdate, oldState._StatItemsUpdate, newState._StatItemsUpdate);

            //CONFIGITEMS
            CompressableHelpers.CompressList(_ConfigItems, oldState._ConfigItems, newState._ConfigItems);

            // PROCESSORS
            CompressableHelpers.CompressList(_Processors, oldState._Processors, newState._Processors);

            CompressableHelpers.CompressList(_Buffs, oldState._Buffs, newState._Buffs);

            CompressableHelpers.CompressList(_Gatherers, oldState._Gatherers, newState._Gatherers);

            CompressableHelpers.CompressList(_Achievements, oldState._Achievements, newState._Achievements);

            return newState;
        }

        public partial class Item : ICompressable<Item>, IIdentifiableObject
        {
            public Item Compress(Item oldItem)
            {
                Item item = null;
                if (_Quantity != oldItem._Quantity)
                {
                    if(item == null)
                        item = new Item { _Id = _Id };
                    item._Quantity = _Quantity;
                }
                if (_Worth != oldItem._Worth)
                {
                    if (item == null)
                        item = new Item { _Id = _Id };
                    item._Worth = _Worth;
                }
                return item;
            }
        }

        public partial class Gatherer : ICompressable<Gatherer>, IIdentifiableObject
        {
            public Gatherer Compress(Gatherer oldItem)
            {
                Gatherer gatherer=null;
                if (_Enabled != oldItem._Enabled)
                {
                    if(gatherer == null)
                        gatherer = new Gatherer(){ _Id = _Id};
                    gatherer._Enabled = _Enabled;
                }
                if (_Efficiency != oldItem._Efficiency)
                {
                    if (gatherer == null)
                        gatherer = new Gatherer() { _Id = _Id };
                    gatherer._Efficiency = _Efficiency;
                }
                if (_FuelConsumed != oldItem._FuelConsumed)
                {
                    if (gatherer == null)
                        gatherer = new Gatherer() { _Id = _Id };
                    gatherer._FuelConsumed = _FuelConsumed;
                }
                if (_RarityBonus != oldItem._RarityBonus)
                {
                    if (gatherer == null)
                        gatherer = new Gatherer() { _Id = _Id };
                    gatherer._RarityBonus = _RarityBonus;
                }
                return gatherer;
            } 
        }


        public partial class AntiCheat : ICompressable<AntiCheat>
        {
            public AntiCheat Compress(AntiCheat oldObject)
            {
                if (oldObject._X != _X || oldObject._Y != _Y)
                {
                    return new AntiCheat() { _X = _X, _Y = _Y };
                }
                return null;
            }
        }

        public partial class ChatMessage : ICompressable<ChatMessage>
        {
            public ChatMessage Compress(ChatMessage oldObject)
            {
                ChatMessage message = null;
                if (oldObject.Text != Text)
                {
                    if (message == null)
                        message = new ChatMessage();
                    message.Text = Text;
                }
                if (oldObject.Sender != Sender)
                {
                    if (message == null)
                        message = new ChatMessage();
                    message.Sender = Sender;
                }
                if (oldObject.Time != Time)
                {
                    if (message == null)
                        message = new ChatMessage();
                    message.Time = Time;
                }
                if (oldObject.Permissions != Permissions)
                {
                    if (message == null)
                        message = new ChatMessage();
                    message.Permissions = Permissions;
                }
                return message;
            }
        }

        public partial class Processor : ICompressable<Processor>, IIdentifiableObject
        {
            public Processor Compress(Processor oldObject)
            {
                Processor processor = null;
                if (oldObject._SelectedRecipe != _SelectedRecipe)
                {
                    if (processor == null)
                        processor = new Processor() { _Id = _Id };
                    processor._SelectedRecipe = _SelectedRecipe;
                }
                if (oldObject._OperationDuration != _OperationDuration)
                {
                    if (processor == null)
                        processor = new Processor() { _Id = _Id };
                    processor._OperationDuration = _OperationDuration;
                }
                if (oldObject._CompletedOperations != _CompletedOperations)
                {
                    if (processor == null)
                        processor = new Processor() { _Id = _Id };
                    processor._CompletedOperations = _CompletedOperations;
                }
                if (oldObject._TotalOperations != _TotalOperations)
                {
                    if (processor == null)
                        processor = new Processor() { _Id = _Id };
                    processor._TotalOperations = _TotalOperations;
                }
                if (oldObject._Capacity != _Capacity)
                {
                    if (processor == null)
                        processor = new Processor() { _Id = _Id };
                    processor._Capacity = _Capacity;
                }
                return processor;
            }
        }

        public partial class Schematic : ICompressable<Schematic>
        {
            public Schematic Compress(Schematic oldObject)
            {
                // create it eagerly because we are compressing lists
                var schematic = new Schematic();
                CompressableHelpers.CompressList(_Items, oldObject._Items, schematic._Items);
                CompressableHelpers.CompressList(_StoreItems, oldObject._StoreItems, schematic._StoreItems);
                CompressableHelpers.CompressList(_Buffs, oldObject._Buffs, schematic._Buffs);
                CompressableHelpers.CompressList(_Achievements, oldObject._Achievements,schematic._Achievements);
                return schematic;
            }

            public partial class SchemaItem : ICompressable<SchemaItem>, IIdentifiableObject
            {
                public SchemaItem Compress(SchemaItem oldObject)
                {
                    SchemaItem schematic = null;
                    if (oldObject._Name != _Name)
                    {
                        if (schematic == null)
                            schematic = new SchemaItem { _Id = _Id };
                        schematic._Name = _Name;
                    }
                    if (oldObject._Worth != _Worth)
                    {
                        if (schematic == null)
                            schematic = new SchemaItem { _Id = _Id };
                        schematic._Worth = _Worth;
                    }
                    if (oldObject._Category != _Category)
                    {
                        if (schematic == null)
                            schematic = new SchemaItem { _Id = _Id };
                        schematic._Category = _Category;
                    }
                    return schematic;
                }
            }

            public partial class SchemaStoreItem : ICompressable<SchemaStoreItem>, IIdentifiableObject
            {
                public SchemaStoreItem Compress(SchemaStoreItem oldObject)
                {
                    SchemaStoreItem schematic = null;
                    if (oldObject._Category != _Category)
                    {
                        if (schematic == null)
                            schematic = new SchemaStoreItem { _Id = _Id };
                        schematic._Category = _Category;
                    }
                    if (oldObject._Name != _Name)
                    {
                        if (schematic == null)
                            schematic = new SchemaStoreItem { _Id = _Id };
                        schematic._Name = _Name;
                    }
                    if (oldObject._Price != _Price)
                    {
                        if (schematic == null)
                            schematic = new SchemaStoreItem { _Id = _Id };
                        schematic._Price = _Price;
                    }
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (oldObject._Factor != _Factor)
                    {
                        if (schematic == null)
                            schematic = new SchemaStoreItem { _Id = _Id };
                        schematic._Factor = _Factor;
                    }
                    if (oldObject._MaxQuantity != _MaxQuantity)
                    {
                        if (schematic == null)
                            schematic = new SchemaStoreItem { _Id = _Id };
                        schematic._MaxQuantity = _MaxQuantity;
                    }
                    return schematic;
                }
            }

            public partial class SchemaBuff : ICompressable<SchemaBuff>, IIdentifiableObject 
            {
                public SchemaBuff Compress(SchemaBuff oldObject)
                {
                    SchemaBuff schema = null;
                    if (oldObject._Name != _Name)
                    {
                        if (schema == null) schema = new SchemaBuff() {_Id = _Id};
                        schema._Name = _Name;
                    }
                    if (oldObject._Duration != _Duration)
                    {
                        if (schema == null) schema = new SchemaBuff() {_Id = _Id};
                        schema._Duration = _Duration;
                    }
                    if (oldObject._Description != _Description)
                    {
                        if (schema == null) schema = new SchemaBuff() {_Id = _Id};
                        schema._Description = _Description;
                    }
                    return schema;
                }
            }

            public partial class SchemaAchievement : ICompressable<SchemaAchievement>, IIdentifiableObject
            {
                public SchemaAchievement Compress(SchemaAchievement oldObject)
                {
                    SchemaAchievement schema = null;
                    if (oldObject._Name != _Name)
                    {
                        if(schema==null) schema = new SchemaAchievement(){_Id = _Id};
                        schema._Name = _Name;
                    }
                    if (oldObject._RequiredId != _RequiredId)
                    {
                        if (schema == null) schema = new SchemaAchievement() { _Id = _Id };
                        schema._RequiredId = _RequiredId;
                    }
                    if (oldObject._Category != _Category)
                    {
                        if (schema == null) schema = new SchemaAchievement() { _Id = _Id };
                        schema._Category = _Category;
                    }
                    if (oldObject._Goal != _Goal)
                    {
                        if (schema == null) schema = new SchemaAchievement() { _Id = _Id };
                        schema._Goal = _Goal;
                    }

                    return schema;
                }
            }
        }

        public partial class StoreItem : ICompressable<StoreItem>, IIdentifiableObject
        {
            public StoreItem Compress(StoreItem oldItem)
            {
                StoreItem storeItem = null;
                if (Quantity != oldItem.Quantity)
                {
                    if (storeItem == null)
                        storeItem = new StoreItem() {_Id = _Id};
                    storeItem._Quantity = _Quantity;
                }
                if (_MaxQuantity != oldItem._MaxQuantity)
                {
                    if (storeItem == null)
                        storeItem = new StoreItem() { _Id = _Id };
                    storeItem._MaxQuantity = _MaxQuantity;
                }
                if (_Price != oldItem._Price)
                {
                    if (storeItem == null)
                        storeItem = new StoreItem() {_Id = _Id};
                    storeItem._Price = _Price;
                }
                return storeItem;
            }
        }

        public partial class StatItem : ICompressable<StatItem>, IIdentifiableObject
        {
            public StatItem Compress(StatItem oldObject)
            {
                StatItem stat = null;
                if (_PrestigeQuantity != oldObject._PrestigeQuantity)
                {
                    if (stat == null)
                        stat = new StatItem { _Id = _Id };
                    stat._PrestigeQuantity = _PrestigeQuantity;
                }
                if (_LifeTimeQuantity != oldObject._LifeTimeQuantity)
                {
                    if (stat == null)
                        stat = new StatItem { _Id = _Id };
                    stat._LifeTimeQuantity = _LifeTimeQuantity;
                }
                return stat;
            }
        }

        public partial class ConfigItem : ICompressable<ConfigItem>, IIdentifiableObject
        {
            public ConfigItem Compress(ConfigItem oldObject)
            {
                if (_Enabled != oldObject._Enabled)
                    return new ConfigItem { _Id = _Id, _Enabled = _Enabled };
                return null;
            }
        }

        public partial class Buff : ICompressable<Buff>, IIdentifiableObject
        {
            public Buff Compress(Buff oldObject)
            {
                if (_TimeActive != oldObject._TimeActive)
                    return new Buff {_Id = _Id, _TimeActive = _TimeActive};
                return null;
            }
        }

        public partial class Achievement : ICompressable<Achievement>, IIdentifiableObject
        {
            public Achievement Compress(Achievement oldObject)
            {
                Achievement achievement = null;
                if (_Progress != oldObject._Progress)
                {
                    if(achievement == null) achievement = new Achievement(){_Id = _Id};
                    achievement._Progress = _Progress;
                }
                return achievement;
            }
        }
    }
}
