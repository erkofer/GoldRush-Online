using System;

namespace GoldRush
{
    interface IGoldRushGame
    {
        OutputState Update(InputState message, UpdateFlags flags);
        GameSave Save();
        void Load(GameSave save);
    }

    [Flags]
    public enum UpdateFlags
    {
        SendAllState = 1
    }

    internal interface IModifiedState
    {
    }

    internal interface IInput
    {
    }
}
