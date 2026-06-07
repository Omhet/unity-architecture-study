namespace App.Systems.Saving.Orchestration
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Subscribes to Application.quitting for a fire-and-forget safety-net save.
    /// Primary saves happen on scene exit (awaited); this catches unexpected closes.
    /// Uses synchronous I/O since we're quitting and can't await.
    /// </summary>
    public class SaveOnQuitSystem
    {
        private readonly SaveLoadSystem _saveLoadSystem;

        public SaveOnQuitSystem(SaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
            Application.quitting += OnApplicationQuitting;
        }

        private void OnApplicationQuitting()
        {
            // Fire-and-forget synchronous save on quit.
            // We can't await here, so we do a best-effort sync write.
            var activeSlot = _saveLoadSystem.GetActiveSlot();

            // Note: SaveLoadSystem.SaveSlotAsync is async, but Application.quitting
            // doesn't support awaiting. This is a safety net - primary saves happen
            // on scene exit which are properly awaited.
            // We fire the task and let it complete as best we can.
            _saveLoadSystem.SaveSlotAsync(activeSlot).Forget();
        }
    }
}
