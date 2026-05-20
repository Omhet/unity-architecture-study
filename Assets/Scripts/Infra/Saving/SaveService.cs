namespace Infra.Saving
{
    using System.IO;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;

    public class SaveService
    {
        private readonly string _saveFilePath;

        public SaveService()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, "GameState.json");
        }

        public async UniTask<T> LoadAsync<T>() where T : new()
        {
            if (!File.Exists(_saveFilePath)) return new T();

            string json = await File.ReadAllTextAsync(_saveFilePath);
            return JsonConvert.DeserializeObject<T>(json) ?? new T();
        }

        public async UniTask SaveAsync<T>(T state)
        {
            string json = JsonConvert.SerializeObject(state, Formatting.Indented);
            await File.WriteAllTextAsync(_saveFilePath, json);
        }
    }
}
