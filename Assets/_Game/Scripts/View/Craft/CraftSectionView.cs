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
    using App.Products.Core;

    public class CraftSectionView : GameplaySectionViewBase
    {
        private readonly RecipeState _recipeState;
        private readonly RecipeRegistry _recipeRegistry;
        private readonly ProductState _productState;
        private readonly ICommandPublisher _publisher;
        private VisualElement _recipesList;
        private VisualElement _productsList;
        private IDisposable _ownedRecipesSubscription;
        private IDisposable _ownedProductsSubscription;

        public CraftSectionView(
            ICommandPublisher publisher,
            RecipeState recipeState,
            RecipeRegistry recipeRegistry,
            ProductState productState
            )
            : base(new GameplaySectionDefinition("craft", "Craft", 0))
        {
            _recipeState = recipeState;
            _recipeRegistry = recipeRegistry;
            _productState = productState;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("craft-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            var content = new VisualElement();
            content.AddToClassList("craft-section-content");

            _recipesList = new VisualElement();
            _recipesList.AddToClassList("recipes-list");

            _productsList = new VisualElement();
            _productsList.AddToClassList("products-list");

            content.Add(_recipesList);
            content.Add(_productsList);

            root.Add(content);
        }

        protected override void Bind()
        {
            _ownedRecipesSubscription?.Dispose();
            _ownedProductsSubscription?.Dispose();

            if (_recipeState == null || _productState == null)
            {
                return;
            }

            var recipesUpdates = Observable.Merge(
                _recipeState.PlayerOwnedRecipeIds.ObserveAdd().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveRemove().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveReplace().Select(_ => Unit.Default),
                _recipeState.PlayerOwnedRecipeIds.ObserveReset().Select(_ => Unit.Default));

            _ownedRecipesSubscription = Observable.Return(Unit.Default)
                .Concat(recipesUpdates)
                .Subscribe(_ => RebuildRecipeRows());

            var productsUpdates = Observable.Merge(
                _productState.PlayerOwnedProductAmounts.ObserveAdd().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveRemove().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveReplace().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveReset().Select(_ => Unit.Default));

            _ownedProductsSubscription = Observable.Return(Unit.Default)
                .Concat(productsUpdates)
                .Subscribe(_ => RebuildProductRows());
        }

        protected override void Unbind()
        {
            _ownedRecipesSubscription?.Dispose();
            _ownedRecipesSubscription = null;

            _ownedProductsSubscription?.Dispose();
            _ownedProductsSubscription = null;
        }

        private VisualElement BuildRecipeRow(string recipeId)
        {
            var row = new VisualElement();
            row.AddToClassList("recipe-card");

            var title = new Label(recipeId);
            title.AddToClassList("recipe-title");
            row.Add(title);

            // Show ingredients list
            _recipeRegistry.TryGetById(recipeId, out var recipe);
            if (recipe != null)
            {
                var ingredientsList = new VisualElement();
                ingredientsList.AddToClassList("ingredients-list");

                foreach (var ingredient in recipe.InputResources)
                {
                    var ingredientLabel = new Label($"{ingredient.Key}: {ingredient.Value}");
                    ingredientLabel.AddToClassList("ingredient-label");
                    ingredientsList.Add(ingredientLabel);
                }

                row.Add(ingredientsList);
            }

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

        private void BuildProductRow(string productId, int productAmount)
        {
            var row = new VisualElement();
            row.AddToClassList("product-list-item");

            var title = new Label($"{productId}: {productAmount}");
            title.AddToClassList("product-title");
            row.Add(title);

            _productsList.Add(row);
        }

        private void RebuildRecipeRows()
        {
            if (_recipesList == null || _recipeState == null)
            {
                return;
            }

            _recipesList.Clear();

            for (int i = 0; i < _recipeState.PlayerOwnedRecipeIds.Count; i++)
            {
                var recipeId = _recipeState.PlayerOwnedRecipeIds[i];

                _recipesList.Add(BuildRecipeRow(recipeId));
            }
        }

        private void RebuildProductRows()
        {
            if (_productsList == null || _productState == null)
            {
                return;
            }

            _productsList.Clear();

            foreach (var pair in _productState.GetProductAmounts())
            {
                var productId = pair.Key;
                var productAmount = pair.Value;

                BuildProductRow(productId, productAmount);
            }
        }
    }
}
