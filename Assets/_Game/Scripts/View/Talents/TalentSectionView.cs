namespace App.Hud.View
{
    using System;
    using App.Flow.Events;
    using App.Talents.Core;
    using App.View;
    using R3;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class TalentSectionView : GameplaySectionViewBase
    {
        private readonly TalentState _talentState;
        private readonly TalentRegistry _talentRegistry;
        private readonly TalentService _talentService;
        private readonly ICommandPublisher _publisher;

        private Label _pointsLabel;
        private VisualElement _list;
        private IDisposable _availablePointsSubscription;

        public TalentSectionView(
            TalentState talentState,
            TalentRegistry talentRegistry,
            TalentService talentService,
            ICommandPublisher publisher)
            : base(new GameplaySectionDefinition("talents", "Talents", 5))
        {
            _talentState = talentState;
            _talentRegistry = talentRegistry;
            _talentService = talentService;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("talents-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            var pointsContainer = new VisualElement();
            pointsContainer.AddToClassList("talent-points-container");

            var pointsLabelPrefix = new Label("Available Points: ");
            pointsLabelPrefix.AddToClassList("talent-points-label");
            pointsContainer.Add(pointsLabelPrefix);

            _pointsLabel = new Label(_talentState.AvailablePoints.Value.ToString());
            _pointsLabel.AddToClassList("talent-points-value");
            pointsContainer.Add(_pointsLabel);

            root.Add(pointsContainer);

            _list = new VisualElement();
            _list.AddToClassList("talents-list");
            root.Add(_list);

            RebuildRows();
        }

        protected override void Bind()
        {
            _availablePointsSubscription?.Dispose();

            _availablePointsSubscription = _talentState.AvailablePoints.Subscribe(value =>
            {
                if (_pointsLabel != null)
                {
                    _pointsLabel.text = value.ToString();
                }

                RebuildRows();
            });
        }

        protected override void Unbind()
        {
            _availablePointsSubscription?.Dispose();
            _availablePointsSubscription = null;
        }

        VisualElement BuildTalentRow(int index)
        {
            var talent = _talentRegistry[index];
            if (talent == null) return new VisualElement();

            var pointsSpent = _talentService.GetPointsSpent(talent.Id);
            bool isMaxed = pointsSpent >= talent.MaxPoints;
            int availablePoints = _talentState.AvailablePoints.Value;
            bool canAfford = availablePoints >= talent.Cost;

            var row = new VisualElement();
            row.AddToClassList("talent-card");

            var titleLabel = new Label(talent.Name);
            titleLabel.AddToClassList("talent-name");
            row.Add(titleLabel);

            var investmentLabel = new Label($"{pointsSpent}/{talent.MaxPoints}");
            investmentLabel.AddToClassList("talent-investment");
            row.Add(investmentLabel);

            var buyButton = new Button(() => HandleBuyClicked(talent.Id))
            {
                text = "Buy"
            };
            buyButton.AddToClassList("talent-buy-button");
            buyButton.SetEnabled(!isMaxed && canAfford);

            if (isMaxed)
            {
                buyButton.text = "MAX";
            }

            row.Add(buyButton);

            return row;
        }

        void HandleBuyClicked(string talentId)
        {
            _publisher.PublishAsync(new PurchaseTalentEvent(talentId));
            RebuildRows();
        }

        void RebuildRows()
        {
            if (_list == null || _talentRegistry == null) return;

            _list.Clear();

            for (int i = 0; i < _talentRegistry.Count; i++)
            {
                _list.Add(BuildTalentRow(i));
            }
        }
    }
}
