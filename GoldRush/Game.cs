using Caroline.App.Models;

namespace GoldRush
{
    class Game : IGoldRushGame
    {
        public Game()
        {
            objs = new GameObjects();
        }
        public GameObjects objs;


        public GameState Update(ClientActions message)
        {
            // TODO
            return new GameState();
        }

        public SaveState Save()
        {
            //TODO
            return new SaveState();
        }

        public void Load(SaveState save)
        {
            //TODO
        }
    }
}
