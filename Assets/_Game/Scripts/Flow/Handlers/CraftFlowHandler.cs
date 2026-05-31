namespace App.Flow.Handlers
{
    using App.Craft.Core;
    using App.Flow.Events;
    using VitalRouter;

    [Routes]
    public partial class CraftFlowHandler
    {
        private readonly CraftService _craftService;

        public CraftFlowHandler(CraftService craftService)
        {
            _craftService = craftService;
        }

        [Route]
        void On(CraftRecipeEvent command)
        {
            _craftService.Craft(command.RecipeId);
        }
    }
}