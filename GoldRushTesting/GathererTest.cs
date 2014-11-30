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
        [TestMethod]
        public void GuaranteedCollect()
        {
            Game game = new Game();
            game.objs.Gatherers.Pumpjack.Mine(4000);

            Assert.AreEqual(1, game.objs.Items.Oil.Quantity);
        }

        [TestMethod]
        public void GuaranteeedNoCollect()
        {
            Game game = new Game();
            game.objs.Gatherers.Pumpjack.Mine(1000);

            Assert.AreEqual(0, game.objs.Items.Oil.Quantity);
        }

        [TestMethod]
        public void ChanceCollect()
        {
            Game game=new Game();
            var baseResources = new[]
            {
                game.objs.Items.Stone, game.objs.Items.Copper, game.objs.Items.Iron, game.objs.Items.Silver,
                game.objs.Items.Gold, game.objs.Items.Opal, game.objs.Items.Jade, game.objs.Items.Topaz
            };
            foreach (var resource in baseResources)
                Debug.WriteLine(resource.Quantity);
            

            game.objs.Gatherers.Miner.Mine(10000);
            Assert.AreEqual(5,baseResources.Sum(resource => resource.Quantity));
            
        }

        [TestMethod]
        public void ChanceNoCollect()
        {
            Game game = new Game();
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
            var game = new Game();
            var baseResources = new[]
            {
                game.objs.Items.Stone, game.objs.Items.Copper, game.objs.Items.Iron, game.objs.Items.Silver,
                game.objs.Items.Gold, game.objs.Items.Opal, game.objs.Items.Jade, game.objs.Items.Topaz
            };
            game.objs.Gatherers.Miner.Mine(1000);
            game.objs.Gatherers.Miner.Mine(1000);
            Assert.AreEqual(1, baseResources.Sum(resource => resource.Quantity));
        }
    }
}
