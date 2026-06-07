#nullable enable

namespace App.Systems.Saving.Orchestration
{
    using System;
    using System.Collections.Generic;
    using App.Systems.Saving.Migrations;
    using App.Systems.Saving.Modules;
    using App.Systems.Saving.Storage;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Orchestrates the complete save/load pipeline: backup, read, migrate, validate, deserialize.
    /// Mirrors the config system's loader → validator → hydrator pattern.
    /// </summary>
    public class SaveLoadSystem
    {
        private readonly ISaveStorage _storage;
        private readonly IEnumerable<ISaveModule> _modules;
        private readonly MigrationChainBuilder _chainBuilder;

        public SaveLoadSystem(
            ISaveStorage storage,
            IEnumerable<ISaveModule> modules,
            MigrationChainBuilder chainBuilder)
        {
            _storage = storage;
            _modules = modules;
            _chainBuilder = chainBuilder;
        }

        /// <summary>
        /// Full load pipeline: backup → read → parse JSON → run migrations → validate → deserialize.
        /// </summary>
        public async UniTask LoadSlotAsync(int slotIndex)
        {
            var slotKey = slotIndex.ToString();

            // Step 1: Backup current file before loading
            await _storage.CopyToBackupAsync(slotKey);

            // Step 2: Read JSON from storage
            string? json = await _storage.ReadAsync(slotKey);
            if (string.IsNullOrEmpty(json))
                return; // Empty slot - no-op, domain states retain defaults

            // Step 3: Parse JSON into raw dictionary for migration
            var saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)
                ?? throw new InvalidOperationException($"Failed to parse save data for slot {slotIndex}");

            // Step 4: Run migrations if version mismatch
            int fileVersion;
            if (saveData.TryGetValue("version", out var versionObj) && versionObj is JsonToken token && token == JsonToken.Integer)
                fileVersion = Convert.ToInt32(saveData["version"]);
            else
                fileVersion = 0; // Assume oldest version if missing

            if (fileVersion < SaveSchemaVersion.Current)
            {
                var chain = _chainBuilder.BuildChain(fileVersion);
                foreach (var migration in chain)
                    migration.Migrate(saveData);

                // Update version in the dictionary after migrations
                saveData["version"] = SaveSchemaVersion.Current;
            }

            // Step 5: Validate all domain sections
            var errors = new List<string>();
            foreach (var module in _modules)
            {
                if (saveData.TryGetValue(module.Key, out var sectionData))
                    module.Validate(sectionData!, errors);
            }

            if (errors.Count > 0)
                throw new InvalidOperationException(
                    $"Save validation failed for slot {slotIndex}: {string.Join("; ", errors)}");

            // Step 6: Dispatch to matching ISaveModule.Deserialize()
            foreach (var module in _modules)
            {
                if (saveData.TryGetValue(module.Key, out var sectionData))
                    module.Deserialize(sectionData!);
            }
        }

        /// <summary>
        /// Full save pipeline: collect from all modules → merge with version + metadata → write JSON.
        /// </summary>
        public async UniTask SaveSlotAsync(int slotIndex)
        {
            var slotKey = slotIndex.ToString();

            // Build save data dictionary
            var saveData = new Dictionary<string, object>
            {
                ["version"] = SaveSchemaVersion.Current,
                ["metadata"] = new JObject
                {
                    ["lastPlayed"] = DateTime.UtcNow
                }
            };

            // Collect from all ISaveModule.Serialize()
            foreach (var module in _modules)
            {
                var serialized = module.Serialize();
                if (serialized != null)
                    saveData[module.Key] = serialized;
            }

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            await _storage.WriteAsync(slotKey, json);
        }
    }
}
