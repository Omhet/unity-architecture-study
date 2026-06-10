namespace App.Systems.Saving.Modules
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface for per-domain save serialization/deserialization/validation.
    /// Mirrors the IConfigModule pattern used in configuration loading.
    /// Uses SaveDataBundle for strongly typed data access and explicit DTO contracts.
    /// </summary>
    public interface ISaveModule
    {
        /// <summary>
        /// The JSON key this module owns in the save file (e.g., "resources", "progression").
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Serialize current domain state as a typed DTO and store it in the bundle.
        /// Should not contain ReactiveProperty or ObservableCollections.
        /// </summary>
        void Serialize(SaveDataBundle bundle);

        /// <summary>
        /// Deserialize a JSON token section into a typed DTO and store it in the bundle.
        /// Called only if the save JSON contains this module's Key section.
        /// </summary>
        void Deserialize(JToken section, SaveDataBundle bundle);

        /// <summary>
        /// Validate save data before applying. Populates errors list with any issues found.
        /// Reads typed data from the bundle - no side effects on domain state.
        /// </summary>
        void Validate(SaveDataBundle bundle, List<string> errors);

        /// <summary>
        /// Apply validated typed data from the bundle to the domain's runtime state.
        /// Called only after all modules pass validation (transaction semantics).
        /// </summary>
        void Apply(SaveDataBundle bundle);
    }
}
