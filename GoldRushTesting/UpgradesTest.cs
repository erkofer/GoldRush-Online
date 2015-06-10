using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRush.Tests
{
    [TestClass]
    public class UpgradesTest
    {
        [ClassInitialize]
        private Game GetGame()
        {
            return new Game();
        }

        /// <summary>
        /// Copper is worth 5 base. With a 20% increase in value it is worth 6.
        /// </summary>
        [TestMethod]
        public void ItemValueActivate()
        {
            var game = GetGame();
            game.objs.Upgrades.SpeechBuff.Activate();

            Assert.AreEqual(6, game.objs.Items.Copper.Value);
        }

        /// <summary>
        /// Copper is worth 5 base. when activated and then deactivated it should return to normal.
        /// </summary>
        [TestMethod]
        public void ItemValueDeactivate()
        {
            var game = GetGame();
            game.objs.Upgrades.SpeechBuff.Activate();
            game.objs.Upgrades.SpeechBuff.Deactivate();

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }

        [TestMethod]
        public void ResourceActivate()
        {
            var game = GetGame();
            game.objs.Upgrades.Researcher.Activate();

            Assert.IsTrue(game.objs.Gatherers.Miner.PossibleResources.Contains(game.objs.Items.Ruby));
        }

        [TestMethod]
        public void ResourceDeactivate()
        {
            var game = GetGame();
            game.objs.Upgrades.Researcher.Activate();
            game.objs.Upgrades.Researcher.Deactivate();

            Assert.IsTrue(!game.objs.Gatherers.Miner.PossibleResources.Contains(game.objs.Items.Ruby));
        }

        [TestMethod]
        public void BaseEfficiencyActivate()
        {
            var game = GetGame();
            game.objs.Upgrades.ChainsawsT1.Activate();
            game.objs.Gatherers.Lumberjack.Quantity = 1;

            Assert.AreEqual(0.75,game.objs.Gatherers.Lumberjack.ResourcesPerSecond);
        }

        [TestMethod]
        public void BaseEfficiencyDeactivate()
        {
            var game = GetGame();
            game.objs.Upgrades.ChainsawsT1.Activate();
            game.objs.Upgrades.ChainsawsT1.Deactivate();
            game.objs.Gatherers.Lumberjack.Quantity = 1;
            
            Assert.AreEqual(0.5, game.objs.Gatherers.Lumberjack.ResourcesPerSecond);
        }


        [TestMethod]
        public void PercEfficiencyActivate()
        {
            var game = GetGame();
            game.objs.Upgrades.Foreman.Activate();
            game.objs.Gatherers.Miner.Quantity = 1;

            Assert.AreEqual(0.575,game.objs.Gatherers.Miner.ResourcesPerSecond);
        }

        [TestMethod]
        public void PercEfficiencyDeactivate()
        {
            var game = GetGame();
            game.objs.Upgrades.Foreman.Activate();
            game.objs.Upgrades.Foreman.Deactivate();
            game.objs.Gatherers.Miner.Quantity = 1;

            Assert.AreEqual(0.5, game.objs.Gatherers.Miner.ResourcesPerSecond);
        }


        /// <summary>
        /// The Speech Potion lasts 45 seconds. We ensure it is correctly deactivated.
        /// </summary>
        [TestMethod]
        public async Task BuffDecay()
        {
            var game = GetGame();
            game.objs.Upgrades.SpeechBuff.Activate();
            await game.objs.Update(46);

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }

        [TestMethod]
        public void BuffNoDecay()
        {
            var game = GetGame();
            game.objs.Upgrades.SpeechBuff.Activate();
            game.objs.Upgrades.SpeechBuff.Update(44000);

            Assert.AreEqual(6, game.objs.Items.Copper.Value);
        }
    }
}
