using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace GoldRush
{
    class Achievements
    {
        public Achievements(GameObjects objs)
        {
            var timePlayedT1 = new StatisticAchievement(GameConfig.Achievements.TimePlayedT1);
            timePlayedT1.GameObject = objs.Statistics.TimePlayed;
            All.Add(timePlayedT1.Id, timePlayedT1);

            var timePlayedT2 = new StatisticAchievement(GameConfig.Achievements.TimePlayedT2);
            timePlayedT2.GameObject = objs.Statistics.TimePlayed;
            timePlayedT2.Requires = timePlayedT1;
            All.Add(timePlayedT2.Id, timePlayedT2);

            var timePlayedT3 = new StatisticAchievement(GameConfig.Achievements.TimePlayedT3);
            timePlayedT3.GameObject = objs.Statistics.TimePlayed;
            timePlayedT3.Requires = timePlayedT2;
            All.Add(timePlayedT3.Id, timePlayedT3);

            var timePlayedT4 = new StatisticAchievement(GameConfig.Achievements.TimePlayedT4);
            timePlayedT4.GameObject = objs.Statistics.TimePlayed;
            timePlayedT4.Requires = timePlayedT3;
            All.Add(timePlayedT4.Id, timePlayedT4);

            var timePlayedT5 = new StatisticAchievement(GameConfig.Achievements.TimePlayedT5);
            timePlayedT5.GameObject = objs.Statistics.TimePlayed;
            timePlayedT5.Requires = timePlayedT4;
            All.Add(timePlayedT5.Id, timePlayedT5);

            var moneyT1 = new ItemAlltimeQuantityAchievement(GameConfig.Achievements.MoneyT1);
            moneyT1.GameObject = objs.Items.Coins;
            All.Add(moneyT1.Id,moneyT1);

            var moneyT2 = new ItemAlltimeQuantityAchievement(GameConfig.Achievements.MoneyT2);
            moneyT2.GameObject = objs.Items.Coins;
            moneyT2.Requires = moneyT1;
            All.Add(moneyT2.Id, moneyT2);

            var moneyT3 = new ItemAlltimeQuantityAchievement(GameConfig.Achievements.MoneyT3);
            moneyT3.GameObject = objs.Items.Coins;
            moneyT3.Requires = moneyT2;
            All.Add(moneyT3.Id, moneyT3);

            var minerT1 = new StatisticAchievement(GameConfig.Achievements.MinerT1);
            minerT1.GameObject = objs.Statistics.RockClicked;
            All.Add(minerT1.Id,minerT1);

            var minerT2 = new StatisticAchievement(GameConfig.Achievements.MinerT2);
            minerT2.GameObject = objs.Statistics.RockClicked;
            minerT2.Requires = minerT1;
            All.Add(minerT2.Id, minerT2);
            
            var minerT3 = new StatisticAchievement(GameConfig.Achievements.MinerT3);
            minerT3.GameObject = objs.Statistics.RockClicked;
            minerT3.Requires = minerT2;
            All.Add(minerT3.Id, minerT3);

            var oilT1 = new ItemAlltimeQuantityAchievement(GameConfig.Achievements.OilT1);
            oilT1.GameObject = objs.Items.Oil;
            All.Add(oilT1.Id,oilT1);
        }

        public enum AchievementType
        {
            Undefined,
            Money,
            TimePlayed,
            RockClicks,
            Oil
        };

        public long TimePlayed;

        public Dictionary<int, Achievement> All = new Dictionary<int, Achievement>();

        internal abstract class Achievement : GameObjects.GameObject
        {
            private GameConfig.Achievements.AchievementConfig config;

            protected Achievement(GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {
                this.config = config;
                
            }

            public override bool Active
            {
                get
                {
                    if (Requires == null) return true;
                    else
                    {
                        return ((Achievement) Requires).Complete;
                    }
                }
            }

            /// <summary>
            /// Used to allow clients to generate tooltips dynamically.
            /// </summary>
            public AchievementType Type {get { return config.Type; }}
            public abstract long Progress { get; }
            public long Goal { get { return config.Goal; } }

            public bool Complete { get { return (Progress >= Goal); } }

            public GameObjects.GameObject GameObject { get; set; }

            /// <summary>
            /// The number of achievement points are granted upon completion of this achievement.
            /// </summary>
            public int Points { get { return config.Points; } }
        }

        internal class GameObjectQuantityAchievement : Achievement
        {
            public GameObjectQuantityAchievement(GoldRush.GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {

            }

            public override long Progress
            {
                get { return GameObject.Quantity; }
            }
        }

        internal class ItemAlltimeQuantityAchievement : Achievement
        {
            public ItemAlltimeQuantityAchievement(GoldRush.GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {

            }

            public override long Progress
            {
                get { return ((Items.Item)GameObject).LifeTimeTotal; }
            }
        }

        internal class StatisticAchievement : Achievement
        {
            public StatisticAchievement(GameConfig.Achievements.AchievementConfig config)
                : base(config)
            {

            }

            public override long Progress
            {
                get { return ((Statistics.Statistic)GameObject).Value; }
            }
        }

    }
}
