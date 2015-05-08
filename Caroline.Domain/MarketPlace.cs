using System;
using Caroline.Domain.Models;

namespace Caroline.Domain
{
    public class MarketPlace
    {// Order{long Id; long UserId; long Quantity; bool Selling; long UnitValue;}
        void Sell(Order order)
        {
            while (order.Quantity > 0)
            {

                var buyOrder = PopHighestValueBuyOrderGreaterThan(order.Id, order.UnitWorth);
                if (buyOrder == null)
                    break;

                var unitsTransacted = Math.Min(buyOrder.Quantity, order.Quantity);
                order.Quantity -= unitsTransacted;

                buyOrder.Quantity -= unitsTransacted;

                // delete or update buy order
                if (buyOrder.Quantity == 0)
                    // remove buy order from active marketplace
                    DeleteOrder(buyOrder.Id);
                else
                    UpdateOrder(buyOrder);

                // stale order advantage, give buyer the difference in price
                var differenceToRefund = (buyOrder.UnitWorth - order.UnitWorth) * unitsTransacted;
                GiveBuyerMoney(differenceToRefund);

            }

            // update sell order
            if (order.Quantity == 0)
                ; // delete order.. but we never commited it to the db so what ever
            else
                UpdateOrder(order);
        }

        Order PopHighestValueBuyOrderGreaterThan(long itemId, long minValue, ComparisonPredicate predicate)
        {
            throw new NotImplementedException();
        }
    }
}
