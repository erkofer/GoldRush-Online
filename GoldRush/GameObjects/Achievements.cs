using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Achievements
    {
        public Achievements(GameObjects objs)
        {

        }


        public enum AchievementType
        {
            Undefined,
            ItemAlltimeQuantity,
            TimePlayed
        };

        public long TimePlayed;

        internal abstract class Achievement : GoldRush.GameObjects.GameObject
        {
            protected Achievement(GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {

            }

            /// <summary>
            /// Used to allow clients to generate tooltips dynamically.
            /// </summary>
            public abstract AchievementType Type { get; }
            public abstract double Progress { get; }
            public double Goal { get; set; }

            public GoldRush.GameObjects.GameObject GameObject { get; set; }

            /// <summary>
            /// The number of achievement points are granted upon completion of this achievement.
            /// </summary>
            public int Points { get; set; }
        }


        internal class ItemAlltimeQuantityAchievement : Achievement
        {
            public ItemAlltimeQuantityAchievement(GoldRush.GameConfig.Achievements.AchievementConfig config)
                :base(config)
            {
                
            }

            public override AchievementType Type { get { return AchievementType.ItemAlltimeQuantity; } }

            public override double Progress
            {
                get { return GameObject.Quantity; }
            }

            //public override string Tooltip { get { return "Have an alltime total of " + Goal + " " + GameObject.Name; } }
        }

        internal class TimePlayedAchievement : Achievement
        {
            public TimePlayedAchievement(GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {
                
            }

            public override AchievementType Type { get { return AchievementType.TimePlayed; } }

            public override double Progress
            {
                //get { return GameObject.TimePlayed; }
                get { return 0; }
            }
        }
    }
}
