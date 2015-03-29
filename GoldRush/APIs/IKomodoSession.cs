using JetBrains.Annotations;

namespace GoldRush.APIs
{
    public interface IKomodoSession
    {
        /// <summary>
        /// Updates the game, returning its new state.
        /// </summary>
        /// <param name="args">The input to process</param>
        /// <param name="flags">The flags</param>
        /// <returns>An OutputState. Can be null if no state was modified.</returns>
        [CanBeNull]
        UpdateDto Update([CanBeNull]UpdateArgs args);
        /// <summary>
        /// Returns all state required to resume a args at a later date.
        /// </summary>
        /// <returns>A GameSave representing all unique internal and external state of a game.</returns>
        [NotNull]
        SaveDto Save();
        /// <summary>
        /// Loads a game from a GameSave.
        /// </summary>
        /// <param name="args">The state to use. Null to reset the game.</param>
        void Load([CanBeNull]LoadArgs args);
    }
}
