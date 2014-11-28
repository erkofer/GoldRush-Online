using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    public class Gatherers
    {
        public GameObjects Game;

        public Gatherers(GameObjects game)
        {
            Game = game;
            var baseResources = new[]
            {
                game.Items.Stone, game.Items.Copper, game.Items.Iron, game.Items.Silver,
                game.Items.Gold, game.Items.Opal, game.Items.Jade, game.Items.Topaz
            };

            Miner = new Gatherer(game);
            Miner.PossibleResources.AddRange(baseResources);
            Miner.BaseResourcesPerSecond = 0.5;

            Pumpjack = new Gatherer(game);
            Pumpjack.GuaranteedResources.Add(game.Items.Oil);
            Pumpjack.BaseResourcesPerSecond = 0.5;
            
        }
        
       

        public Gatherer Miner;
        public Gatherer Pumpjack;

        public class Gatherer : GameObjects.GameObject
        {
            public Gatherer(GameObjects game)
            {
                PossibleResources = new List<Items.Resource>();
                GuaranteedResources = new List<Items.Resource>();
                ResourcesPerSecondEfficiency = 1;
                this.game = game;
            }

            private GameObjects game;

            /// <summary>
            /// The percentage increase of resources gathered per second by upgrades.
            /// </summary>
            public double ResourcesPerSecondEfficiency { get; set; }

            /// <summary>
            /// The flat increase of resources gathered per second by upgrades.
            /// </summary>
            public double ResourcesPerSecondBaseIncrease { get; set; }

            /// <summary>
            /// The base quantity of resources gathered per second.
            /// </summary>
            public double BaseResourcesPerSecond { get; set; }

            public double TotalBaseResourcesPerSecond { get
            {
                return BaseResourcesPerSecond + ResourcesPerSecondBaseIncrease;
            } }

            /// <summary>
            /// The quantity of resources gathered per second.
            /// </summary>
            public double ResourcesPerSecond { get { return TotalBaseResourcesPerSecond * ResourcesPerSecondEfficiency; } }

            /// <summary>
            /// A buffer to hold left over efficiency.
            /// </summary>
            public double ResourceBuffer=0;

            /// <summary>
            /// The resources the Gatherer will collect.
            /// </summary>
            public List<Items.Resource> GuaranteedResources { get; set; }

            /// <summary>
            /// The resources the Gatherer can collect.
            /// </summary>
            public List<Items.Resource> PossibleResources { get; set; }

            /// <summary>
            /// The total probability of all possible resources.
            /// </summary>
            private int totalProbability;

            public void RecalculateMiningStuff()
            {
                totalProbability = PossibleResources.Sum(resource=>resource.Probability);
            }

            /// <summary>
            /// Gathers resources based on time.
            /// </summary>
            /// <param name="ms">Time that has passed since last mine.</param>
            public void Mine(double ms)
            {
                RecalculateMiningStuff();

                var resourcesGained = ResourcesPerSecond;
                resourcesGained += ResourceBuffer;
                resourcesGained *= (ms / 1000); // gathers resources based on time passed.
                // Stores excess resources in the resource buffer.
                ResourceBuffer = resourcesGained - Math.Floor(resourcesGained);
                resourcesGained = Math.Floor(resourcesGained);
               
                if (GuaranteedResources.Count > 0)
                {
                    // Guaranteed Resources
                    foreach (var resource in GuaranteedResources)
                        resource.Quantity += (int) resourcesGained;
                }

                if (PossibleResources.Count > 0)
                {
                    for (var i = 0; i < resourcesGained; i++)
                    {
                        var chance = game.Random.Next(1, totalProbability);
                        var probabilitySoFar = 0;
                        foreach (Items.Resource t in PossibleResources)
                        {
                            if (chance < t.Probability+probabilitySoFar)
                            {
                                t.Quantity++;
                                break;
                            }
                            probabilitySoFar += t.Probability;
                        }
                    }
                }
            }
        }

    }
}
