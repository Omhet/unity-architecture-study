#nullable enable

namespace App.Systems.Saving.Storage
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface ISaveStorage
    {
        UniTask<string?> ReadAsync(string slotKey);
        UniTask WriteAsync(string slotKey, string json);
        UniTask DeleteAsync(string slotKey);
        UniTask<List<string>> ListSlotsAsync();
        UniTask CopyToBackupAsync(string slotKey);
    }
}
