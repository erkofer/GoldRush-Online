using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Tutorial
    {
        public Tutorial(GameObjects objs)
        {
            /* MINING TUTORIAL.
             * Click the rock 10 times.
             */
            var miningTutorialTrigger = new Achievements.StatisticAchievement(GameConfig.Achievements.TutorialMining);
            miningTutorialTrigger.GameObject = objs.Statistics.RockClicked;

            var miningTutorial = new TutorialStage
            {
                CompletionTrigger = miningTutorialTrigger,
                Title="MiningTutorial"
            };
            stages.Add(miningTutorial);
            /* SELLING TUTORIAL
             * Earn 1000 coins.
             */

            var sellingTutorialTrigger = new Achievements.ItemAlltimeQuantityAchievement(GameConfig.Achievements.TutorialSelling);
            sellingTutorialTrigger.GameObject = objs.Items.Coins;

            var sellingTutorial = new TutorialStage
            {
                CompletionTrigger = sellingTutorialTrigger,
                Title = "SellingTutorial"
            };
            stages.Add(sellingTutorial);
            /* STORE TUTORIAL
             * Hire a miner.
             */

            var storeTutorialTrigger = new Achievements.GameObjectQuantityAchievement(GameConfig.Achievements.TutorialGatherers);
            storeTutorialTrigger.GameObject = objs.Gatherers.Miner;

            var storeTutorial = new TutorialStage
            {
                CompletionTrigger = storeTutorialTrigger,
                Title = "StoreTutorial"
            };
            stages.Add(storeTutorial);
        }

        private List<TutorialStage> stages = new List<TutorialStage>();

        public string GetActiveTutorialTitle()
        {
            for (var i = 0; i < stages.Count; i++)
            {
                var stage = stages[i];
                if(stage.Completed) continue;

                return stage.Title;
            }
            return "Completed";
        }

        class TutorialStage
        {
            public Achievements.Achievement CompletionTrigger;

            public bool Completed { get { return CompletionTrigger.Complete; } }
            /// <summary>
            /// Stringly-typed title to tell client which tutorial to open.
            /// </summary>
            public string Title;
        }
    }
}
