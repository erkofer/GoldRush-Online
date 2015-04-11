namespace GoldRush.APIs
{
    class KomodoSession : IKomodoSession
    {
        Game _game = new Game();

        // private GameState cachedGameState;
        public UpdateDto Update(UpdateArgs args)
        {
            var cachedGameState = args != null && args.Session != null ? args.Session.CachedGameState : null;
            var fullGameState = _game.Update(args != null && args.ClientActions != null ? args.ClientActions : null);

            if (args != null && args.Session != null)
                args.Session.CachedGameState = fullGameState;

            // if we have a cached game state compress our game state against it.
            var sendState = cachedGameState != null && fullGameState != null
                ? fullGameState.Compress(cachedGameState)
                : fullGameState;

            return new UpdateDto { GameState = sendState };
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
