using R3;

namespace App.Progression.Core
{
    public class ProgressionState
    {
        public ReactiveProperty<int> Level { get; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> Xp { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> NextLevelXp { get; } = new ReactiveProperty<int>(0);
    }
}
