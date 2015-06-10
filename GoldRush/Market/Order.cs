using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace GoldRush.Market
{
    public static class OrderExtension
    {
        public static bool IsEmpty(this StaleOrder order)
        {
            if (order.UnfulfilledQuantity > 0) return false;
            if (order.UnclaimedItemsRecieved > 0) return false;
            if (order.UnclaimedMoneyRecieved > 0) return false;

            return true;
        }

        public static bool IsCanceled(this StaleOrder order)
        {
            if (order.TotalItemsRecieved > 0 && order.TotalMoneyRecieved > 0) return true;
            if (order.UnfulfilledQuantity != 0) return false;
            if (order.IsSelling)
            {
                if (order.Quantity != order.TotalMoneyRecieved/order.UnitValue) return true;
            }
            else
            {
                if (order.Quantity != order.TotalItemsRecieved) return true;
            }
            return false;
        }
    }

    public class SaveOrder
    {
        public string Id;
        public int Position;
    }
}
