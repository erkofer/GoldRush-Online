﻿using System;
using System.Security.AccessControl;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRushTesting
{
    [TestClass]
    public class ItemsTest
    {

        private Game _game;

        [ClassInitialize]
        private Game GetGame()
        {
            return _game ?? (_game = new Game());
        }

        [TestMethod]
        public void CraftWithPrerequisites()
        {
            var game = GetGame();
            game.objs.Items.Copper.Quantity = 1000;
            game.objs.Items.BronzeBar.Quantity = 10;

            game.objs.Items.CopperWire.Craft();
            Assert.AreEqual(100,game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void CraftWithoutPrerequisites()
        {
            var game = GetGame();
            game.objs.Items.CopperWire.Craft();
            Assert.AreEqual(0,game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void CraftManyWithPrerequisites()
        {
            var game = GetGame();
            game.objs.Items.Copper.Quantity = 5000;
            game.objs.Items.BronzeBar.Quantity = 50;
            game.objs.Items.CopperWire.Craft(5);
         

            Assert.AreEqual(500, game.objs.Items.CopperWire.Quantity);
            Assert.AreEqual(0,game.objs.Items.BronzeBar.Quantity);
            Assert.AreEqual(0, game.objs.Items.Copper.Quantity);
        }

        [TestMethod]
        public void CraftManyWithoutPrerequisites()
        {
            var game = GetGame();
            game.objs.Items.CopperWire.Craft(5);

            Assert.AreEqual(0, game.objs.Items.CopperWire.Quantity);
        }

        [TestMethod]
        public void TotalQuantity()
        {
            var game = GetGame();
            game.objs.Items.Stone.Quantity = 5;
            game.objs.Items.Stone.Quantity = 0;

            Assert.AreEqual(5,game.objs.Items.Stone.PrestigeTimeTotal);
        }

        [TestMethod]
        public void ItemSell()
        {
            var game = GetGame();
            game.objs.Items.Copper.Quantity = 5;
            game.objs.Items.Copper.Sell(5);

            Assert.AreEqual(25,game.objs.Items.Coins.Quantity);
        }

        [TestMethod]
        public void ItemNoSell()
        {
            var game = GetGame();
            game.objs.Items.Diamond.Sell();

            Assert.AreEqual(0,game.objs.Items.Coins.Quantity);
        }

        [TestMethod]
        public void PotionConsume()
        {
            var game = GetGame();
            game.objs.Items.SpeechPotion.Quantity = 1;
            game.objs.Items.SpeechPotion.Consume();

            Assert.AreEqual(6, game.objs.Items.Copper.Value);
        }

        [TestMethod]
        public void PotionDoNotConsume()
        {
            var game = GetGame();
            game.objs.Items.SpeechPotion.Consume();

            Assert.AreEqual(5, game.objs.Items.Copper.Value);
        }
    }
}