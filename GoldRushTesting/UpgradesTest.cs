using System;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRushTesting
{
    [TestClass]
    public class UpgradesTest
    {
        /// <summary>
        /// Copper is worth 5 base. With a 20% increase in value it is worth 6.
        /// </summary>
        [TestMethod]
        public void ItemValueActivate()
        {
            Game game = new Game();
            game.objs.Upgrades.SpeechBuff.Activate();

            Assert.AreEqual(6, game.objs.Items.Copper.Value);
        }

        /// <summary>
        /// Copper is worth 5 base. when activated and then deactivated it should return to normal.
        /// </summary>
        [TestMethod]
        public void ItemValueDeactivate()
        {
            Game game = new Game();
            game.objs.Upgrades.SpeechBuff.Activate();
            game.objs.Upgrades.SpeechBuff.Deactivate();

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }

        /// <summary>
        /// The Speech Potion lasts 45 seconds. We ensure it is correctly deactivated.
        /// </summary>
        [TestMethod]
        public void BuffDecay()
        {
            var game = new Game();
            game.objs.Upgrades.SpeechBuff.Activate();
            game.objs.Upgrades.SpeechBuff.Update(46000);

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }

        [TestMethod]
        public void BuffNoDecay()
        {
            var game = new Game();
            game.objs.Upgrades.SpeechBuff.Activate();
            game.objs.Upgrades.SpeechBuff.Update(44000);

            Assert.AreEqual(6, game.objs.Items.Copper.Value);
        }
    }
}
