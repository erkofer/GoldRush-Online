using System;
using System.Collections;
using System.Collections.Generic;
using Caroline.Models;

namespace Caroline.Dal
{
    interface IGameRepository : IDisposable
    {
        IEnumerable<Game> GetGames();
    }
}
