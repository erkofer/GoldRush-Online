using System.Threading.Tasks;

namespace GoldRush.APIs
{
    class KomodoSession : IKomodoSession
    {
        Game _game = new Game();

        // private GameState cachedGameState;
        public async Task<UpdateDto> Update(UpdateArgs args)
        {
            var cachedGameState = args.Session.CachedGameState;
            var fullGameState = _game.Update(args.ClientActions);

                args.Session.CachedGameState = fullGameState;

            // if we have a cached game state compress our game state against it.
            var sendState = cachedGameState != null
                ? fullGameState.Compress(cachedGameState)
                : fullGameState;

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
