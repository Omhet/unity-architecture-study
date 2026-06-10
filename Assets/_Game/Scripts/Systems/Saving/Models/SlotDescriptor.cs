namespace App.Systems.Saving.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Describes a save slot for listing purposes without loading full save data.
    /// </summary>
    public class SlotDescriptor
    {
        [JsonProperty("slotIndex")]
        public int SlotIndex { get; set; }

        [JsonProperty("hasData")]
        public bool HasData { get; set; }

        [JsonProperty("lastPlayed")]
        public DateTime? LastPlayed { get; set; }

        /// <summary>
        /// Human-readable summary for UI display.
        /// </summary>
        public string Summary => HasData
            ? $"Slot {SlotIndex} - Last played: {LastPlayed?.ToString("g") ?? "Unknown"}"
            : $"Slot {SlotIndex} - Empty";
    }
}
