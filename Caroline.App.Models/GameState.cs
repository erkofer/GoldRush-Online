using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caroline.App.Models
{
    public partial class GameState
    {
        public partial class Item
        {
            public Item Compress(Item oldItem)
            {
                Item newItem = null;
                if (this.Quantity != oldItem.Quantity) 
                {
                    if (newItem == null) 
                    { 
                        newItem = new Item();
                        newItem.Id = this.Id;
                    }
                    newItem.Quantity = this.Quantity;
                }
                return newItem;
            }
        }

        public partial class StoreItem
        {
            public StoreItem Compress(StoreItem oldItem)
            {
                StoreItem newItem = null;
                if(this.Quantity !=oldItem.Quantity)
                {
                    if(newItem == null)
                    {
                        newItem = new StoreItem();
                        newItem.Id = this.Id;
                    }
                    newItem.Quantity = this.Quantity;
                }
                return newItem;
            }
        }

        public GameState Compress(GameState oldState)
        {
            GameState newState = null;
            //var itemMat= oldState.Items.GroupBy(item => item.Id)
            // INVENTORY
            foreach (var item in this.Items) // Switch these loops around? If an item is not contained in the old state then it does not need to be compressed.
            {
                foreach (var oldItem in oldState.Items)
                {
                    if (item.Id != oldItem.Id) continue;
                    var newItem = item.Compress(oldItem);
                    if (newItem == null) continue;
                    if (newState == null) 
                        newState = new GameState();
                    newState.Items.Add(newItem);
                }
            }

            // STORE
            foreach (var item in this.StoreItemsUpdate)
            {
                foreach (var oldItem in oldState.StoreItemsUpdate)
                {
                    if (item.Id != oldItem.Id) continue;
                    var newItem = item.Compress(oldItem);
                    if (newItem == null) continue;
                    if (newState == null) newState = new GameState();
                    newState.StoreItemsUpdate.Add(newItem);
                }
            }

            return newState;
        }
    }
}
