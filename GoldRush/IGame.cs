using System;

namespace GoldRush
{
    public interface IGoldRushGame
    {
        OutputState Update(InputState message, UpdateFlags flags);
        GameSave Save();
        void Load(GameSave save);
    }

    [Flags]
    public enum UpdateFlags
    {
        ReturnAllState = 1
    }

    public interface IModifiedState
    {
    }

    public interface IInput
    {
    }
}
