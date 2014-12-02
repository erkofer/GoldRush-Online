using System;
using JetBrains.Annotations;

namespace GoldRush
{
    public interface IGoldRushGame
    {
        /// <summary>
        /// Updates the game, returning its new state.
        /// </summary>
        /// <param name="message">The input to process</param>
        /// <param name="flags">The flags</param>
        /// <returns>An OutputState. Can be null if no state was modified.</returns>
        [CanBeNull]
        OutputState Update([CanBeNull]InputState message, UpdateFlags flags);
        /// <summary>
        /// Returns all state required to resume a save at a later date.
        /// </summary>
        /// <returns>A GameSave representing all unique internal and external state of a game.</returns>
        [NotNull]
        GameSave Save();
        /// <summary>
        /// Loads a game from a GameSave.
        /// </summary>
        /// <param name="save">The state to use. Null to reset the game.</param>
        void Load([CanBeNull]GameSave save);
    }

    [Flags]
    public enum UpdateFlags
    {
        ReturnAllState = 1
    }
}
