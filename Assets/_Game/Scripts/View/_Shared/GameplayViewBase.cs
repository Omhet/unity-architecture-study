namespace App.View
{
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public abstract class GameplayViewBase : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        protected UIDocument UiDocument { get; private set; }

        private void Awake()
        {
            UiDocument = GetComponent<UIDocument>();
            OnViewAwake();
        }

        private void Start()
        {
            BuildView();
            BindView();
        }

        private void OnDestroy()
        {
            UnbindView();
            DisposeView();
        }

        protected virtual void OnViewAwake()
        {
        }

        protected virtual void BuildView()
        {
        }

        protected virtual void BindView()
        {
        }

        protected virtual void UnbindView()
        {
        }

        protected virtual void DisposeView()
        {
        }

        protected VisualElement PrepareRoot(bool clear = true)
        {
            if (UiDocument == null || UiDocument.rootVisualElement == null)
            {
                return null;
            }

            var root = UiDocument.rootVisualElement;
            if (clear)
            {
                root.Clear();
            }

            if (_styleSheet != null)
            {
                root.styleSheets.Add(_styleSheet);
            }

            return root;
        }
    }
}