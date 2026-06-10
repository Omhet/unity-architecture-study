namespace App.Systems.Saving.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Type-safe container for save data sections
    /// Each module stores and retrieves its own typed DTO by key.
    /// </summary>
    public class SaveDataBundle
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// Store typed data under the given key. Overwrites any existing value for that key.
        /// </summary>
        public void SetData<T>(string key, T data)
        {
            _data[key] = data;
        }

        /// <summary>
        /// Retrieve typed data for the given key. Throws if the key does not exist.
        /// </summary>
        public T GetData<T>(string key)
        {
            if (!_data.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"Save data bundle has no entry for key '{key}'. The save file may be corrupted or a migration failed to add this section.");

            return (T)value;
        }

        /// <summary>
        /// Check whether data exists for the given key.
        /// </summary>
        public bool HasData(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
