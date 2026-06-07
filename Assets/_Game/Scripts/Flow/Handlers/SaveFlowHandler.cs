namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Systems.Saving.Orchestration;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class SaveFlowHandler
    {
        private readonly SlotManager _slotManager;
        private readonly SaveLoadSystem _saveLoadSystem;

        public SaveFlowHandler(
            SlotManager slotManager,
            SaveLoadSystem saveLoadSystem)
        {
            _slotManager = slotManager;
            _saveLoadSystem = saveLoadSystem;
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(ManualSaveEvent _)
        {
            int activeSlot = _slotManager.GetActiveSlot();
            await _saveLoadSystem.SaveSlotAsync(activeSlot);
        }
    }
}
