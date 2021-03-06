﻿namespace Caroline.Domain.Models
{
    public class FreshOrder
    {
        public long GameId { get; set; }

        public long ItemId { get; set; }

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
    }
}
