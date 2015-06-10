using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class Tutorial
    {
        private GameObjects.Notifier notify;

        public Tutorial(GameObjects objs)
        {
            notify = objs.Notify;
        }

        class TutorialStage
        {
            public Achievements.Achievement CompletionTrigger;
            public string Message;
        }
    }
}
