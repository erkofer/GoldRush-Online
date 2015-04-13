using Caroline.Persistence.Models;
using Caroline.App.Models;

namespace GoldRush.APIs
{
    public class UpdateArgs
    {
        public ClientActions ClientActions { get; set; }
        public GameSession Session { get; set; }
    }

    public class UpdateDto
    {
        public GameState GameState { get; set; }
        public long Score { get; set; }
    }

    public class SaveDto
    {
        public SaveState SaveState { get; set; }
        
    }

    public class LoadArgs
    {
        public SaveState SaveState { get; set; }
    }
}
