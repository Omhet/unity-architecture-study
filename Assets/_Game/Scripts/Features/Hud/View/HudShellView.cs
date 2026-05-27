namespace App.Hud.View
{
    using System;
    using System.Collections.Generic;
    using App.View;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class HudShellView : GameplayViewBase
    {
        private readonly Dictionary<string, VisualElement> _sections = new Dictionary<string, VisualElement>();
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

            AddSection(tabs, content, "generators", "Generators", "Generator controls will appear here.");
            AddSection(tabs, content, "crafting", "Crafting", "Crafting recipes will appear here.");
            AddSection(tabs, content, "orders", "Orders", "Customer orders will appear here.");
            AddSection(tabs, content, "shop", "Shop", "Shop catalog will appear here.");
            AddSection(tabs, content, "quests", "Quests", "Quest progress will appear here.");
            AddSection(tabs, content, "talents", "Talents", "Talent upgrades will appear here.");

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
            _sections.Clear();
        }

        private void AddSection(VisualElement tabs, VisualElement content, string id, string title, string placeholder)
        {
            var section = new VisualElement();
            section.AddToClassList("hud-section");
            section.AddToClassList("is-hidden");

            var sectionTitle = new Label(title);
            sectionTitle.AddToClassList("hud-section-title");

            var sectionBody = new Label(placeholder);
            sectionBody.AddToClassList("hud-section-placeholder");

            section.Add(sectionTitle);
            section.Add(sectionBody);
            content.Add(section);
            _sections[id] = section;

            var tabButton = new Button();
            tabButton.text = title;
            tabButton.AddToClassList("hud-tab-button");
            tabButton.userData = (Action)(() => SetActiveSection(id));
            tabButton.clicked += (Action)tabButton.userData;

            tabs.Add(tabButton);
            _tabButtons.Add(tabButton);
            _tabIds[tabButton] = id;
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
                    pair.Value.RemoveFromClassList("is-hidden");
                    pair.Value.AddToClassList("is-visible");
                }
                else
                {
                    pair.Value.RemoveFromClassList("is-visible");
                    pair.Value.AddToClassList("is-hidden");
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
