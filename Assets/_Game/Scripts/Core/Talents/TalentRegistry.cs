namespace App.Talents.Core
{
    using System.Collections.Generic;

    public class TalentRegistry
    {
        private readonly List<TalentEntry> _talents = new List<TalentEntry>();

        public void Load(TalentEntry[] entries)
        {
            _talents.Clear();
            if (entries == null) return;

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.Id)) continue;

                _talents.Add(entry);
            }
        }

        public bool TryGetById(string talentId, out TalentEntry talent)
        {
            talent = null;
            for (int i = 0; i < _talents.Count; i++)
            {
                var current = _talents[i];
                if (current != null && current.Id == talentId)
                {
                    talent = current;
                    return true;
                }
            }
            return false;
        }

        public int Count => _talents.Count;
        public TalentEntry this[int index] => _talents[index];
    }
}
