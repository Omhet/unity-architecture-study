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
        /// Full load pipeline: backup → read → parse JSON → run migrations → deserialize → validate → apply.
        /// Transaction semantics: all modules are validated before any Apply is called.
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

            // Step 3: Parse JSON into JObject for migration and module deserialization
            var saveData = JsonConvert.DeserializeObject<JObject>(json)
                ?? throw new InvalidOperationException($"Failed to parse save data for slot {slotIndex}");

            // Step 4: Run migrations if version mismatch
            int fileVersion;
            var versionToken = saveData["version"];
            if (versionToken != null)
            {
                fileVersion = versionToken.Value<int>();
            }
            else
            {
                throw new InvalidOperationException($"Save data for slot {slotIndex} is missing 'version' field.");
            }

            if (fileVersion < SaveSchemaVersion.Current)
            {
                var chain = _chainBuilder.BuildChain(fileVersion);
                foreach (var migration in chain)
                    migration.Migrate(saveData);

                // Update version after migrations
                saveData["version"] = SaveSchemaVersion.Current;
            }

            // Step 5: Create bundle and deserialize all module sections
            var bundle = new SaveDataBundle();
            foreach (var module in _modules)
            {
                var sectionToken = saveData[module.Key];
                if (sectionToken != null)
                    module.Deserialize(sectionToken, bundle);
                else
                    throw new InvalidOperationException($"Save data for slot {slotIndex} is missing expected section '{module.Key}'. The save file may be corrupted or a migration failed to add this section.");
            }

            // Step 6: Validate all modules - collect ALL errors before any mutation
            var errors = new List<string>();
            foreach (var module in _modules)
            {
                if (bundle.HasData(module.Key))
                    module.Validate(bundle, errors);
                else
                    throw new InvalidOperationException($"Save data bundle is missing expected section '{module.Key}' for validation. The save file may be corrupted or a migration failed to add this section.");
            }

            if (errors.Count > 0)
                throw new InvalidOperationException(
                    $"Save validation failed for slot {slotIndex}: {string.Join("; ", errors)}");

            // Step 7: Apply validated data to domain state (transaction - only after all validations pass)
            foreach (var module in _modules)
            {
                if (bundle.HasData(module.Key))
                    module.Apply(bundle);
            }
        }

        /// <summary>
        /// Full save pipeline: create bundle → collect from all modules → build JObject → write JSON.
        /// </summary>
        public async UniTask SaveSlotAsync(int slotIndex)
        {
            var slotKey = slotIndex.ToString();

            // Create bundle for save pipeline
            var bundle = new SaveDataBundle();

            // Collect from all ISaveModule.Serialize()
            foreach (var module in _modules)
                module.Serialize(bundle);

            // Build JObject from bundle data
            var saveData = new JObject
            {
                ["version"] = SaveSchemaVersion.Current,
                ["metadata"] = new JObject
                {
                    ["lastPlayed"] = DateTime.UtcNow
                }
            };

            foreach (var module in _modules)
            {
                if (bundle.HasData(module.Key))
                {
                    var data = bundle.GetData<object>(module.Key);
                    saveData[module.Key] = JToken.FromObject(data);
                }
                else
                {
                    throw new InvalidOperationException($"Save data bundle is missing expected section '{module.Key}' for serialization. This indicates a module failed to serialize its data.");
                }
            }

            string json = saveData.ToString(Formatting.Indented);
            await _storage.WriteAsync(slotKey, json);
        }
    }
}
