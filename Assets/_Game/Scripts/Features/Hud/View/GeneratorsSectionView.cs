namespace App.Hud.View
{
    using App.Flow.Events;
    using App.Generators.Core;
    using App.View;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class GeneratorsSectionView : GameplaySectionViewBase
    {
        private readonly GeneratorModel _generatorModel;
        private readonly ICommandPublisher _publisher;

        public GeneratorsSectionView(
            GeneratorModel generatorModel,
            ICommandPublisher publisher)
            : base(new GameplaySectionDefinition("generators", "Generators", 0))
        {
            _generatorModel = generatorModel;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("generators-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            var list = new VisualElement();
            list.AddToClassList("generators-list");

            for (int i = 0; i < _generatorModel.Generators.Count; i++)
            {
                var generator = _generatorModel.Generators[i];
                if (generator == null || string.IsNullOrWhiteSpace(generator.Id))
                {
                    continue;
                }

                list.Add(BuildGeneratorRow(generator));
            }

            root.Add(list);
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
    }
}
