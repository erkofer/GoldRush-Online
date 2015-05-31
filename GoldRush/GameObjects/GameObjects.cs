using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace GoldRush
{
    class GameObjects
    {
        public GameObjects()
        {
            Random = new Random();
            Statistics = new Statistics();
            Items = new Items(this);
            Gatherers = new Gatherers(this);
            Processing = new Processing(this);
            Upgrades = new Upgrades(this);
            Crafting = new Crafting(this);
            Store = new Store(this);
            Achievements = new Achievements(this);
            // Add all gameobjects to a big dictionary.
            All = new Dictionary<int,GameObject>();
            foreach (var item in Items.All) { All.Add(item.Key, item.Value); }
            foreach (var gatherer in Gatherers.All) { All.Add(gatherer.Key, gatherer.Value); }
            foreach (var upgrade in Upgrades.All) { All.Add(upgrade.Key, upgrade.Value); }
        }

        public Random Random;
        public Statistics Statistics;
        public Items Items;
        //GameId = 200
        public Upgrades Upgrades;
        //GameId =400
        public Gatherers Gatherers;
        public Store Store;
        public Crafting Crafting;
        public Processing Processing;
        public Achievements Achievements;
        public Dictionary<int,GameObject> All;
        public User User;
        public long UserId;
        public void Update(long seconds)
        {
            Upgrades.Update(seconds);
            Gatherers.Update(seconds);
            Processing.Update(seconds);
        }

        internal abstract class GameObject
        {
            protected GameObject()
            {
                
            }

            private readonly GameConfig.Config _config;

            protected GameObject(GameConfig.Config config)
            {
                _config = config;
                Quantity = 0;
            }

            /// <summary>
            /// Unique identifier used for interfacing with clients.
            /// </summary>
            public int Id { get { return _config.Id;} }

            /// <summary>
            /// The prerequisite for this GameObject.
            /// </summary>
            public GameObject Requires { get; set; }

            /// <summary>
            /// A user facing identifier.
            /// </summary>
            public virtual string Name { get { return _config.Name; } }
            public virtual long Quantity { get; set; }
            public virtual bool Active { get; set; }

            public virtual string Tooltip { get { return "undefined"; } }
        }
    }
}
