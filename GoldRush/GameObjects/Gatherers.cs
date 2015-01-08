using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Gatherers
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

            Miner = new Gatherer(game,GameConfig.Gatherers.Miner);
            Miner.PossibleResources.AddRange(baseResources);
            All.Add(Miner.Id, Miner);

            Lumberjack = new Gatherer(game, GameConfig.Gatherers.Lumberjack);
            Lumberjack.GuaranteedResources.Add(game.Items.Logs);
            Lumberjack.ChanceOfNothing = 300;
            All.Add(Lumberjack.Id, Lumberjack);

            Pumpjack = new Gatherer(game, GameConfig.Gatherers.Pumpjack);
            Pumpjack.GuaranteedResources.Add(game.Items.Oil);
            All.Add(Pumpjack.Id, Pumpjack);
        }
        
        public Dictionary<int,Gatherer> All = new Dictionary<int,Gatherer>();

        public Gatherer Miner;
        public Gatherer Lumberjack;
        public Gatherer Pumpjack;

        internal class Gatherer : GameObjects.GameObject
        {
            public Gatherer(GameObjects game, GameConfig.Gatherers.GathererConfig config):base(config)
            {
                PossibleResources = new List<Items.Resource>();
                GuaranteedResources = new List<Items.Resource>();
                _game = game;
                _config = config;
            }

            private readonly GameObjects _game;
            private readonly GameConfig.Gatherers.GathererConfig _config;
            /// <summary>
            /// The odds of no resources being gathered.
            /// </summary>
            public int ChanceOfNothing { get; set; }

            private double resourcesPerSecondEfficiency;
            /// <summary>
            /// The percentage increase of resources gathered per second by upgrades.
            /// </summary>
            public double ResourcesPerSecondEfficiency { get { return resourcesPerSecondEfficiency; } 
                set { resourcesPerSecondEfficiency = value; RecalculateMiningStuff(); } }

            private double resourcesPerSecondBaseIncrease;
            /// <summary>
            /// The flat increase of resources gathered per second by upgrades.
            /// </summary>
            public double ResourcesPerSecondBaseIncrease { get { return resourcesPerSecondBaseIncrease; } 
                set { resourcesPerSecondBaseIncrease = value; RecalculateMiningStuff(); } }

            /// <summary>
            /// The base quantity of resources gathered per second.
            /// </summary>
            public double BaseResourcesPerSecond { get { return _config.BaseResourcesPerSecond; } }

            public double TotalBaseResourcesPerSecond { get { 
                return BaseResourcesPerSecond + ResourcesPerSecondBaseIncrease;
            } }

            private double probabilityModifier;
            /// <summary>
            /// The increased chance of finding rarer ores.
            /// </summary>
            public double ProbabilityModifier { get { return probabilityModifier; } 
                set { probabilityModifier = value; RecalculateMiningStuff(); } }

            /// <summary>
            /// The quantity of resources gathered per second.
            /// </summary>
            public double ResourcesPerSecond { get { return (TotalBaseResourcesPerSecond * (ResourcesPerSecondEfficiency+1))*Quantity; } }

            /// <summary>
            /// A buffer to hold left over efficiency.
            /// </summary>
            private double resourceBuffer;

            /// <summary>
            /// The resources the Gatherer will collect.
            /// </summary>
            public List<Items.Resource> GuaranteedResources { get; set; }

            private List<Items.Resource> possibleResources; 
            /// <summary>
            /// The resources the Gatherer can collect.
            /// </summary>
            public List<Items.Resource> PossibleResources { get { return possibleResources; } 
                set { possibleResources = value; RecalculateMiningStuff(); } }

            /// <summary>
            /// The total probability of all possible resources.
            /// </summary>
            private List<int> totalProbability;

            /// <summary>
            /// Determines when we must recalculate mining stuff.
            /// </summary>
            private bool recalculate=true;

            /// <summary>
            /// Forces the gatherer to recalculate its mining variables next update.
            /// </summary>
            public void RecalculateMiningStuff()
            {
                recalculate = true;
            }

            private void recalculateMiningStuff()
            {
                totalProbability = new List<int>();
                var total = 0;
                foreach (var resource in PossibleResources)
                    totalProbability.Add(total+=resource.Probability);

                if (ProbabilityModifier <= 0) return;
                
                for (var i = 0; i < totalProbability.Count; i++)
                    if (totalProbability[i] < (total/2))
                        totalProbability[i] *= (int) Math.Floor(ProbabilityModifier + 1);
                    else
                        totalProbability[i] /= (int) Math.Floor(ProbabilityModifier + 1);
            }

            /// <summary>
            /// Gathers resources based on time.
            /// </summary>
            /// <param name="ms">Time that has passed since last mine.</param>
            public void Mine(double ms)
            {
                if(ResourcesPerSecond<=0) return;

                if (recalculate)
                {
                    recalculateMiningStuff();
                    recalculate = false;
                }

                var resourcesGained = ResourcesPerSecond;
                resourcesGained += resourceBuffer;
                resourcesGained *= (ms / 1000); // gathers resources based on time passed.
                // Stores excess resources in the resource buffer.
                resourceBuffer = resourcesGained - Math.Floor(resourcesGained);
                resourcesGained = Math.Floor(resourcesGained);
               
                if (GuaranteedResources.Count > 0)
                    foreach (var resource in GuaranteedResources)
                        resource.Quantity += (int) resourcesGained;

                if (PossibleResources.Count <= 0) return;
                
                for (var i = 0; i < resourcesGained; i++)
                {
                    if (ChanceOfNothing != _game.Random.Next(ChanceOfNothing+1)) continue;

                    var chance = _game.Random.Next(1, totalProbability[totalProbability.Count - 1]);
                    var roll = Array.BinarySearch(totalProbability.ToArray(), chance);
                    if (roll < 0) roll = ~roll;
                    PossibleResources[roll].Quantity++;
                }
            }
        }
    }
}
