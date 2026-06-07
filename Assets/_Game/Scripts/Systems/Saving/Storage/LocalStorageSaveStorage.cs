#nullable enable

namespace App.Systems.Saving.Storage
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class LocalStorageSaveStorage : ISaveStorage
    {
        private const string SlotPrefix = "save_slot_";
        private const string BackupPrefix = "save_backup_";

        public UniTask<string?> ReadAsync(string slotKey)
        {
            var value = PlayerPrefs.GetString(SlotPrefix + slotKey, "");
            return UniTask.FromResult(string.IsNullOrEmpty(value) ? (string?)null : value);
        }

        public UniTask WriteAsync(string slotKey, string json)
        {
            PlayerPrefs.SetString(SlotPrefix + slotKey, json);
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        public UniTask DeleteAsync(string slotKey)
        {
            PlayerPrefs.DeleteKey(SlotPrefix + slotKey);
            PlayerPrefs.DeleteKey(BackupPrefix + slotKey);
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        public UniTask<List<string>> ListSlotsAsync()
        {
            // WebGL: we track slots by trying known indices
            var slots = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                var value = PlayerPrefs.GetString(SlotPrefix + i.ToString(), "");
                if (!string.IsNullOrEmpty(value))
                    slots.Add(i.ToString());
            }
            return UniTask.FromResult(slots);
        }

        public UniTask CopyToBackupAsync(string slotKey)
        {
            var value = PlayerPrefs.GetString(SlotPrefix + slotKey, "");
            if (!string.IsNullOrEmpty(value))
                PlayerPrefs.SetString(BackupPrefix + slotKey, value);

            return UniTask.CompletedTask;
        }
    }
}
