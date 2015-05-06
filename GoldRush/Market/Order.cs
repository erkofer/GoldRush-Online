using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush.Market
{
    class Order
    {
        public Order(int id, bool selling, int quantity, int unitWorth)
        {
            Id = id;
            Selling = selling;
            Quantity = quantity;
            UnitWorth = unitWorth;
        }

        /// <summary>
        /// The id of the item being sold.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Whether or not this order is a sell order.
        /// </summary>
        public bool Selling { get; set; }

        /// <summary>
        /// The number of items being transacted.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The number of items that have been transacted.
        /// </summary>
        public int FulfilledQuantity { get; set; }
        
        /// <summary>
        /// The number of items left to be transacted.
        /// </summary>
        public int RemainingQuantity { get { return Quantity - FulfilledQuantity; } }

        /// <summary>
        /// The value of each item being sold.
        /// </summary>
        public int UnitWorth { get; set; }
    }
}
