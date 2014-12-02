using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GoldRush
{
    public class Game
    {
        public Game()
        {
            objs = new GameObjects();
        }
        public GameObjects objs;
    

    }
}
