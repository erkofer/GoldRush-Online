using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    public class Upgrades
    {
        public GameObjects Game;

        public Upgrades(GameObjects game)
        {
            Game = game;
            ExpensiveItems = new Upgrade(new ItemValueUpgradeEffect(game,1));
            ExpensiveItems.Name = "Expensive Items";
        }

        public Upgrade ExpensiveItems;

        public class Upgrade : GameObjects.GameObject
        {
            public Upgrade(UpgradeEffect effect)
            {
                Effect = effect;
            }

            public void Activate()
            {
                if (!Active)
                {
                    Active = true;
                    Effect.Activate();
                }
            }

            public void Deactivate()
            {
                if (Active)
                {
                    Active = false;
                    Effect.Deactivate();
                }
            }

            public UpgradeEffect Effect { get; set; }
        }

        public class Buff
        {
            public Buff()
            {
                
            }

            public Upgrade Upgrade;
            public int Duration;
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

            public ItemValueUpgradeEffect(GameObjects game, double value):base(game)
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
        #endregion
    }
}
