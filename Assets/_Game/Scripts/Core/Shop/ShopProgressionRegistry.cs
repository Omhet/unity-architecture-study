namespace App.Shop.Core
{
    using System.Collections.Generic;

    public class ShopProgressionRegistry
    {
        private readonly List<ShopProgressionEntry> _entries = new List<ShopProgressionEntry>();

        public void Load(ShopProgressionEntry[] entries)
        {
            _entries.Clear();

            if (entries == null)
            {
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                _entries.Add(entries[i]);
            }
        }

        public List<string> GetUnlockedUpToLevel(int level)
        {
            var result = new List<string>();

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry.Level <= level && entry.ShopItemIds != null)
                {
                    for (int j = 0; j < entry.ShopItemIds.Length; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(entry.ShopItemIds[j]))
                        {
                            result.Add(entry.ShopItemIds[j]);
                        }
                    }
                }
            }

            return result;
        }

        public List<string> GetNewAtLevel(int level)
        {
            var result = new List<string>();

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry.Level == level && entry.ShopItemIds != null)
                {
                    for (int j = 0; j < entry.ShopItemIds.Length; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(entry.ShopItemIds[j]))
                        {
                            result.Add(entry.ShopItemIds[j]);
                        }
                    }
                }
            }

            return result;
        }
    }
}
