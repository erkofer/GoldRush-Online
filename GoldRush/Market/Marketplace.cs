using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush.Market
{
    static class Marketplace
    {
        //TODO: save which orders have been editted to allow easy resubmittion.
        //TODO: attach a unique transaction id to each order.

        //TODO: tristyn save this to db pls.
        private static List<Order> orders = new List<Order>(); 

        static Marketplace()
        {
            //TODO: initially scan all offers to ensure 
            //TODO: no outstanding offers can be fulfilled.
        }

        static void Buy(int id, int quantity, int unitWorth)
        {
            SubmitOrder(id, false, quantity, unitWorth);
        }

        static void Sell(int id, int quantity, int unitWorth)
        {
            SubmitOrder(id, true, quantity, unitWorth);
        }

        private static void SubmitOrder(int id, bool selling, int quantity, int unitWorth)
        {
            var order = new Order(id, selling, quantity, unitWorth);
            AddNewOrder(order);
            ProcessOrders(order);
        }

        private static void AddNewOrder(Order order)
        {
            orders.Add(order);
        }

        private static IEnumerable<Order> GetOrders()
        {
            return orders;
        }
        //TODO: finish this.
        private static void TransactOrders(Order a, Order b)
        {
            var sellOrder = a.Selling ? a : b;
            var buyOrder = a.Selling ? b : a;

            var quantityToBuy = Math.Min(sellOrder.RemainingQuantity, buyOrder.RemainingQuantity);
            sellOrder.FulfilledQuantity += quantityToBuy;
            buyOrder.FulfilledQuantity += quantityToBuy;
        }
        //TODO: finish this.
        private static void ProcessOrders(Order order)
        {
            var id = order.Id;
            var quantity = order.Quantity;
            var unitWorth = order.UnitWorth;
            var selling = order.Selling;
            var alikeOrders = new List<Order>();
            /*Select all orders with specified Id.
             For decreased search times store all
             orders with an Id in their own column?*/
            var loadedOrders = GetOrders();
            foreach (var selectedOrder in loadedOrders)
            {
                /*If this order is dealing in our items and they are
                 interested in our order save.*/
                if (selectedOrder.Id == id && selectedOrder.Selling == !selling)
                    alikeOrders.Add(selectedOrder);
            }

            if (selling)
            {
                // sort the alike orders in descending value
                alikeOrders.Sort((a,b)=>a.UnitWorth.CompareTo(b.UnitWorth));
                // iterate through orders buying our items.
                foreach (var selectedOrder in alikeOrders)
                {
                    // if this order's offer is greater than our asking price.
                    if (selectedOrder.UnitWorth >= unitWorth)
                    {
                        
                    }
                }
            }
           
        }

    }
}
