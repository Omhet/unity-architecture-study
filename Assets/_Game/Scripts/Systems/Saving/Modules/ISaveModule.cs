#nullable enable

namespace App.Systems.Saving.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for per-domain save serialization/deserialization/validation.
    /// Mirrors the IConfigModule pattern used in configuration loading.
    /// </summary>
    public interface ISaveModule
    {
        /// <summary>
        /// The JSON key this module owns in the save file (e.g., "resources", "progression").
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Serialize current domain state to a plain serializable object.
        /// Should not contain ReactiveProperty or ObservableCollections.
        /// </summary>
        object? Serialize();

        /// <summary>
        /// Deserialize plain data and apply it to the domain's runtime state.
        /// Called only if the save JSON contains this module's Key section.
        /// </summary>
        void Deserialize(object data);

        /// <summary>
        /// Validate save data before applying. Populates errors list with any issues found.
        /// </summary>
        void Validate(object data, List<string> errors);
    }
}
