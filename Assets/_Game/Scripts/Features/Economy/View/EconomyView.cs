namespace App.Economy.View
{
    using System;
    using App.Economy.Core;
    using R3;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;

    [RequireComponent(typeof(UIDocument))]
    public class EconomyView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private UIDocument _uiDocument;
        private EconomyModel _economyModel;
        private Label _balanceLabel;
        private IDisposable _balanceSubscription;

        [Inject]
        public void Construct(EconomyModel economyModel)
        {
            _economyModel = economyModel;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            BuildView();
            BindModel();
        }

        private void OnDestroy()
        {
            _balanceSubscription?.Dispose();
            _balanceSubscription = null;
        }

        private void BuildView()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                return;
            }

            var root = _uiDocument.rootVisualElement;
            root.Clear();

            if (_styleSheet != null)
            {
                root.styleSheets.Add(_styleSheet);
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

        private void BindModel()
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
