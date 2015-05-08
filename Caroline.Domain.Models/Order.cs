namespace Caroline.Domain.Models
{
    public class Order
    {
        /// <summary>
        /// The id of the item being sold.
        /// </summary>
        public long Id { get; set; }

        public long OwnerId { get; set; }

        /// <summary>
        /// Whether or not this order is a sell order.
        /// </summary>
        public bool Selling { get; set; }

        /// <summary>
        /// The number of items being transacted.
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// The number of items that have been transacted.
        /// </summary>
        public long FulfilledQuantity { get; set; }

        /// <summary>
        /// The value of each item being sold.
        /// </summary>
        public long UnitWorth { get; set; }
    }
}
