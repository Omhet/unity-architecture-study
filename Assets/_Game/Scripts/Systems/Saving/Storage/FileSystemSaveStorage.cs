#nullable enable

namespace App.Systems.Saving.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class FileSystemSaveStorage : ISaveStorage
    {
        private readonly string _saveDirectory;
        private readonly string _backupDirectory;

        public FileSystemSaveStorage()
        {
            _saveDirectory = Path.Combine(Application.persistentDataPath, "saves");
            _backupDirectory = Path.Combine(_saveDirectory, "backups");

            Directory.CreateDirectory(_saveDirectory);
            Directory.CreateDirectory(_backupDirectory);
        }

        public UniTask<string?> ReadAsync(string slotKey)
        {
            var path = GetSlotPath(slotKey);
            return UniTask.FromResult(File.Exists(path) ? File.ReadAllText(path) : null);
        }

        public UniTask WriteAsync(string slotKey, string json)
        {
            var path = GetSlotPath(slotKey);
            File.WriteAllText(path, json);
            return default;
        }

        public UniTask DeleteAsync(string slotKey)
        {
            var path = GetSlotPath(slotKey);
            if (File.Exists(path))
                File.Delete(path);

            var backupPath = GetBackupPath(slotKey);
            if (File.Exists(backupPath))
                File.Delete(backupPath);

            return default;
        }

        public UniTask<List<string>> ListSlotsAsync()
        {
            var slots = new List<string>();
            var files = Directory.GetFiles(_saveDirectory, "slot_*.json");

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                if (name.StartsWith("slot_") && name.EndsWith(".json"))
                {
                    var key = name.Substring(5, name.Length - 9);
                    slots.Add(key);
                }
            }

            slots.Sort();
            return UniTask.FromResult(slots);
        }

        public UniTask CopyToBackupAsync(string slotKey)
        {
            var sourcePath = GetSlotPath(slotKey);
            if (!File.Exists(sourcePath)) return default;

            var backupPath = GetBackupPath(slotKey);
            File.Copy(sourcePath, backupPath, overwrite: true);
            return default;
        }

        private string GetSlotPath(string slotKey) => Path.Combine(_saveDirectory, $"slot_{slotKey}.json");
        private string GetBackupPath(string slotKey) => Path.Combine(_backupDirectory, $"slot_{slotKey}_backup.json");
    }
}
