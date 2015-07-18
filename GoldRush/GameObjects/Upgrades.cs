using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Upgrades
    {
        public Upgrades(GameObjects game)
        {
            InitializeUpgrades(game);
            InitializeBuffs(game);
        }

        private void InitializeBuffs(GameObjects game)
        {
            SpeechBuff = new Buff(new ItemValueUpgradeEffect(game, 1.2), GameConfig.Upgrades.SpeechBuff);
            game.Items.SpeechPotion.Buff = SpeechBuff;
            Buffs.Add(SpeechBuff.Id, SpeechBuff);

            ClickingBuff = new Buff(new EfficiencyMagnitudeUpgradeEffect(game,
                new[] {game.Gatherers.Player},
                2), GameConfig.Upgrades.ClickingBuff);
            game.Items.ClickingPotion.Buff = ClickingBuff;
            Buffs.Add(ClickingBuff.Id, ClickingBuff);

            SmeltingBuff = new Buff(new ProcessorEfficiencyUpgradeEffect(game,
                new[] {game.Processing.Furnace},
                1.25), GameConfig.Upgrades.SmeltingBuff);
            game.Items.SmeltingPotion.Buff = SmeltingBuff;
            Buffs.Add(SmeltingBuff.Id, SmeltingBuff);

            AlchemyBuff = new Buff(new ProcessorRecipeEfficiencyUpgradeEffect(game,
                new[] {game.Processing.Furnace},
                2), GameConfig.Upgrades.AlchemyBuff);
            game.Items.AlchemyPotion.Buff = AlchemyBuff;
            Buffs.Add(AlchemyBuff.Id, AlchemyBuff);
        }

        private void InitializeUpgrades(GameObjects game)
        {
            var oreMiningMachines = new[] { game.Gatherers.Player, game.Gatherers.Miner, game.Gatherers.Drill, game.Gatherers.Excavator, game.Gatherers.Crusher };

            Foreman = new Upgrade(new EfficiencyMagnitudeUpgradeEffect(game,
                new[]
                {
                    game.Gatherers.Miner,
                    game.Gatherers.Lumberjack
                },
                1.15), GameConfig.Upgrades.Foreman);
            All.Add(Foreman.Id, Foreman);

            Backpack = new Upgrade(new ResourceUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                new[]
                {
                    game.Items.Cubicula,
                    game.Items.BitterRoot,
                    game.Items.Thornberries,
                    game.Items.Transfruit
                }), GameConfig.Upgrades.Backpack);
            All.Add(Backpack.Id, Backpack);

            Botanist = new Upgrade(new ResourceUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                new[]
                {
                    game.Items.IronFlower,
                    game.Items.TongtwistaFlower,
                    game.Items.MeltingNuts
                }), GameConfig.Upgrades.Botanist);
            Botanist.Requires = Backpack;
            All.Add(Botanist.Id, Botanist);

            Researcher = new Upgrade(new ResourceUpgradeEffect(game,
                oreMiningMachines,
                new[]
                {
                    game.Items.Sapphire,
                    game.Items.Emerald,
                    game.Items.Ruby
                }), GameConfig.Upgrades.Researcher);
            All.Add(Researcher.Id, Researcher);

            ClickUpgradeT1 = new Upgrade(new BaseEfficiencyUpgradeEffect(game, new[] {game.Gatherers.Player}, 1),
                GameConfig.Upgrades.ClickUpgradeT1);
            All.Add(ClickUpgradeT1.Id, ClickUpgradeT1);

            ClickUpgradeT2 = new Upgrade(new BaseEfficiencyUpgradeEffect(game, new[] {game.Gatherers.Player}, 5),
                GameConfig.Upgrades.ClickUpgradeT2);
            ClickUpgradeT2.Requires = ClickUpgradeT1;
            All.Add(ClickUpgradeT2.Id, ClickUpgradeT2);

            ClickUpgradeT3 = new Upgrade(new BaseEfficiencyUpgradeEffect(game, new[] {game.Gatherers.Player}, 10),
                GameConfig.Upgrades.ClickUpgradeT3);
            ClickUpgradeT3.Requires = ClickUpgradeT2;
            All.Add(ClickUpgradeT3.Id, ClickUpgradeT3);

            // Craftable
            ChainsawsT1 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                0.05), GameConfig.Upgrades.ChainsawsT1);
            All.Add(ChainsawsT1.Id, ChainsawsT1);

            ChainsawsT2 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                0.1), GameConfig.Upgrades.ChainsawsT2);
            ChainsawsT2.Requires = ChainsawsT1;
            All.Add(ChainsawsT2.Id, ChainsawsT2);

            ChainsawsT3 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                0.2), GameConfig.Upgrades.ChainsawsT3);
            ChainsawsT3.Requires = ChainsawsT2;
            All.Add(ChainsawsT3.Id, ChainsawsT3);

            ChainsawsT4 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new[] {game.Gatherers.Lumberjack},
                0.4), GameConfig.Upgrades.ChainsawsT4);
            ChainsawsT4.Requires = ChainsawsT3;
            All.Add(ChainsawsT4.Id, ChainsawsT4);

            ReinforcedFurnace = new Upgrade(new ProcessorCapacityUpgradeEffect(game,
                new[] {game.Processing.Furnace},
                150), GameConfig.Upgrades.ReinforcedFurnace);
            All.Add(ReinforcedFurnace.Id, ReinforcedFurnace);

            LargerCauldron = new Upgrade(new ProcessorCapacityUpgradeEffect(game,
                new[] {game.Processing.Cauldron},
                9), GameConfig.Upgrades.LargerCauldron);
            All.Add(LargerCauldron.Id, LargerCauldron);

            DeeperTunnels = new Upgrade(new ResourceUpgradeEffect(game,
                oreMiningMachines,
                new[] {game.Items.Uranium, game.Items.Titanium}), GameConfig.Upgrades.DeeperTunnels);
            All.Add(DeeperTunnels.Id, DeeperTunnels);

            IronPickaxe = new Upgrade(new ProbabilityUpgradeEffect(game,
                new[] {game.Gatherers.Player},
                1.05), GameConfig.Upgrades.IronPickaxe);
            All.Add(IronPickaxe.Id, IronPickaxe);

            SteelPickaxe = new Upgrade(new ProbabilityUpgradeEffect(game,
                new[] {game.Gatherers.Player},
                1.1), GameConfig.Upgrades.SteelPickaxe);
            SteelPickaxe.Requires = IronPickaxe;
            All.Add(SteelPickaxe.Id, SteelPickaxe);

            GoldPickaxe = new Upgrade(new ProbabilityUpgradeEffect(game,
                new[] {game.Gatherers.Player},
                1.15), GameConfig.Upgrades.GoldPickaxe);
            GoldPickaxe.Requires = SteelPickaxe;
            All.Add(GoldPickaxe.Id, GoldPickaxe);

            DiamondPickaxe = new Upgrade(new ProbabilityUpgradeEffect(game,
                new[] {game.Gatherers.Player},
                1.25), GameConfig.Upgrades.DiamondPickaxe);
            DiamondPickaxe.Requires = GoldPickaxe;
            All.Add(DiamondPickaxe.Id, DiamondPickaxe);

            Furnace = new Upgrade(new ProcessorUnlockUpgradeEffect(game,
                new[] {game.Processing.Furnace}), GameConfig.Upgrades.Furnace);
            game.Processing.Furnace.Requires = Furnace;
            All.Add(Furnace.Id, Furnace);

            Cauldron = new Upgrade(new ProcessorUnlockUpgradeEffect(game,
                new[] {game.Processing.Cauldron}), GameConfig.Upgrades.Cauldron);
            game.Processing.Cauldron.Requires = Cauldron;
            All.Add(Cauldron.Id, Cauldron);

            Geologist = new Upgrade(new ResourceUpgradeEffect(game,
                oreMiningMachines,
                new[]
                {
                    game.Items.Diamond,
                    game.Items.Quartz,
                    game.Items.Onyx
                }), GameConfig.Upgrades.Geologist);
            All.Add(Geologist.Id, Geologist);
        }

        /// <summary>
        /// Updates the duration on all buffs.
        /// </summary>
        public void Update(long seconds)
        {
            foreach (var upgradePair in All)
            {
                var upgrade = upgradePair.Value;

                if (upgrade.Quantity > 0)
                    upgrade.Activate();

                if (upgrade.Quantity == 0)
                    upgrade.Deactivate();
            }

            foreach (var buffPair in Buffs)
            {
                var buff = buffPair.Value;

                if (buff.TimeActive > 0 && buff.TimeActive < buff.Duration)
                    buff.Activate();
                else
                {
                    buff.Deactivate();
                    buff.TimeActive = 0;
                }

                buff.Update(seconds);
            }
        }

        public double SecondsToPotionExpiry()
        {
            var greatestDuration = 0.0;
            foreach (var buffPair in Buffs)
            {
                var buff = buffPair.Value;
                if (buff.TimeActive == 0) continue;

                var timeRemaining = buff.Duration - buff.TimeActive;
                if (timeRemaining > greatestDuration) 
                    greatestDuration = timeRemaining;
            }
            return greatestDuration;
        }

        public Dictionary<int, Upgrade> All = new Dictionary<int, Upgrade>();
        public Dictionary<int, Buff> Buffs = new Dictionary<int, Buff>();

        public Buff SpeechBuff;
        public Buff ClickingBuff;
        public Buff SmeltingBuff;
        public Buff AlchemyBuff;

        public Upgrade Backpack;
        public Upgrade Botanist;
        public Upgrade Researcher;
        public Upgrade Foreman;
        public Upgrade ChainsawsT1;
        public Upgrade ChainsawsT2;
        public Upgrade ChainsawsT3;
        public Upgrade ChainsawsT4;

        public Upgrade ClickUpgradeT1;
        public Upgrade ClickUpgradeT2;
        public Upgrade ClickUpgradeT3;

        public Upgrade ReinforcedFurnace;
        public Upgrade LargerCauldron;

        public Upgrade DeeperTunnels;

        public Upgrade IronPickaxe;
        public Upgrade SteelPickaxe;
        public Upgrade GoldPickaxe;
        public Upgrade DiamondPickaxe;

        public Upgrade Furnace;
        public Upgrade Cauldron;

        public Upgrade Geologist;

        internal class Upgrade : GameObjects.GameObject
        {
            public Upgrade(UpgradeEffect effect, GameConfig.Upgrades.UpgradeConfig config)
                : base(config)
            {
                Effect = effect;
            }

            public virtual void Activate()
            {
                if (Active) return;
                Active = true;
                Effect.Activate();
            }

            public virtual void Deactivate()
            {
                if (!Active) return;
                Active = false;
                Effect.Deactivate();
            }

            public override string Tooltip
            {
                get { return Effect.Tooltip; }
            }

            private UpgradeEffect Effect { get; set; }
        }

        internal class Buff : Upgrade
        {
            private readonly GameConfig.Upgrades.BuffConfig _config;

            public Buff(UpgradeEffect effect, GameConfig.Upgrades.BuffConfig config)
                : base(effect, config)
            {
                _config = config;
            }

            public void Update(long seconds)
            {
                if (Active)
                    TimeActive += seconds;
            }

            /// <summary>
            /// The length of time this buff will last for.
            /// </summary>
            public double Duration { get { return _config.Duration; } }

            /// <summary>
            /// The time the buff has been active for.
            /// </summary>
            public double TimeActive;
        }

        #region UpgradeEffects

        internal abstract class UpgradeEffect
        {
            public GameObjects Game;
            protected UpgradeEffect(GameObjects game)
            {
                Game = game;
            }

            public abstract void Activate();
            public abstract void Deactivate();

            public abstract string Tooltip { get; }
        }

        /// <summary>
        /// Increases the value of items by a percentage.
        /// </summary>
        class ItemValueUpgradeEffect : UpgradeEffect
        {
            private double value;

            public ItemValueUpgradeEffect(GameObjects game, double value)
                : base(game)
            {
                this.value = value;
            }

            public override void Activate()
            {
                Game.Items.WorthModifier *= value;
            }

            public override void Deactivate()
            {
                Game.Items.WorthModifier /= value;
            }

            public override string Tooltip
            {
                get { return "Increases the value of items by " + ((value * 100) - 100) + "%."; }
            }
        }

        /// <summary>
        /// Adds new resources to gatherers.
        /// </summary>
        class ResourceUpgradeEffect : UpgradeEffect
        {
            private Items.Resource[] resources;
            private Gatherers.Gatherer[] gatherers;
            public ResourceUpgradeEffect(GameObjects game, Gatherers.Gatherer[] gatherers, Items.Resource[] resources)
                : base(game)
            {
                this.resources = resources;
                this.gatherers = gatherers;
            }

            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                {
                    gatherer.PossibleResources.AddRange(resources);
                    gatherer.RecalculateMiningStuff();
                }

            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                {
                    foreach (var resource in resources)
                        gatherer.PossibleResources.Remove(resource);

                    gatherer.RecalculateMiningStuff();
                }
            }
            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var resource in resources)
                        names.Add(resource.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Discovers " + concatenatedNames + ".";
                }
            }
        }

        /// <summary>
        /// Increases the base efficiency of gatherers.
        /// </summary>
        class EfficiencyUpgradeEffect : UpgradeEffect
        {
            private Gatherers.Gatherer[] gatherers;
            private double efficiency;

            public EfficiencyUpgradeEffect(GameObjects game, Gatherers.Gatherer[] gatherers, double efficiency)
                : base(game)
            {
                this.gatherers = gatherers;
                this.efficiency = efficiency;
            }

            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondBaseIncrease += efficiency;

            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondBaseIncrease -= efficiency;

            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var gatherer in gatherers)
                        names.Add(gatherer.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases the resources gathered per tick by " + efficiency + " for " + concatenatedNames + ".";
                }
            }
        }

        /// <summary>
        /// Increases the efficiency of gatherers by a percentage.
        /// </summary>
        class EfficiencyMagnitudeUpgradeEffect : UpgradeEffect
        {
            private Gatherers.Gatherer[] gatherers;
            private double magnitude;

            public EfficiencyMagnitudeUpgradeEffect(GameObjects game, Gatherers.Gatherer[] gatherers, double magnitude)
                : base(game)
            {
                this.gatherers = gatherers;
                this.magnitude = magnitude;
            }
            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondEfficiency *= magnitude;

            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondEfficiency /= magnitude;

            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var gatherer in gatherers)
                        names.Add(gatherer.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases the resources gathered per tick by " + ((magnitude * 100) - 100) + "% for " + concatenatedNames + ".";
                }
            }
        }

        // TODO: Find an elegant way to test probability upgrades.

        /// <summary>
        /// Increases the chance of finding rarer ores and decreases the chance of finding more common ores.
        /// </summary>
        class ProbabilityUpgradeEffect : UpgradeEffect
        {
            private Gatherers.Gatherer[] gatherers;
            private double probability;

            public ProbabilityUpgradeEffect(GameObjects game, Gatherers.Gatherer[] gatherers, double probability)
                : base(game)
            {
                this.gatherers = gatherers;
                this.probability = probability;
            }

            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ProbabilityModifier *= probability;

            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ProbabilityModifier /= probability;

            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var gatherer in gatherers)
                        names.Add(gatherer.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases the chance of gathering rare resources by " + ((probability * 100) - 100) + "% for " + concatenatedNames + ".";
                }
            }
        }

        class BaseEfficiencyUpgradeEffect : UpgradeEffect
        {
            private Gatherers.Gatherer[] gatherers;
            private double efficiencyIncrease;

            public BaseEfficiencyUpgradeEffect(GameObjects game, Gatherers.Gatherer[] gatherers, double efficiencyIncrease)
                : base(game)
            {
                this.gatherers = gatherers;
                this.efficiencyIncrease = efficiencyIncrease;
            }

            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondBaseIncrease += efficiencyIncrease;

            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondBaseIncrease -= efficiencyIncrease;

            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var gatherer in gatherers)
                        names.Add(gatherer.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases the resources gathered per tick by " + efficiencyIncrease + " for " + concatenatedNames + ".";
                }
            }
        }

        class ProcessorCapacityUpgradeEffect : UpgradeEffect
        {
            Processing.Processor[] processors;
            int capacity;

            public ProcessorCapacityUpgradeEffect(GameObjects game, Processing.Processor[] processors, int capacity)
                : base(game)
            {
                this.processors = processors;
                this.capacity = capacity;
            }
            public override void Activate()
            {
                foreach (var processor in processors)
                {
                    processor.ExtraCapacity += capacity;
                }
            }

            public override void Deactivate()
            {
                foreach (var processor in processors)
                {
                    processor.ExtraCapacity -= capacity;
                }
            }

            public override string Tooltip
            {

                get
                {
                    List<string> names = new List<string>();

                    foreach (var processor in processors)
                        names.Add(processor.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases capacity of " + concatenatedNames + " by " + capacity + ".";
                }
            }
        }

        class ProcessorEfficiencyUpgradeEffect : UpgradeEffect
        {
            Processing.Processor[] processors;
            double efficiency;

            public ProcessorEfficiencyUpgradeEffect(GameObjects game, Processing.Processor[] processors, double efficiency)
                : base(game)
            {
                this.processors = processors;
                this.efficiency = efficiency;
            }
            public override void Activate()
            {
                foreach (var processor in processors)
                {
                    processor.Speed *= efficiency;
                }
            }

            public override void Deactivate()
            {
                foreach (var processor in processors)
                {
                    processor.Speed /= efficiency;
                }
            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var processor in processors)
                        names.Add(processor.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Increases efficiency of " + concatenatedNames + " by " + ((efficiency * 100) - 100) + "%.";
                }
            }
        }

        class ProcessorRecipeEfficiencyUpgradeEffect : UpgradeEffect
        {
            readonly Processing.Processor[] _processors;
            readonly int _increase;
            public ProcessorRecipeEfficiencyUpgradeEffect(GameObjects game, Processing.Processor[] processors, int increase)
                : base(game)
            {
                _processors = processors;
                _increase = increase;
            }
            public override void Activate()
            {
                foreach (var processor in _processors)
                {
                    processor.RecipesCraftedPerIteration += _increase;
                }
            }

            public override void Deactivate()
            {
                foreach (var processor in _processors)
                {
                    processor.RecipesCraftedPerIteration -= _increase;
                }
            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var processor in _processors)
                        names.Add(processor.Name);

                    string concatenatedNames = String.Join(",", names);

                    return concatenatedNames + " craft an additional " + _increase + " recipes per iteration.";
                }
            }
        }

        class ProcessorUnlockUpgradeEffect : UpgradeEffect
        {
            readonly Processing.Processor[] _processors;

            public ProcessorUnlockUpgradeEffect(GameObjects game, Processing.Processor[] processors)
                : base(game)
            {
                _processors = processors;
            }

            public override void Activate()
            {
                foreach (var processor in _processors)
                    processor.Quantity = 1;
            }

            public override void Deactivate()
            {
                foreach (var processor in _processors)
                    processor.Quantity = 0;
            }

            public override string Tooltip
            {
                get
                {
                    List<string> names = new List<string>();

                    foreach (var processor in _processors)
                        names.Add(processor.Name);

                    string concatenatedNames = String.Join(",", names);

                    return "Unlocks "+concatenatedNames+".";
                }
            }
        }
        #endregion
    }
}

