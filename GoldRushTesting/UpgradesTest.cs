using System;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRushTesting
{
    [TestClass]
    public class UpgradesTest
    {
        /// <summary>
        /// Copper is worth 5 base. With a 100% increase in value it is worth 10.
        /// </summary>
        [TestMethod]
        public void ItemValueActivate()
        {
            Game game = new Game();
            game.objs.Upgrades.ExpensiveItems.Activate();

            Assert.AreEqual(10, game.objs.Items.Copper.Value);
        }

        /// <summary>
        /// Copper is worth 5 base. when activated and then deactivated it should return to normal.
        /// </summary>
        [TestMethod]
        public void ItemValueDeactivate()
        {
            Game game = new Game();
            game.objs.Upgrades.ExpensiveItems.Activate();
            game.objs.Upgrades.ExpensiveItems.Deactivate();

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }
    }
}
