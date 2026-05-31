namespace App.Hud.View
{
    using ObservableCollections;
    using App.Flow.Events;
    using App.Recipes.Core;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class CraftSectionView : GameplaySectionViewBase
    {
        private readonly RecipeState _recipeState;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _ownedRecipesSubscription;

        public CraftSectionView(
            ICommandPublisher publisher,
            RecipeState recipeState
            )
            : base(new GameplaySectionDefinition("craft", "Craft", 0))
        {
            _recipeState = recipeState;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("craft-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            _list = new VisualElement();
            _list.AddToClassList("craft-list");

            root.Add(_list);
        }

        protected override void Bind()
        {
            _ownedRecipesSubscription?.Dispose();

            if (_recipeState == null)
            {
                return;
            }

            var updates = Observable.Merge(
                _recipeState.PlayerOwnedRecipeIds.ObserveAdd().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveRemove().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveReplace().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveReset().Select(_ => Unit.Default));

            _ownedRecipesSubscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _ownedRecipesSubscription?.Dispose();
            _ownedRecipesSubscription = null;
        }

        private VisualElement BuildRecipeRow(string recipeId)
        {
            var row = new VisualElement();
            row.AddToClassList("recipe-card");

            var title = new Label(recipeId);
            title.AddToClassList("recipe-title");
            row.Add(title);

            var craftButton = new Button(() => HandleCraftClicked(recipeId))
            {
                text = "Craft"
            };
            craftButton.AddToClassList("craft-button");

            row.Add(craftButton);

            return row;
        }

        private void HandleCraftClicked(string recipeId)
        {
            _publisher.PublishAsync(new CraftRecipeEvent(recipeId));
        }

        private void RebuildRows()
        {
            if (_list == null || _recipeState == null)
            {
                return;
            }

            _list.Clear();

            for (int i = 0; i < _recipeState.PlayerOwnedRecipeIds.Count; i++)
            {
                var recipeId = _recipeState.PlayerOwnedRecipeIds[i];

                _list.Add(BuildRecipeRow(recipeId));
            }
        }
    }
}
