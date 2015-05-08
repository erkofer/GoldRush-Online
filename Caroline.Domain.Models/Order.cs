namespace Caroline.Domain.Models
{
    public class Order
    {
        public long OwnerId { get; set; }

        /// <summary>
        /// Whether or not this order is a sell order.
        /// </summary>
        public bool IsSelling { get; set; }

        /// <summary>
        /// The number of items being transacted.
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// The value of each item being sold.
        /// </summary>
        public long UnitValue { get; set; }

        public long ItemId { get; set; }
    }
}
