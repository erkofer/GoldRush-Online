using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace GoldRush
{
    class Gatherers
    {
        public GameObjects Game;
        private Random acRandom = new Random();
        public Gatherers(GameObjects game)
        {
            ScrambleAntiCheat();

            Game = game;
            var baseResources = new[]
            {
                game.Items.Stone, game.Items.Copper, game.Items.Iron, game.Items.Silver,
                game.Items.Gold, game.Items.Opal, game.Items.Jade, game.Items.Topaz
            };

            Player = new Gatherer(game, GameConfig.Gatherers.Player);
            Player.Quantity = 1;
            Player.PossibleResources.AddRange(baseResources);

            Miner = new Gatherer(game,GameConfig.Gatherers.Miner);
            Miner.PossibleResources.AddRange(baseResources);
            All.Add(Miner.Id, Miner);

            Lumberjack = new Gatherer(game, GameConfig.Gatherers.Lumberjack);
            Lumberjack.GuaranteedResources.Add(game.Items.Logs);
            Lumberjack.ChanceOfNothing = 300;
            All.Add(Lumberjack.Id, Lumberjack);

            Drill = new Gatherer(game, GameConfig.Gatherers.Drill);
            Drill.PossibleResources.AddRange(baseResources);
            Drill.Requires = Miner;
            Drill.Fuel = game.Items.Oil;
            All.Add(Drill.Id, Drill);

            Crusher = new Gatherer(game, GameConfig.Gatherers.Crusher);
            Crusher.PossibleResources.AddRange(baseResources);
            Crusher.Requires = Drill;
            Crusher.Fuel = game.Items.Oil;
            All.Add(Crusher.Id, Crusher);

            Excavator = new Gatherer(game, GameConfig.Gatherers.Excavator);
            Excavator.PossibleResources.AddRange(baseResources);
            Excavator.Requires = Crusher;
            Excavator.Fuel = game.Items.Oil;
            All.Add(Excavator.Id, Excavator);

            Pumpjack = new Gatherer(game, GameConfig.Gatherers.Pumpjack);
            Pumpjack.GuaranteedResources.Add(game.Items.Oil);
            All.Add(Pumpjack.Id, Pumpjack);

            BigTexan = new Gatherer(game, GameConfig.Gatherers.BigTexan);
            BigTexan.GuaranteedResources.Add(game.Items.Oil);
            BigTexan.Requires = Pumpjack;
            All.Add(BigTexan.Id, BigTexan);
        }

        public void Update(int ms)
        {
            foreach (var gatherer in All)
            {
                gatherer.Value.Mine(ms);
            }
        }


        public void Mine(int x, int y)
        {
            if (AntiCheat(x, y))
            {
                Player.Mine(1000);
                AntiCheatNextChange--;
            }

            if(AntiCheatNextChange <= 0)
                ScrambleAntiCheat();
        }

        private bool AntiCheat(int x, int y)
        {
            return (x > AntiCheatX && x < AntiCheatX + 64 && y > AntiCheatY && y < AntiCheatY + 64);
        }

        private void ScrambleAntiCheat()
        {
            AntiCheatX = acRandom.Next(5, 195);
            AntiCheatY = acRandom.Next(5, 195);
            AntiCheatNextChange = 25;
        }

        public Dictionary<int,Gatherer> All = new Dictionary<int,Gatherer>();

        public Gatherer Player;
        public Gatherer Miner;
        public Gatherer Lumberjack;
        public Gatherer Drill;
        public Gatherer Crusher;
        public Gatherer Excavator;
        public Gatherer Pumpjack;
        public Gatherer BigTexan;

        public int AntiCheatX;
        public int AntiCheatY;
        public int AntiCheatNextChange;

        internal class Gatherer : GameObjects.GameObject
        {
            public Gatherer(GameObjects game, GameConfig.Gatherers.GathererConfig config):base(config)
            {
                PossibleResources = new List<Items.Resource>();
                GuaranteedResources = new List<Items.Resource>();
                ChanceOfNothing = 0;
                _game = game;
                _config = config;
            }

            private readonly GameObjects _game;
            private readonly GameConfig.Gatherers.GathererConfig _config;
            /// <summary>
            /// The odds of no resources being gathered.
            /// </summary>
            public int ChanceOfNothing { get; set; }

            public GameObjects.GameObject Fuel { get; set; }

            public double FuelConsumption { get { return _config.FuelConsumption; } }

            private double resourcesPerSecondEfficiency=1;
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

            public bool Enabled = true;

            private double probabilityModifier=1;
            /// <summary>
            /// The increased chance of finding rarer ores.
            /// </summary>
            public double ProbabilityModifier { get { return probabilityModifier; } 
                set { probabilityModifier = value; RecalculateMiningStuff(); } }

            /// <summary>
            /// The quantity of resources gathered per second.
            /// </summary>
            public double ResourcesPerSecond { get { return (TotalBaseResourcesPerSecond * (ResourcesPerSecondEfficiency))*Quantity; } }

            /// <summary>
            /// A buffer to hold left over efficiency.
            /// </summary>
            private double resourceBuffer;

            public double ResourceBuffer { get { return resourceBuffer; } set { resourceBuffer = value; } }

            /// <summary>
            /// The resources the Gatherer will collect.
            /// </summary>
            public List<Items.Resource> GuaranteedResources { get; set; }

            private List<Items.Resource> possibleResources; 
            /// <summary>
            /// The resources the Gatherer can collect.
            /// </summary>
            public List<Items.Resource> PossibleResources { get { return possibleResources; } 
                set { possibleResources = value; } }

            /// <summary>
            /// The total probability of all possible resources.
            /// </summary>
            private List<int> totalProbability;

            /// <summary>
            /// The totaled probability of all resources.
            /// </summary>
            private int totaledProbability;

            /// <summary>
            /// Determines when we must recalculate mining stuff.
            /// </summary>
            private bool recalculate=true;

            int factorial(int i)
            {
                if (i <= 1)
                    return 1;
                return i * factorial(i - 1);
            }

            public override bool Active
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

            /// <summary>
            /// Forces the gatherer to recalculate its mining variables next update.
            /// </summary>
            public void RecalculateMiningStuff()
            {
                recalculate = true;
            }

            private void recalculateMiningStuff()
            {
                totaledProbability = 0;
                totalProbability = new List<int>();
                var total = 0;
                foreach (var resource in PossibleResources)
                {
                    totalProbability.Add(total += resource.Probability);
                    totaledProbability += resource.Probability;
                }

                if (ProbabilityModifier <= 0) return;
                
                for (var i = 0; i < totalProbability.Count; i++)
                    if (totalProbability[i] < (total/2))
                        totalProbability[i] *= (int) Math.Floor(ProbabilityModifier);
                    else
                        totalProbability[i] /= (int) Math.Floor(ProbabilityModifier);
            }
             

            /// <summary>
            /// Gathers resources based on time.
            /// </summary>
            /// <param name="ms">Time that has passed since last mine.</param>
            public void Mine(double ms)
            {
                if (!Enabled) return;

                if(ResourcesPerSecond<=0) return;

                if (recalculate)
                {
                    recalculateMiningStuff();
                    recalculate = false;
                }

                var fuelEfficiency=1.0;
                if (Fuel != null)
                {
                    var fuelToConsume = Math.Min(Fuel.Quantity, FuelConsumption*Quantity);
                    Fuel.Quantity -= (long)fuelToConsume;

                    fuelEfficiency = fuelToConsume/FuelConsumption;
                }

                var resourcesGained = ResourcesPerSecond;
                resourcesGained *= fuelEfficiency;

                resourcesGained += resourceBuffer;
                resourcesGained *= (ms / 1000); // gathers resources based on time passed.
                // Stores excess resources in the resource buffer.
                resourceBuffer = resourcesGained - Math.Floor(resourcesGained);
                resourcesGained = Math.Floor(resourcesGained);

                if (GuaranteedResources.Count > 0)
                    foreach (var resource in GuaranteedResources)
                        resource.Quantity += (int) resourcesGained;

                if (PossibleResources.Count <= 0) return;

                if (ms <= 10000) // if less than ten seconds have passed since we last mined.
                {
                    for (var i = 0; i < resourcesGained; i++)
                    {
                        if (totalProbability.ToArray().Length < 1) break;

                        var random = _game.Random.Next(ChanceOfNothing + 1);

                        if (ChanceOfNothing != random) continue;

                        var chance = _game.Random.Next(1, totalProbability[totalProbability.Count - 1]);
                        var roll = Array.BinarySearch(totalProbability.ToArray(), chance);
                        if (roll < 0) roll = ~roll;
                        PossibleResources[roll].Quantity++;
                    }
                }
                else
                {
                    for (var i = 0; i < PossibleResources.Count; i++)
                    {
                        double ticks = (ms / 1000);

                        double chance = ((double)PossibleResources[i].Probability / totaledProbability);
                        double r = chance * resourcesGained;
                        double luck = ((double)_game.Random.Next(0, 11) / 10);
                        double probability = 1 / Math.Pow(2.71828, r);
                        int resourcesToGain = 0;
                        
                        if(r < 10)
                        {
                            while (luck > probability)
                            {
                                resourcesToGain++;
                                probability += Math.Pow(r, resourcesToGain) / (factorial(resourcesToGain) * Math.Pow(2.71828, r));
                            }
                        }
                        else
                        {
                            double expectedOutput = ticks*chance;
                            bool increase = _game.Random.NextDouble() >= 0.5;
                            double change = (double) _game.Random.Next(1, 3)/10;
                            
                            resourcesToGain = (int)Math.Floor(increase ? (1+change) * expectedOutput : (1-change)*expectedOutput);
                        }
                        PossibleResources[i].Quantity+=resourcesToGain;
                    }
                }
            }
        }
    }
}
