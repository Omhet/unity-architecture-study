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

        public async UniTask<string?> ReadAsync(string slotKey)
        {
            var path = GetSlotPath(slotKey);
            if (!File.Exists(path)) return null;

            return await File.ReadAllTextAsync(path);
        }

        public async UniTask WriteAsync(string slotKey, string json)
        {
            var path = GetSlotPath(slotKey);
            await File.WriteAllTextAsync(path, json);
        }

        public async UniTask DeleteAsync(string slotKey)
        {
            var path = GetSlotPath(slotKey);
            if (File.Exists(path))
                await TaskExtensions.DeleteFileAsync(path);

            var backupPath = GetBackupPath(slotKey);
            if (File.Exists(backupPath))
                await TaskExtensions.DeleteFileAsync(backupPath);
        }

        public async UniTask<List<string>> ListSlotsAsync()
        {
            await UniTask.SwitchToThreadPool();

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
            return slots;
        }

        public async UniTask CopyToBackupAsync(string slotKey)
        {
            var sourcePath = GetSlotPath(slotKey);
            if (!File.Exists(sourcePath)) return;

            var backupPath = GetBackupPath(slotKey);
            await TaskExtensions.CopyFileAsync(sourcePath, backupPath, overwrite: true);
        }

        private string GetSlotPath(string slotKey) => Path.Combine(_saveDirectory, $"slot_{slotKey}.json");
        private string GetBackupPath(string slotKey) => Path.Combine(_backupDirectory, $"slot_{slotKey}_backup.json");
    }

    // Helper for File.DeleteAsync and File.CopyAsync which aren't available in all Unity API compat levels
    static class TaskExtensions
    {
        public static async UniTask DeleteFileAsync(string path)
        {
            await UniTask.SwitchToThreadPool();
            File.Delete(path);
        }

        public static async UniTask CopyFileAsync(string source, string dest, bool overwrite)
        {
            await UniTask.SwitchToThreadPool();
            File.Copy(source, dest, overwrite);
        }
    }
}
