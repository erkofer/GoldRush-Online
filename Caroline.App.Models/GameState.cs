namespace Caroline.App.Models
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

            // ITEMS
            CompressableHelpers.CompressList(Items, oldState.Items, newState.Items);

            // MESSAGE
            if (_Message != null)
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

            return newState;
        }

        public partial class Item : ICompressable<Item>, IIdentifiableObject
        {
            public Item Compress(Item oldItem)
            {
                if (Quantity != oldItem.Quantity)
                {
                    return new Item { Id = Id, Quantity = Quantity };
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
                    message = new ChatMessage { Text = Text };
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

        public partial class Schematic : ICompressable<Schematic>
        {
            public Schematic Compress(Schematic oldObject)
            {
                var schematic = new Schematic();
                CompressableHelpers.CompressList(_Items, oldObject._Items, schematic._Items);
                CompressableHelpers.CompressList(_StoreItems, oldObject._StoreItems, schematic._StoreItems);
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
        }

        public partial class StoreItem : ICompressable<StoreItem>, IIdentifiableObject
        {
            public StoreItem Compress(StoreItem oldItem)
            {
                if (Quantity != oldItem.Quantity)
                    return new StoreItem { _Id = _Id, Quantity = Quantity };
                return null;
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
    }
}
