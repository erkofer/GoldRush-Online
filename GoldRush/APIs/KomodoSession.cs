using System.Threading.Tasks;
using Caroline.Domain.Models;
using MongoDB.Driver;

namespace GoldRush.APIs
{
    class KomodoSession : IKomodoSession
    {
        Game _game = new Game();

        // private GameState cachedGameState;
        public async Task<UpdateDto> Update(UpdateArgs args)
        {
            var cachedGameState = args.Session.CachedGameState;
            var fullGameState = await _game.Update(args.ClientActions, args.MarketPlace, args.User);

            args.Session.CachedGameState = fullGameState;

            // if we have a cached game state compress our game state against it.
            var sendState = cachedGameState != null
                ? fullGameState.Compress(cachedGameState)
                : fullGameState;
            /*
                    var mp = args.MarketPlace;
                    var orders = await mp.GetOrders(123);
                    var ordersList = await orders.ToListAsync();
                    var orderone = ordersList[0];
            
                    // if moneyGained is null, order doesnt exist in db
                    long? moneyGained = 0;
                    if (orderone.UnclaimedMoneyRecieved != 0)
                        moneyGained = await mp.ClaimOrderContents(orderone.Id, ClaimField.Money);*/

            return new UpdateDto { GameState = sendState, Score = _game.Score };
        }

        public SaveDto Save()
        {
            var saveState = _game.Save();
            return new SaveDto { SaveState = saveState };
        }

        public void Load(LoadArgs args)
        {
            if (args != null)
            {
                var saveState = args.SaveState;

                if (saveState != null)
                    _game.Load(args.SaveState);
            }
        }
    }
}
