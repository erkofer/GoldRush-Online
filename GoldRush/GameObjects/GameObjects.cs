using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class GameObjects
    {
        public GameObjects()
        {
            Random = new Random();
            Items = new Items(this);
            Gatherers = new Gatherers(this);
            Upgrades = new Upgrades(this);
            Store = new Store(this);
            // Add all gameobjects to a big dictionary.
            All = new Dictionary<int,GameObject>();
            foreach (var item in Items.All) { All.Add(item.Key, item.Value); }
            foreach (var gatherer in Gatherers.All) { All.Add(gatherer.Key, gatherer.Value); }
        }

        

        public Random Random;
        public Items Items;
        public Upgrades Upgrades;
        public Gatherers Gatherers;
        public Store Store;
        public Dictionary<int,GameObject> All;

        public void Update(int ms)
        {
            Gatherers.Update(ms);
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
            public virtual int Quantity { get; set; }

            public virtual bool Active
            {
                get { return Quantity > 0; }
                set
                {
                    if (value)
                        if (Quantity <= 0) Quantity = 1;
                    else
                        if (Quantity > 0) Quantity = 0;
                }
            }
        }

    }
}
