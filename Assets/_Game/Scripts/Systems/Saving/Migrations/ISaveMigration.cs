namespace App.Systems.Saving.Migrations
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for save file schema migrations. Operates on raw JSON (Dictionary&lt;string, object&gt;)
    /// rather than domain objects, so migrations are pure data transforms independent of runtime state.
    /// </summary>
    public interface ISaveMigration
    {
        int FromVersion { get; }
        int ToVersion { get; }

        /// <summary>
        /// Mutate the save data dictionary in place to transform from FromVersion to ToVersion structure.
        /// </summary>
        void Migrate(Dictionary<string, object> saveData);
    }
}
