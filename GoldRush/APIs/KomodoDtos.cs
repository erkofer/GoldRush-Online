using Caroline.Persistence.Models;
using Caroline.App.Models;
using GoldRush.Market;

namespace GoldRush.APIs
{
    public class UpdateArgs
    {
        public ClientActions ClientActions { get; set; }
        public GameSession Session { get; set; }
        public IMarketPlace MarketPlace { get; set; }
        public User User { get; set; }
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
