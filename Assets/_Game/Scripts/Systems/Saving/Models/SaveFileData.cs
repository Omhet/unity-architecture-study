namespace App.Systems.Saving.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Root structure of a save file JSON. Contains version, metadata, and per-domain sections.
    /// </summary>
    public class SaveFileData
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("metadata")]
        public SaveMetadata Metadata { get; set; } = new SaveMetadata();

        // Per-domain sections are stored in the raw JSON dictionary during migration,
        // but for serialization we use a flexible approach with ISaveModule keys.
    }

    public class SaveMetadata
    {
        [JsonProperty("lastPlayed")]
        public DateTime LastPlayed { get; set; }
    }
}
