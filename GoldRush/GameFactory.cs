namespace GoldRush
{
    public class GameFactory
    {
        private IGoldRushGame template;

        public IGoldRushGame Create()
        {
            return template ?? (template = new Game());
        }
    }
}
