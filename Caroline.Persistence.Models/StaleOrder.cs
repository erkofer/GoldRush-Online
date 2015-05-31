using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Caroline.Persistence.Models
{
    public class StaleOrder
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public long ItemId { get; set; }
        public long GameId { get; set; }
        public long UnitValue { get; set; }
        /// <summary>
        /// The total quantity of an item that is being transacted.
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// The quantity left of an item to be transacted.
        /// </summary>
        public long UnfulfilledQuantity { get; set; }
        public bool IsSelling { get; set; }
        public long Version { get; set; }
        public long TotalMoneyRecieved { get; set; }
        /// <summary>
        /// Amount of money that has not been claimed by the owner.
        /// </summary>
        public long UnclaimedMoneyRecieved { get; set; }
        public long TotalItemsRecieved { get; set; }
        /// <summary>
        /// Amount of items that has not been claimed by the owner.
        /// </summary>
        public long UnclaimedItemsRecieved { get; set; }
    }
}
