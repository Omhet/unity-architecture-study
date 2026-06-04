namespace App.Flow.Handlers
{
    using App.Economy.Core;
    using App.Flow.Events;
    using VitalRouter;

    [Routes]
    public partial class StartGameFlowHandler
    {
        private readonly EconomyService _economyService;

        public StartGameFlowHandler(EconomyService economyService)
        {
            _economyService = economyService;
        }

        [Route]
        void On(StartGameEvent _)
        {
            // For testing/demo purposes, give the player some starting currency when the game starts.
            _economyService.Add(10);
        }
    }
}