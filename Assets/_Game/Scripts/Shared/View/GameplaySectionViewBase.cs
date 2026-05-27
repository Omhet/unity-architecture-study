namespace App.View
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.UIElements;

    public abstract class GameplaySectionViewBase : IGameplaySectionView
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private bool _isBuilt;
        private bool _isMounted;

        protected GameplaySectionViewBase(GameplaySectionDefinition definition)
        {
            Definition = definition;
        }

        public GameplaySectionDefinition Definition { get; }

        public VisualElement Root { get; private set; }

        public void BuildOnce()
        {
            if (_isBuilt)
            {
                return;
            }

            Root = new VisualElement();
            Root.AddToClassList("hud-section");
            Root.AddToClassList("is-hidden");

            BuildContent(Root);

            _isBuilt = true;
        }

        public void Mount()
        {
            if (!_isBuilt)
            {
                BuildOnce();
            }

            if (_isMounted)
            {
                return;
            }

            Root.RemoveFromClassList("is-hidden");
            Root.AddToClassList("is-visible");

            Bind();
            _isMounted = true;
        }

        public void Unmount()
        {
            if (!_isMounted || Root == null)
            {
                return;
            }

            Unbind();

            Root.RemoveFromClassList("is-visible");
            Root.AddToClassList("is-hidden");

            _isMounted = false;
        }

        public void Dispose()
        {
            Unmount();

            for (int i = 0; i < _disposables.Count; i++)
            {
                _disposables[i]?.Dispose();
            }

            _disposables.Clear();
            DisposeSection();
        }

        protected void TrackDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                _disposables.Add(disposable);
            }
        }

        protected abstract void BuildContent(VisualElement root);

        protected virtual void Bind()
        {
        }

        protected virtual void Unbind()
        {
        }

        protected virtual void DisposeSection()
        {
        }
    }
}