namespace App.Menu.View
{
    using App.Flow.Events;
    using App.Systems.Saving.Models;
    using App.Systems.Saving.Orchestration;
    using App.View;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : GameplayViewBase
    {
        private ICommandPublisher _publisher;
        private SlotManager _slotManager;

        private VisualElement _mainPanel;
        private VisualElement _slotsPanel;
        private VisualElement _slotsList;

        [Inject]
        public void Construct(ICommandPublisher publisher, SlotManager slotManager)
        {
            _publisher = publisher;
            _slotManager = slotManager;
        }

        protected override void BuildView()
        {
            var root = PrepareRoot();
            if (root == null)
            {
                return;
            }

            BuildMainPanel(root);
            BuildSlotsPanel(root);
        }

        private void BuildMainPanel(VisualElement root)
        {
            _mainPanel = new VisualElement();
            _mainPanel.AddToClassList("menu-container");

            var title = new Label("Workshop Game");
            title.AddToClassList("title-text");

            var playButton = new Button(HandlePlayClicked)
            {
                name = "play-button",
                text = "Start Game"
            };
            playButton.AddToClassList("play-button");

            _mainPanel.Add(title);
            _mainPanel.Add(playButton);
            root.Add(_mainPanel);
        }

        private void BuildSlotsPanel(VisualElement root)
        {
            _slotsPanel = new VisualElement();
            _slotsPanel.AddToClassList("menu-container");
            _slotsPanel.style.display = DisplayStyle.None;

            var header = new VisualElement();
            header.AddToClassList("slots-header");

            var title = new Label("Save Slots");
            title.AddToClassList("title-text");

            var backButton = new Button(HandleBackClicked)
            {
                name = "back-button",
                text = "← Back"
            };
            backButton.AddToClassList("play-button");

            header.Add(title);
            header.Add(backButton);
            _slotsPanel.Add(header);

            _slotsList = new VisualElement();
            _slotsList.AddToClassList("slots-list");
            _slotsPanel.Add(_slotsList);

            root.Add(_slotsPanel);
        }

        private void HandlePlayClicked()
        {
            _mainPanel.style.display = DisplayStyle.None;
            _slotsPanel.style.display = DisplayStyle.Flex;
            RefreshSlotsPanelAsync().Forget();
        }

        private void HandleBackClicked()
        {
            _slotsPanel.style.display = DisplayStyle.None;
            _mainPanel.style.display = DisplayStyle.Flex;
        }

        private async UniTask RefreshSlotsPanelAsync()
        {
            var descriptors = await _slotManager.ListSlotsAsync();

            _slotsList.Clear();

            foreach (var descriptor in descriptors)
            {
                var row = BuildSlotRow(descriptor);
                _slotsList.Add(row);
            }
        }

        private VisualElement BuildSlotRow(SlotDescriptor descriptor)
        {
            var row = new VisualElement();
            row.AddToClassList("slot-row");

            var infoPanel = new VisualElement();
            infoPanel.AddToClassList("slot-info");

            var indexLabel = new Label($"Slot #{descriptor.SlotIndex}");
            indexLabel.AddToClassList("slot-index");

            string metaText = descriptor.HasData
                ? $"Last played: {descriptor.LastPlayed.Value:yyyy-MM-dd}"
                : "(empty)";
            var metaLabel = new Label(metaText);
            metaLabel.AddToClassList("slot-meta");

            infoPanel.Add(indexLabel);
            infoPanel.Add(metaLabel);
            row.Add(infoPanel);

            var buttonsPanel = new VisualElement();
            buttonsPanel.AddToClassList("slot-buttons");

            int slotIndex = descriptor.SlotIndex;

            var playButton = new Button(() => OnSlotPlayClicked(slotIndex))
            {
                name = $"play-slot-{slotIndex}",
                text = "Play"
            };
            playButton.AddToClassList("slot-play-button");
            buttonsPanel.Add(playButton);

            if (descriptor.HasData)
            {
                var deleteButton = new Button(() => OnSlotDeleteClicked(slotIndex).Forget())
                {
                    name = $"delete-slot-{slotIndex}",
                    text = "Delete"
                };
                deleteButton.AddToClassList("slot-delete-button");
                buttonsPanel.Add(deleteButton);
            }

            row.Add(buttonsPanel);

            return row;
        }

        private void OnSlotPlayClicked(int slotIndex)
        {
            _publisher.PublishAsync(new SelectSlotEvent(slotIndex));
        }

        private async UniTask OnSlotDeleteClicked(int slotIndex)
        {
            await _publisher.PublishAsync(new DeleteSlotEvent(slotIndex));
            await RefreshSlotsPanelAsync();
        }
    }
}