#nullable enable

namespace App.Systems.Saving.Orchestration
{
    using System;
    using System.Collections.Generic;
    using App.Systems.Saving.Models;
    using App.Systems.Saving.Options;
    using App.Systems.Saving.Storage;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Manages slot state: slot count (from boot config), active slot (mutable runtime state).
    /// Provides slot listing and deletion. Load/save pipeline is handled by SaveLoadSystem.
    /// </summary>
    public class SlotManager
    {
        private readonly ISaveStorage _storage;
        private readonly int _slotCount;
        private int _activeSlot;

        public SlotManager(ISaveStorage storage, SaveBootstrapOptions options)
        {
            _storage = storage;
            _slotCount = options.SlotCount;
        }

        public int GetActiveSlot() => _activeSlot;

        public void SetActiveSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slotCount)
                throw new ArgumentOutOfRangeException(nameof(slotIndex), $"Slot index must be between 0 and {_slotCount - 1}");
            _activeSlot = slotIndex;
        }

        /// <summary>
        /// List all slots with metadata without loading full save data.
        /// </summary>
        public async UniTask<SlotDescriptor[]> ListSlotsAsync()
        {
            var descriptors = new List<SlotDescriptor>();

            for (int i = 0; i < _slotCount; i++)
            {
                var slotKey = i.ToString();
                string? json = await _storage.ReadAsync(slotKey);

                bool hasData = !string.IsNullOrEmpty(json);
                DateTime? lastPlayed = null;

                if (hasData)
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json!);
                        if (data?.TryGetValue("metadata", out var metaObj) == true && metaObj is JObject metaData)
                        {
                            lastPlayed = metaData.Value<DateTime?>("lastPlayed");
                        }
                    }
                    catch
                    {
                        // Corrupted JSON - still show slot exists but with no metadata
                    }
                }

                descriptors.Add(new SlotDescriptor
                {
                    SlotIndex = i,
                    HasData = hasData,
                    LastPlayed = lastPlayed
                });
            }

            return descriptors.ToArray();
        }

        /// <summary>
        /// Delete a slot, removing both main file and backup.
        /// </summary>
        public async UniTask DeleteSlotAsync(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slotCount)
                throw new ArgumentOutOfRangeException(nameof(slotIndex), $"Slot index must be between 0 and {_slotCount - 1}");

            var slotKey = slotIndex.ToString();
            await _storage.DeleteAsync(slotKey);
        }
    }
}
