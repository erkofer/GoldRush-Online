namespace GoldRush
{
    public class GameFactory
    {
        private IGoldRushGame template;

        public IGoldRushGame Create()
        {
            return template ?? (template = new Game());
        }

        /*private IGoldRushGame NewGame()
        {
            var game = new Game();
            Items(game);
            return game;
        }

        // Am I doing it right?
        private void Items(Game game)
        {
            game.objs.Items.Copper.Worth = 1;
        }*/
    }
}
