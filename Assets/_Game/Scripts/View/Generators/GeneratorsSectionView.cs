namespace App.Hud.View
{
    using ObservableCollections;
    using App.GameConfig.Core;
    using App.Flow.Events;
    using App.Generators.Core;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class GeneratorsSectionView : GameplaySectionViewBase
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly GeneratorState _generatorState;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _ownedGeneratorsSubscription;

        public GeneratorsSectionView(
            GeneratorRegistry generatorRegistry,
            GeneratorState generatorState,
            ICommandPublisher publisher)
            : base(new GameplaySectionDefinition("generators", "Generators", 0))
        {
            _generatorRegistry = generatorRegistry;
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
                _generatorState.OwnedGeneratorIds.ObserveAdd().Select(_ => Unit.Default),
                _generatorState.OwnedGeneratorIds.ObserveRemove().Select(_ => Unit.Default),
                _generatorState.OwnedGeneratorIds.ObserveReplace().Select(_ => Unit.Default),
                _generatorState.OwnedGeneratorIds.ObserveReset().Select(_ => Unit.Default));

            _ownedGeneratorsSubscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _ownedGeneratorsSubscription?.Dispose();
            _ownedGeneratorsSubscription = null;
        }

        private VisualElement BuildGeneratorRow(GeneratorDefinition generator)
        {
            var row = new VisualElement();
            row.AddToClassList("generator-row");

            var generateButton = new Button(() => HandleGenerateClicked(generator.Id))
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

            for (int i = 0; i < _generatorState.OwnedGeneratorIds.Count; i++)
            {
                var generatorId = _generatorState.OwnedGeneratorIds[i];
                if (!_generatorRegistry.TryGetById(generatorId, out var generator) || generator == null)
                {
                    continue;
                }

                _list.Add(BuildGeneratorRow(generator));
            }
        }
    }
}
