namespace App.Systems.Saving.Migrations
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface for save file schema migrations. Operates on JObject
    /// rather than domain objects, so migrations are pure data transforms independent of runtime state.
    /// </summary>
    public interface ISaveMigration
    {
        int FromVersion { get; }
        int ToVersion { get; }

        /// <summary>
        /// Mutate the save data JObject in place to transform from FromVersion to ToVersion structure.
        /// </summary>
        void Migrate(JObject saveData);
    }
}
