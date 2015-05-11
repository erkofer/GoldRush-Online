namespace Caroline.Persistence.Models
{
    public class StaleOrder
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public long GameId { get; set; }
        public long UnitValue { get; set; }
        public long Quantity { get; set; }
        public long UnfulfilledQuantity { get; set; }
        public bool IsSelling { get; set; }
        public long Version { get; set; }
        public long TotalMoneyRecieved { get; set; }
        public long UnclaimedMoneyRecieved { get; set; }
        public long TotalItemsRecieved { get; set; }
        public long UnclaimedItemsRecieved { get; set; }
    }
}
