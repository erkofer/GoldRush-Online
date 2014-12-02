namespace GoldRush
{
    public class Game : IGoldRushGame
    {
        public Game()
        {
            objs = new GameObjects();
        }
        public GameObjects objs;


        public OutputState Update(InputState message, UpdateFlags flags)
        {
            // TODO
            return new OutputState();
        }

        public GameSave Save()
        {
            //TODO
            return new GameSave();
        }

        public void Load(GameSave save)
        {
            //TODO
        }
    }
}
