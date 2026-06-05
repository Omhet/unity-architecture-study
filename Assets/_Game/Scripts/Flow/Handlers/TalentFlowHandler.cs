namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Talents.Core;
    using VitalRouter;

    [Routes]
    public partial class TalentFlowHandler
    {
        private readonly TalentService _talentService;

        public TalentFlowHandler(TalentService talentService)
        {
            _talentService = talentService;
        }

        [Route]
        void On(PurchaseTalentEvent command)
        {
            _talentService.TryPurchase(command.TalentId);
        }
    }
}
