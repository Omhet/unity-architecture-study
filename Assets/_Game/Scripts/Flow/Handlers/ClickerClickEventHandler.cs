namespace App.Flow.Handlers
{
    using App.Economy.Core;
    using App.Flow.Events;
    using VitalRouter;

    [Routes]
    public partial class ClickerClickEventHandler
    {
        private readonly EconomyService _economyService;

        public ClickerClickEventHandler(EconomyService economyService)
        {
            _economyService = economyService;
        }

        [Route]
        void On(ClickerClickEvent _)
        {
            _economyService.Add(1);
        }
    }
}
