namespace App.Hud.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using App.View;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class HudShellView : GameplayViewBase
    {
        private readonly Dictionary<string, IGameplaySectionView> _sections = new Dictionary<string, IGameplaySectionView>();
        private readonly List<Button> _tabButtons = new List<Button>();
        private readonly Dictionary<Button, string> _tabIds = new Dictionary<Button, string>();

        private string _activeSection;

        protected override void BuildView()
        {
            var root = PrepareRoot();
            if (root == null)
            {
                return;
            }

            var shell = new VisualElement();
            shell.AddToClassList("hud-shell");

            var tabs = new VisualElement();
            tabs.AddToClassList("hud-tabs");

            var content = new VisualElement();
            content.AddToClassList("hud-content");

            shell.Add(tabs);
            shell.Add(content);
            root.Add(shell);

            var registry = BuildRegistry();
            foreach (var definition in registry.Definitions.OrderBy(x => x.TabOrder))
            {
                if (!registry.TryCreate(definition.Id, out IGameplaySectionView sectionView))
                {
                    continue;
                }

                sectionView.BuildOnce();
                content.Add(sectionView.Root);
                _sections[definition.Id] = sectionView;

                var tabButton = new Button
                {
                    text = definition.TabTitle
                };
                tabButton.AddToClassList("hud-tab-button");
                tabButton.userData = (Action)(() => SetActiveSection(definition.Id));
                tabButton.clicked += (Action)tabButton.userData;

                tabs.Add(tabButton);
                _tabButtons.Add(tabButton);
                _tabIds[tabButton] = definition.Id;
            }

            SetActiveSection("generators");
        }

        protected override void DisposeView()
        {
            foreach (var button in _tabButtons)
            {
                button.clicked -= button.userData as Action;
            }

            _tabButtons.Clear();
            _tabIds.Clear();

            foreach (var section in _sections.Values)
            {
                section.Dispose();
            }

            _sections.Clear();
        }

        private HudSectionRegistry BuildRegistry()
        {
            var registry = new HudSectionRegistry();
            registry.Register(
                new GameplaySectionDefinition("generators", "Generators", 0),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("generators", "Generators", 0),
                    "Generator controls will appear here."));
            registry.Register(
                new GameplaySectionDefinition("crafting", "Crafting", 1),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("crafting", "Crafting", 1),
                    "Crafting recipes will appear here."));
            registry.Register(
                new GameplaySectionDefinition("orders", "Orders", 2),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("orders", "Orders", 2),
                    "Customer orders will appear here."));
            registry.Register(
                new GameplaySectionDefinition("shop", "Shop", 3),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("shop", "Shop", 3),
                    "Shop catalog will appear here."));
            registry.Register(
                new GameplaySectionDefinition("quests", "Quests", 4),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("quests", "Quests", 4),
                    "Quest progress will appear here."));
            registry.Register(
                new GameplaySectionDefinition("talents", "Talents", 5),
                () => new PlaceholderSectionView(
                    new GameplaySectionDefinition("talents", "Talents", 5),
                    "Talent upgrades will appear here."));

            return registry;
        }

        private void SetActiveSection(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !_sections.ContainsKey(id))
            {
                return;
            }

            _activeSection = id;

            foreach (var pair in _sections)
            {
                if (pair.Key == _activeSection)
                {
                    pair.Value.Mount();
                }
                else
                {
                    pair.Value.Unmount();
                }
            }

            foreach (var button in _tabButtons)
            {
                bool isActive = _tabIds.TryGetValue(button, out string tabId)
                    && string.Equals(tabId, _activeSection, StringComparison.Ordinal);
                if (isActive)
                {
                    button.AddToClassList("is-active");
                }
                else
                {
                    button.RemoveFromClassList("is-active");
                }
            }
        }
    }
}
