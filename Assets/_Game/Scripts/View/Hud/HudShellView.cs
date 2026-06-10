namespace App.Hud.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using App.Economy.Core;
    using App.Resources.Core;
    using App.Progression.Core;
    using App.View;
    using ObservableCollections;
    using R3;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;
    using App.Flow.Events;

    [RequireComponent(typeof(UIDocument))]
    public class HudShellView : GameplayViewBase
    {
        private readonly Dictionary<string, IGameplaySectionView> _sections = new Dictionary<string, IGameplaySectionView>();
        private readonly List<Button> _tabButtons = new List<Button>();
        private readonly Dictionary<Button, string> _tabIds = new Dictionary<Button, string>();
        private readonly Dictionary<string, Label> _resourceLabels = new Dictionary<string, Label>();

        private EconomyState _economyState;
        private ResourceState _resourceState;
        private ProgressionState _progressionState;
        private ProgressionRegistry _progressionRegistry;
        private Func<GameplaySectionDefinition, IGameplaySectionView> _sectionFactory;
        private string _activeSection;
        private Label _moneyValue;
        private VisualElement _resourcesRow;
        private Label _levelLabel;
        private Label _xpLabel;
        private IDisposable _moneySubscription;
        private IDisposable _resourceSubscription;
        private IDisposable _levelSubscription;
        private IDisposable _xpSubscription;

        private ICommandPublisher _publisher;
        private Button _saveButton;
        private Button _menuButton;

        [Inject]
        public void Construct(
            EconomyState economyState,
            ResourceState resourceState,
            ProgressionState progressionState,
            ProgressionRegistry progressionRegistry,
            Func<GameplaySectionDefinition, IGameplaySectionView> sectionFactory,
            ICommandPublisher publisher)
        {
            _economyState = economyState;
            _resourceState = resourceState;
            _progressionState = progressionState;
            _progressionRegistry = progressionRegistry;
            _sectionFactory = sectionFactory;
            _publisher = publisher;
        }

        protected override void BuildView()
        {
            var root = PrepareRoot();
            if (root == null) return;

            var shell = new VisualElement();
            shell.AddToClassList("hud-shell");

            var actionRow = BuildActionRow();
            var statusBar = BuildStatusBar();

            var tabs = new VisualElement();
            tabs.AddToClassList("hud-tabs");

            var content = new VisualElement();
            content.AddToClassList("hud-content");

            shell.Add(actionRow);
            shell.Add(statusBar);
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
            UnbindView();

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
            _resourceLabels.Clear();
            _moneyValue = null;
            _resourcesRow = null;
            _levelLabel = null;
            _xpLabel = null;
            _saveButton = null;
            _menuButton = null;
        }

        protected override void BindView()
        {
            _moneySubscription?.Dispose();
            _resourceSubscription?.Dispose();
            _levelSubscription?.Dispose();
            _xpSubscription?.Dispose();

            if (_economyState != null)
            {
                _moneySubscription = _economyState.Balance.Subscribe(UpdateMoney);
            }

            if (_resourceState != null)
            {
                var updates = Observable.Merge(
                    _resourceState.Balances.ObserveAdd().Select(_ => Unit.Default),
                    _resourceState.Balances.ObserveRemove().Select(_ => Unit.Default),
                    _resourceState.Balances.ObserveReplace().Select(_ => Unit.Default),
                    _resourceState.Balances.ObserveReset().Select(_ => Unit.Default));

                _resourceSubscription = Observable.Return(Unit.Default)
                    .Concat(updates)
                    .Subscribe(_ => RefreshResources());
            }

            if (_progressionState != null)
            {
                Func<int> getNextLevelXp = () => _progressionRegistry.GetNextLevelXpForLevel(_progressionState.Level.Value);

                var updates = Observable.Merge(
                    _progressionState.Level.Select(_ => Unit.Default),
                    _progressionState.Xp.Select(_ => Unit.Default));

                _levelSubscription = _progressionState.Level.Subscribe(level => UpdateLevel(level));
                _xpSubscription = Observable.Return(Unit.Default)
                    .Concat(updates)
                    .Subscribe(_ => UpdateXp(_progressionState.Xp.Value, getNextLevelXp()));
            }
        }

        protected override void UnbindView()
        {
            _moneySubscription?.Dispose();
            _resourceSubscription?.Dispose();
            _moneySubscription = null;
            _resourceSubscription = null;
            _levelSubscription?.Dispose();
            _xpSubscription?.Dispose();
            _levelSubscription = null;
            _xpSubscription = null;
        }

        private HudSectionRegistry BuildRegistry()
        {
            var registry = new HudSectionRegistry();
            registry.Register(
                new GameplaySectionDefinition("generators", "Generators", 0),
                () => CreateFromFactory(new GameplaySectionDefinition("generators", "Generators", 0)));
            registry.Register(
                new GameplaySectionDefinition("crafting", "Crafting", 1),
                () => CreateFromFactory(new GameplaySectionDefinition("crafting", "Crafting", 1)));
            registry.Register(
                new GameplaySectionDefinition("orders", "Orders", 2),
                () => CreateFromFactory(new GameplaySectionDefinition("orders", "Orders", 2)));
            registry.Register(
                new GameplaySectionDefinition("shop", "Shop", 3),
                () => CreateFromFactory(new GameplaySectionDefinition("shop", "Shop", 3)));
            registry.Register(
                new GameplaySectionDefinition("quests", "Quests", 4),
                () => CreateFromFactory(new GameplaySectionDefinition("quests", "Quests", 4)));
            registry.Register(
                new GameplaySectionDefinition("talents", "Talents", 5),
                () => CreateFromFactory(new GameplaySectionDefinition("talents", "Talents", 5)));

            return registry;
        }

        private IGameplaySectionView CreateFromFactory(GameplaySectionDefinition definition)
        {
            if (_sectionFactory != null)
            {
                var sectionView = _sectionFactory(definition);
                if (sectionView != null)
                {
                    return sectionView;
                }
            }

            return new PlaceholderSectionView(definition, definition.TabTitle + " section is not available.");
        }

        private VisualElement BuildActionRow()
        {
            var actionRow = new VisualElement();
            actionRow.AddToClassList("hud-actions-row");

            _saveButton = new Button
            {
                text = "Save"
            };
            _saveButton.AddToClassList("hud-action-button");
            _saveButton.clicked += () =>
            {
                _publisher.PublishAsync(new ManualSaveEvent());
            };

            _menuButton = new Button
            {
                text = "← Menu"
            };
            _menuButton.AddToClassList("hud-action-button");
            _menuButton.clicked += () =>
            {
                _publisher.PublishAsync(new ExitToMenuEvent());
            };

            actionRow.Add(_saveButton);
            actionRow.Add(_menuButton);

            return actionRow;
        }

        private VisualElement BuildStatusBar()
        {
            var statusBar = new VisualElement();
            statusBar.AddToClassList("hud-status-bar");

            var moneyBlock = new VisualElement();
            moneyBlock.AddToClassList("hud-money-block");

            var moneyTitle = new Label("Money");
            moneyTitle.AddToClassList("hud-money-title");
            _moneyValue = new Label("$0");
            _moneyValue.AddToClassList("hud-money-value");

            moneyBlock.Add(moneyTitle);
            moneyBlock.Add(_moneyValue);

            var resourcesBlock = new VisualElement();
            resourcesBlock.AddToClassList("hud-resources-block");

            var resourcesTitle = new Label("Resources");
            resourcesTitle.AddToClassList("hud-resources-title");

            _resourcesRow = new VisualElement();
            _resourcesRow.AddToClassList("hud-resources-row");

            resourcesBlock.Add(resourcesTitle);
            resourcesBlock.Add(_resourcesRow);

            var progressionBlock = new VisualElement();
            progressionBlock.AddToClassList("hud-progression-block");

            var progressionTitle = new Label("Level");
            progressionTitle.AddToClassList("hud-progression-title");

            _levelLabel = new Label("Lv 1");
            _levelLabel.AddToClassList("hud-progression-level");

            _xpLabel = new Label("0/0");
            _xpLabel.AddToClassList("hud-progression-xp");

            progressionBlock.Add(progressionTitle);
            progressionBlock.Add(_levelLabel);
            progressionBlock.Add(_xpLabel);

            statusBar.Add(moneyBlock);
            statusBar.Add(resourcesBlock);
            statusBar.Add(progressionBlock);

            return statusBar;
        }

        private void UpdateMoney(int value)
        {
            if (_moneyValue != null)
            {
                _moneyValue.text = "$" + value;
            }
        }

        private void UpdateLevel(int level)
        {
            if (_levelLabel != null)
            {
                _levelLabel.text = "Lv " + level;
            }
        }

        private void UpdateXp(int xp, int next)
        {
            Debug.Log($"Updating XP display: xp={xp}, next={next}");
            if (_xpLabel != null)
            {
                if (next > 0)
                {
                    _xpLabel.text = xp + "/" + next;
                }
                else
                {
                    _xpLabel.text = xp + "/-";
                }
            }
        }

        private void RefreshResources()
        {
            if (_resourcesRow == null || _resourceState == null)
            {
                return;
            }

            _resourcesRow.Clear();
            _resourceLabels.Clear();

            foreach (var pair in _resourceState.EnumerateBalances())
            {
                var chip = new Label(pair.Key + ": " + pair.Value);
                chip.AddToClassList("hud-resource-chip");
                _resourcesRow.Add(chip);
                _resourceLabels[pair.Key] = chip;
            }
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
