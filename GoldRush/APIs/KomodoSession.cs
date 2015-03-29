using Caroline.App.Models;

namespace GoldRush.APIs
{
    class KomodoSession : IKomodoSession
    {
        Game _game = new Game();
        public UpdateDto Update(UpdateArgs args)
        {
            GameState updateResult = null;

            if (args != null)
            {
                var actions = args.ClientActions;
                if (actions != null)
                    updateResult = _game.Update(actions);
            }

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
