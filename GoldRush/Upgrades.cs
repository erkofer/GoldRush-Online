using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    public class Upgrades
    {
        public GameObjects game;

        public Upgrades(GameObjects game)
        {
            this.game = game;
            #region Upgrades
            Researcher = new Upgrade(new ResourceUpgradeEffect(game,
                new []{game.Gatherers.Miner},
                new []{game.Items.Sapphire,
                    game.Items.Emerald,
                    game.Items.Ruby}));
            Researcher.Name = "Researcher";

            ChainsawsT1 = new Upgrade(new EfficiencyUpgradeEffect(game,
                new []{game.Gatherers.Lumberjack},
                0.25));
            ChainsawsT1.Name = "Chainsaws";
            #endregion

            #region Buffs
            SpeechBuff = new Buff(new ItemValueUpgradeEffect(game, 0.2));
            SpeechBuff.Name = "Speech Buff";
            SpeechBuff.Duration = 45;
            game.Items.SpeechPotion.Effect = SpeechBuff;
            #endregion
        }

        /// <summary>
        /// Updates the duration on all buffs.
        /// </summary>
        public void Update()
        {
            
        }

        public Buff SpeechBuff;
        public Upgrade Researcher;
        public Upgrade ChainsawsT1;

        public class Upgrade : GameObjects.GameObject
        {
            public Upgrade(UpgradeEffect effect)
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

        public class Buff : Upgrade
        {
            public Buff(UpgradeEffect effect):base(effect)
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
        public abstract class UpgradeEffect
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
        public class ItemValueUpgradeEffect:UpgradeEffect
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
        public class ResourceUpgradeEffect : UpgradeEffect
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

        public class EfficiencyUpgradeEffect : UpgradeEffect
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
        #endregion
    }
}
