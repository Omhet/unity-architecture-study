namespace App.View
{
    using UnityEngine.UIElements;

    public interface IGameplaySectionView
    {
        GameplaySectionDefinition Definition { get; }
        VisualElement Root { get; }

        void BuildOnce();
        void Mount();
        void Unmount();
        void Dispose();
    }
}