#nullable enable

namespace App.Systems.Saving.Orchestration
{
    using System;
    using System.Collections.Generic;
    using App.Systems.Saving.Models;
    using App.Systems.Saving.Storage;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Data service for slot management operations: list, load, save, delete.
    /// Delegates actual load/save through SaveLoadSystem.
    /// </summary>
    public class SlotManager
    {
        private readonly ISaveStorage _storage;
        private readonly SaveLoadSystem _saveLoadSystem;
        private readonly int _slotCount;

        public SlotManager(ISaveStorage storage, SaveLoadSystem saveLoadSystem, int slotCount = 4)
        {
            _storage = storage;
            _saveLoadSystem = saveLoadSystem;
            _slotCount = slotCount;
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
        /// Load a slot by delegating to SaveLoadSystem.
        /// </summary>
        public async UniTask LoadSlotAsync(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slotCount)
                throw new ArgumentOutOfRangeException(nameof(slotIndex), $"Slot index must be between 0 and {_slotCount - 1}");

            _saveLoadSystem.SetActiveSlot(slotIndex);
            await _saveLoadSystem.LoadSlotAsync(slotIndex);
        }

        /// <summary>
        /// Save the active slot by delegating to SaveLoadSystem.
        /// </summary>
        public async UniTask SaveActiveSlotAsync()
        {
            await _saveLoadSystem.SaveSlotAsync(_saveLoadSystem.GetActiveSlot());
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
