namespace App.Hud.View
{
    using System;
    using System.Collections.Generic;
    using App.View;
    using UnityEngine.UIElements;

    public class HudSectionRegistry
    {
        private readonly List<GameplaySectionDefinition> _definitions = new List<GameplaySectionDefinition>();
        private readonly Dictionary<string, Func<IGameplaySectionView>> _factories = new Dictionary<string, Func<IGameplaySectionView>>();

        public IReadOnlyList<GameplaySectionDefinition> Definitions => _definitions;

        public void Register(GameplaySectionDefinition definition, Func<IGameplaySectionView> factory)
        {
            if (string.IsNullOrWhiteSpace(definition.Id) || factory == null)
            {
                return;
            }

            _definitions.Add(definition);
            _factories[definition.Id] = factory;
        }

        public bool TryCreate(string sectionId, out IGameplaySectionView sectionView)
        {
            sectionView = null;
            if (string.IsNullOrWhiteSpace(sectionId))
            {
                return false;
            }

            if (!_factories.TryGetValue(sectionId, out Func<IGameplaySectionView> factory))
            {
                return false;
            }

            sectionView = factory();
            return sectionView != null;
        }
    }

    public class PlaceholderSectionView : GameplaySectionViewBase
    {
        private readonly string _placeholder;

        public PlaceholderSectionView(GameplaySectionDefinition definition, string placeholder)
            : base(definition)
        {
            _placeholder = placeholder;
        }

        protected override void BuildContent(VisualElement root)
        {
            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");

            var sectionBody = new Label(_placeholder);
            sectionBody.AddToClassList("hud-section-placeholder");

            root.Add(sectionTitle);
            root.Add(sectionBody);
        }
    }
}