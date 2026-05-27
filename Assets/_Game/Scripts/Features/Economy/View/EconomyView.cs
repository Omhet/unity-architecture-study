namespace App.Economy.View
{
    using System;
    using App.Economy.Core;
    using App.View;
    using R3;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;

    [RequireComponent(typeof(UIDocument))]
    public class EconomyView : GameplayViewBase
    {
        private EconomyModel _economyModel;
        private Label _balanceLabel;
        private IDisposable _balanceSubscription;

        [Inject]
        public void Construct(EconomyModel economyModel)
        {
            _economyModel = economyModel;
        }

        protected override void UnbindView()
        {
            _balanceSubscription?.Dispose();
            _balanceSubscription = null;
        }

        protected override void BuildView()
        {
            var root = PrepareRoot();
            if (root == null)
            {
                return;
            }

            var container = new VisualElement();
            container.AddToClassList("economy-container");

            _balanceLabel = new Label("$0")
            {
                name = "economy-balance"
            };
            _balanceLabel.AddToClassList("economy-balance");

            container.Add(_balanceLabel);
            root.Add(container);
        }

        protected override void BindView()
        {
            if (_economyModel == null || _balanceLabel == null)
            {
                return;
            }

            _balanceSubscription?.Dispose();
            _balanceSubscription = _economyModel.Balance.Subscribe(UpdateBalance);
        }

        private void UpdateBalance(int value)
        {
            if (_balanceLabel == null)
            {
                return;
            }

            _balanceLabel.text = "$" + value;
        }
    }
}
