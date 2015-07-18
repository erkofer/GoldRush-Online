using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace GoldRush
{
    class Rewards
    {
        public Rewards(GameObjects objs)
        {
            BronzeChest = new Chest();
            var bronzeLoot = new List<Loot>
            {
                new Loot
                {
                    Chance = 1000,
                    Item = objs.Items.Copper,
                    Quantity = 1000
                },
                new Loot
                {
                    Chance = 500,
                    Item = objs.Items.Silver,
                    Quantity = 250
                },
                new Loot
                {
                    Chance = 250,
                    Item = objs.Items.Gold,
                    Quantity = 100
                },
                new Loot
                {
                    Chance = 50,
                    Item = objs.Items.Titanium,
                    Quantity = 3
                },
                new Loot
                {
                    Chance = 10,
                    Item = objs.Items.Diamond,
                    Quantity = 1
                }
            };
            BronzeChest.PossibleLoot = bronzeLoot;
        }

        private Chest BronzeChest;
    }

    class Chest
    {
        static Random random = new Random();
        public List<Loot> PossibleLoot { get; set; }

        public Loot Open()
        {
            var totalChance = 0;
            
            foreach (var loot in PossibleLoot)
                totalChance += loot.Chance;

            var luck = random.Next(0, totalChance);

            var chance = 0;
            foreach (var loot in PossibleLoot)
            {
                chance += loot.Chance;
                if (luck < chance)
                    return loot;
            }

            return new Loot(){Quantity = 0};
        }
    }

    class Loot
    {
        public Items.Item Item { get; set; }
        public int Quantity { get; set; }
        public int Chance { get; set; }
    }
}
