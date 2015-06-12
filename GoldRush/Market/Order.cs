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
            // if there is still unfulfilled quantity then we are not cancelled.
            if (order.UnfulfilledQuantity != 0) return false;

            if (order.IsSelling)
            {
                // if we've received less money than we requested.
                var expectedIncome = order.Quantity*order.UnitValue;
                if (order.TotalMoneyRecieved < expectedIncome) return true;
            }
            else
            {
                // if the amount of item we received is less than we requested. it's cancelled.
                if (order.TotalItemsRecieved < order.Quantity) return true;
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
