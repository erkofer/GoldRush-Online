namespace Caroline.Persistence.Models
{
    public class StaleOrder
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public long UnitValue { get; set; }
        public long Quantity { get; set; }
        public long UnfulfilledQuantity { get; set; }
        public bool IsSelling { get; set; }
        public long Version { get; set; }
  }
}
