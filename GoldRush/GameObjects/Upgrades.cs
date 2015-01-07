﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Upgrades
    {
        public GameObjects game;

        public Upgrades(GameObjects game)
        {
            //TODO: Create configurations for upgrades and buffs.-
            this.game = game;
            #region Upgrades
           /* Foreman = new Upgrade(new EfficiencyMagnitudeUpgradeEffect(game,
                new [] {game.Gatherers.Miner,
                    game.Gatherers.Lumberjack},
                0.15));
            Foreman.Name = "Foreman";
            All.Add(Foreman.Id,Foreman);

            Backpack = new Upgrade(new ResourceUpgradeEffect(game,
                new []{game.Gatherers.Lumberjack},
                new[]{game.Items.Cubicula,
                    game.Items.BitterRoot,
                    game.Items.Thornberries,
                    game.Items.Transfruit}));
           // Backpack.Name = "Backpack";
            All.Add(Backpack.Id, Backpack);

            Botanist = new Upgrade(new ResourceUpgradeEffect(game,
                new []{game.Gatherers.Lumberjack},
                new [] {game.Items.IronFlower,
                    game.Items.TongtwistaFlower,
                    game.Items.MeltingNuts}));
            Botanist.Requires = Backpack;
            //Botanist.Name = "Botanist";
            All.Add(Botanist.Id, Botanist);*/

            Researcher = new Upgrade(new ResourceUpgradeEffect(game,
                new []{game.Gatherers.Miner},
                new []{game.Items.Sapphire,
                    game.Items.Emerald,
                    game.Items.Ruby}),GameConfig.Upgrades.Researcher);
            All.Add(Researcher.Id, Researcher);
            /*
            ChainsawsT1 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new []{game.Gatherers.Lumberjack},
                0.25));
            ChainsawsT1.Name = "Chainsaws";
            All.Add(ChainsawsT1.Id, ChainsawsT1);*/
            #endregion

            #region Buffs
           /* SpeechBuff = new Buff(new ItemValueUpgradeEffect(game, 0.2));
            SpeechBuff.Name = "Speech Buff";
            SpeechBuff.Duration = 45;
            game.Items.SpeechPotion.Effect = SpeechBuff;*/
            #endregion
        }

        /// <summary>
        /// Updates the duration on all buffs.
        /// </summary>
        public void Update()
        {
            
        }

        public Dictionary<int, Upgrade> All = new Dictionary<int, Upgrade>();

        public Buff SpeechBuff;
        public Upgrade Backpack;
        public Upgrade Botanist;
        public Upgrade Researcher;
        public Upgrade Foreman;
        public Upgrade ChainsawsT1;

        internal class Upgrade : GameObjects.GameObject
        {
            public Upgrade(UpgradeEffect effect, GameConfig.Upgrades.UpgradeConfig config)
                :base(config)
            {
                Effect = effect;
            }

            public virtual void Activate()
            {
                if (!Active)
                {
                    Active = true;
                    Effect.Activate();
                }
            }

            public virtual void Deactivate()
            {
                if (Active)
                {
                    Active = false;
                    Effect.Deactivate();
                }
            }

            public UpgradeEffect Effect { get; set; }
        }

        internal class Buff : Upgrade
        {
            public Buff(UpgradeEffect effect, GameConfig.Upgrades.UpgradeConfig config)
                :base(effect,config)
            {
                
            }

            public override void Activate()
            {
                TimeActive = 0;
                base.Activate();
            }

            public override void Deactivate()
            {
                TimeActive = 0;
                base.Deactivate();
            }

            public void Update(double ms)
            {
                TimeActive += (ms/1000);
                if(TimeActive>Duration)
                    Deactivate();
            }

            /// <summary>
            /// The length of time this buff will last for.
            /// </summary>
            public double Duration;

            /// <summary>
            /// The time the buff has been active for.
            /// </summary>
            private double TimeActive;

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
        }

        /// <summary>
        /// Increases the value of items by a percentage.
        /// </summary>
        class ItemValueUpgradeEffect:UpgradeEffect
        {
            private double value;

            public ItemValueUpgradeEffect(GameObjects game, double value)
                :base(game)
            {
                this.value = value;
            }

            public override void Activate()
            {
                Game.Items.WorthModifier += value;
            }

            public override void Deactivate()
            {
                Game.Items.WorthModifier -= value;
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
                :base(game)
            {
                this.resources = resources;
                this.gatherers = gatherers;
            }

            public override void Activate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.PossibleResources.AddRange(resources);
                  
            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    foreach (var resource in resources)
                        gatherer.PossibleResources.Remove(resource);
                
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
                :base(game)
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
                    gatherer.ResourcesPerSecondEfficiency += magnitude;
                    
            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ResourcesPerSecondEfficiency -= magnitude;
                    
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
                    gatherer.ProbabilityModifier += probability;
                   
            }

            public override void Deactivate()
            {
                foreach (var gatherer in gatherers)
                    gatherer.ProbabilityModifier -= probability;
                  
            }
        }
        #endregion
    }
}
