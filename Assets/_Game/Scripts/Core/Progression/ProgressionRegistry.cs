namespace App.Progression.Core
{
    using System.Collections.Generic;

    public class ProgressionRegistry
    {
        private readonly List<ProgressionEntry> _entries = new List<ProgressionEntry>();

        public void Load(ProgressionEntry[] entries)
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

            _entries.Sort((a, b) => a.Level.CompareTo(b.Level));
        }

        public int GetNextLevelXpForLevel(int level)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Level == level)
                {
                    return _entries[i].NextLevelXp;
                }
            }

            // No entry found for this level (no further levels defined)
            return 0;
        }
    }
}
