using Caroline.App.Models;
using JetBrains.Annotations;

namespace Caroline.App
{
    public interface IGoldRushCache
    {
        [CanBeNull]
        GameState GetGameData([NotNull] string sessionGuid);

        void SetGameData([NotNull] string sessionGuid, [NotNull] GameState state);
    }
}