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
        private readonly GeneratorModel _generatorModel;
        private readonly PlayerGeneratorModel _playerGeneratorModel;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _ownedGeneratorsSubscription;

        public GeneratorsSectionView(
            GeneratorModel generatorModel,
            PlayerGeneratorModel playerGeneratorModel,
            ICommandPublisher publisher)
            : base(new GameplaySectionDefinition("generators", "Generators", 0))
        {
            _generatorModel = generatorModel;
            _playerGeneratorModel = playerGeneratorModel;
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

            if (_playerGeneratorModel == null)
            {
                return;
            }

            var updates = Observable.Merge(
                _playerGeneratorModel.OwnedGeneratorIds.ObserveAdd().Select(_ => Unit.Default),
                _playerGeneratorModel.OwnedGeneratorIds.ObserveRemove().Select(_ => Unit.Default),
                _playerGeneratorModel.OwnedGeneratorIds.ObserveReplace().Select(_ => Unit.Default),
                _playerGeneratorModel.OwnedGeneratorIds.ObserveReset().Select(_ => Unit.Default));

            _ownedGeneratorsSubscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _ownedGeneratorsSubscription?.Dispose();
            _ownedGeneratorsSubscription = null;
        }

        private VisualElement BuildGeneratorRow(GeneratorModel.GeneratorState generator)
        {
            var row = new VisualElement();
            row.AddToClassList("generator-row");

            var metaColumn = new VisualElement();
            metaColumn.AddToClassList("generator-meta");

            var title = new Label(generator.DisplayName);
            title.AddToClassList("generator-title");

            var details = new Label("+" + generator.AmountPerClick + " " + generator.ResourceId + " per click");
            details.AddToClassList("generator-details");

            metaColumn.Add(title);
            metaColumn.Add(details);

            var generateButton = new Button(() => HandleGenerateClicked(generator.Id))
            {
                text = "Generate"
            };
            generateButton.AddToClassList("generator-button");

            row.Add(metaColumn);
            row.Add(generateButton);
            return row;
        }

        private void HandleGenerateClicked(string generatorId)
        {
            _publisher.PublishAsync(new GenerateFromGeneratorEvent(generatorId));
        }

        private void RebuildRows()
        {
            if (_list == null || _playerGeneratorModel == null)
            {
                return;
            }

            _list.Clear();

            for (int i = 0; i < _playerGeneratorModel.OwnedGeneratorIds.Count; i++)
            {
                var generatorId = _playerGeneratorModel.OwnedGeneratorIds[i];
                if (!_generatorModel.TryGetById(generatorId, out var generator) || generator == null)
                {
                    continue;
                }

                _list.Add(BuildGeneratorRow(generator));
            }
        }
    }
}
