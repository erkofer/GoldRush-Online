using System;
using System.Diagnostics;
using System.Linq;
using GoldRush;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldRushTesting
{
    [TestClass]
    public class GathererTest
    {
        private Game _game;

        [ClassInitialize]
        private Game GetGame()
        {
            return _game ?? (_game = new Game());
        }

        [TestMethod]
        public void GuaranteedCollect()
        {
            var game = GetGame();
            game.objs.Gatherers.Pumpjack.Quantity = 1;
            game.objs.Gatherers.Pumpjack.Mine(4000);

            Assert.AreEqual(1, game.objs.Items.Oil.Quantity);
        }

        [TestMethod]
        public void GuaranteeedNoCollect()
        {
            var game = GetGame();
            game.objs.Gatherers.Pumpjack.Mine(1000);

            Assert.AreEqual(0, game.objs.Items.Oil.Quantity);
        }

        [TestMethod]
        public void ChanceCollect()
        {
            var game = GetGame();
            
            var baseResources = new[]
            {
                game.objs.Items.Stone, game.objs.Items.Copper, game.objs.Items.Iron, game.objs.Items.Silver,
                game.objs.Items.Gold, game.objs.Items.Opal, game.objs.Items.Jade, game.objs.Items.Topaz
            };

            game.objs.Gatherers.Miner.Quantity = 1;
            game.objs.Gatherers.Miner.Mine(10000);
            Assert.AreEqual(5,baseResources.Sum(resource => resource.Quantity));
            
        }

        [TestMethod]
        public void ChanceNoCollect()
        {
            var game = GetGame();
            var baseResources = new[]
            {
                game.objs.Items.Stone, game.objs.Items.Copper, game.objs.Items.Iron, game.objs.Items.Silver,
                game.objs.Items.Gold, game.objs.Items.Opal, game.objs.Items.Jade, game.objs.Items.Topaz
            };
            game.objs.Gatherers.Miner.Mine(1999);
            Assert.AreEqual(0, baseResources.Sum(resource => resource.Quantity));
        }

        [TestMethod]
        public void ResourceBuffer()
        {
            var game = GetGame();
            var baseResources = new[]
            {
                game.objs.Items.Stone, game.objs.Items.Copper, game.objs.Items.Iron, game.objs.Items.Silver,
                game.objs.Items.Gold, game.objs.Items.Opal, game.objs.Items.Jade, game.objs.Items.Topaz
            };
            game.objs.Gatherers.Miner.Quantity = 1;
            game.objs.Gatherers.Miner.Mine(1000);
            game.objs.Gatherers.Miner.Mine(1000);
            Assert.AreEqual(1, baseResources.Sum(resource => resource.Quantity));
        }
    }
}
