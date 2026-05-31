namespace App.Hud.View
{
    using ObservableCollections;
    using App.Flow.Events;
    using App.Generators.Core;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class GeneratorsSectionView : GameplaySectionViewBase
    {
        private readonly GeneratorState _generatorState;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _ownedGeneratorsSubscription;

        public GeneratorsSectionView(
            ICommandPublisher publisher,
            GeneratorState generatorState
            )
            : base(new GameplaySectionDefinition("generators", "Generators", 0))
        {
            _generatorState = generatorState;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("generators-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            _list = new VisualElement();
            _list.AddToClassList("generators-list");

            root.Add(_list);
        }

        protected override void Bind()
        {
            _ownedGeneratorsSubscription?.Dispose();

            if (_generatorState == null)
            {
                return;
            }

            var updates = Observable.Merge(
                _generatorState.PlayerOwnedGeneratorIds.ObserveAdd().Select(_ => Unit.Default),
                _generatorState.PlayerOwnedGeneratorIds.ObserveRemove().Select(_ => Unit.Default),
                _generatorState.PlayerOwnedGeneratorIds.ObserveReplace().Select(_ => Unit.Default),
                _generatorState.PlayerOwnedGeneratorIds.ObserveReset().Select(_ => Unit.Default));

            _ownedGeneratorsSubscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _ownedGeneratorsSubscription?.Dispose();
            _ownedGeneratorsSubscription = null;
        }

        private VisualElement BuildGeneratorRow(string generatorId)
        {
            var row = new VisualElement();
            row.AddToClassList("generator-card");

            var title = new Label(generatorId);
            title.AddToClassList("generator-title");
            row.Add(title);

            var generateButton = new Button(() => HandleGenerateClicked(generatorId))
            {
                text = "Generate"
            };
            generateButton.AddToClassList("generator-button");

            row.Add(generateButton);

            return row;
        }

        private void HandleGenerateClicked(string generatorId)
        {
            _publisher.PublishAsync(new GenerateFromGeneratorEvent(generatorId));
        }

        private void RebuildRows()
        {
            if (_list == null || _generatorState == null)
            {
                return;
            }

            _list.Clear();

            for (int i = 0; i < _generatorState.PlayerOwnedGeneratorIds.Count; i++)
            {
                var generatorId = _generatorState.PlayerOwnedGeneratorIds[i];

                _list.Add(BuildGeneratorRow(generatorId));
            }
        }
    }
}
