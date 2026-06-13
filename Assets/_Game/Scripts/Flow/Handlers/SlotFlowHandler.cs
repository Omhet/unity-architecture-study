namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Systems.Saving.Orchestration;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class SlotFlowHandler
    {
        private readonly SlotManager _slotManager;
        private readonly ICommandPublisher _publisher;

        public SlotFlowHandler(
            SlotManager slotManager,
            ICommandPublisher publisher)
        {
            _slotManager = slotManager;
            _publisher = publisher;
        }

        [Route]
        async UniTask On(SelectSlotEvent e)
        {
            _slotManager.SetActiveSlot(e.SlotIndex);
            await _publisher.PublishAsync(new LoadGameEvent());
        }

        [Route]
        async UniTask On(DeleteSlotEvent e)
        {
            await _slotManager.DeleteSlotAsync(e.SlotIndex);
        }
    }
}
