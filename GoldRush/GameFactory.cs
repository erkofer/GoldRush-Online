using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    public class GameFactory
    {
        IGoldRushGame Create()
        {
            // 02/12/2014: Hunter, if you have readonly state that you want to share between games,
            // this would be the place to inject it.
            return new Game();
        }
    }
}
