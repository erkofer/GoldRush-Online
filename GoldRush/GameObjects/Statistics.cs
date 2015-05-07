using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Statistics
    {
        public Statistics()
        {
            /* IMPORTANT
             * To ensure backward compatibility do not remove
             * statistics that are no longer used.
             */
            TimePlayed = new Statistic(GameConfig.Statistics.TimePlayed);
            All.Add(TimePlayed.Id,TimePlayed);

            RockClicked = new Statistic(GameConfig.Statistics.RockClicked);
            All.Add(RockClicked.Id,RockClicked);
        }

        public Dictionary<int, Statistic> All = new Dictionary<int, Statistic>(); 

        public Statistic TimePlayed;
        public Statistic RockClicked;

        internal class Statistic : GameObjects.GameObject
        {
            public Statistic(GameConfig.Config config)
                :base(config)
            {
                
            }

            public long Value;
        }
    }
}
