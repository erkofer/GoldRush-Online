using System;
using System.Security.AccessControl;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRushTesting
{
    [TestClass]
    public class ItemsTest
    {
        [TestMethod]
        public void RecipeWithPrerequisites()
        {
            Game game = new Game();
            game.objs.Items.Copper.Quantity = 1000;
            game.objs.Items.BronzeBar.Quantity = 10;

            game.objs.Items.CopperWire.Craft();
            Assert.AreEqual(100,game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void RecipeWithoutPrerequisites()
        {
            Game game = new Game();
            game.objs.Items.CopperWire.Craft();
            Assert.AreEqual(0,game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void RecipeManyWithPrerequisites()
        {
            Game game = new Game();
            game.objs.Items.Copper.Quantity = 5000;
            game.objs.Items.BronzeBar.Quantity = 50;
            game.objs.Items.CopperWire.Craft(5);

            Assert.AreEqual(500, game.objs.Items.CopperWire.Quantity);
            Assert.AreEqual(0,game.objs.Items.BronzeBar.Quantity);
            Assert.AreEqual(0, game.objs.Items.Copper.Quantity);
        }

        [TestMethod]
        public void RecipeManyWithoutPrerequisites()
        {
            Game game = new Game();
            game.objs.Items.CopperWire.Craft(5);

            Assert.AreEqual(0, game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void TotalQuantity()
        {
            Game game = new Game();
            game.objs.Items.Stone.Quantity = 5;
            game.objs.Items.Stone.Quantity = 0;

            Assert.AreEqual(5,game.objs.Items.Stone.PrestigeTimeTotal);
        }

        [TestMethod]
        public void SellItem()
        {
            Game game = new Game();
            game.objs.Items.Copper.Quantity = 5;
            game.objs.Items.Copper.Sell(5);

            Assert.AreEqual(25,game.objs.Items.Coins.Quantity);
        }

        [TestMethod]
        public void DoNotSellItem()
        {
            Game game = new Game();
            game.objs.Items.Diamond.Sell();

            Assert.AreEqual(0,game.objs.Items.Coins.Quantity);
        }
    }
}
