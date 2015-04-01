using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using Caroline.Persistence.Models;

namespace GoldRush.APIs
{
    class KomodoSession : IKomodoSession
    {
        Game _game = new Game();

       // private GameState cachedGameState;
        public UpdateDto Update(UpdateArgs args)
        {
            GameState cachedGameState = args != null && args.Session != null ? args.Session.CachedGameState : null;
            var updateResult = _game.Update(args!= null && args.ClientActions != null ? args.ClientActions : null);
            
            // if we have a cached game state compress our game state against it.
            if (cachedGameState != null)
                updateResult = updateResult.Compress(cachedGameState);

            if(args!=null)
                args.Session.CachedGameState = updateResult;

            return new UpdateDto{GameState = updateResult};
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
